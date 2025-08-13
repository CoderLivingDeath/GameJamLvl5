using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementBehaviour : MonoBehaviour
{
    [SerializeField]
    private bool _blockX;

    [SerializeField]
    private bool _blockY;

    public MovementStateMachine.State State => _movementStateMachine.CurrentState;

    [SerializeField]
    private MovementStateMachine.State _state;

    public event Action<MovementStateMachine.State> OnStateChanged;
    public event Action<Vector2> OnPositionChanged;

    [SerializeField]
    private float _maxVelocity = 0.2f;

    public float Velocity => _velocity;

    [SerializeField]
    private float _velocity;

    public Vector2 DirectionVector => _directionVector;
    [SerializeField]
    private Vector2 _directionVector;

    private Vector2 _inputVector;


    [SerializeField]
    private PlayerLoopTiming _timing = PlayerLoopTiming.FixedUpdate;

    private Rigidbody2D _rigidbody2D;

    public MovementStateMachine MovementStateMachine => _movementStateMachine;
    private MovementStateMachine _movementStateMachine;

    [SerializeField]
    private ContactDirectionProvider _contactDirectionProvider;

    private UniTaskCoroutine _movementCorutine;
    private UniTaskCoroutine _directionMonitoringCorutine;

    private UniTaskCoroutine _isMovingMonitoringCorutine;

    private DisposerContainer _disposerContainer = new();

    private bool CanMove()
    {
        if (_contactDirectionProvider.HasContactDirection(ContactDirectionProvider.CollisionDirection.Down))
        {
            return true;
        }
        return false;
    }

    public void Move(Vector2 direction)
    {
        if (_blockX) direction.x = 0;
        if (_blockY) direction.y = 0;
        _inputVector = direction;
    }

    public bool IsMoving()
    {
        return _velocity != 0;
    }
    private void MovementStateMachine_OnStateChanged(MovementStateMachine.State state)
    {
        OnStateChanged?.Invoke(state);
    }

    #region Corutines

    private async UniTask DirectionMonitoringTask(CancellationToken token)
    {
        await UniTask.WaitForSeconds(1);
        while (!token.IsCancellationRequested)
        {
            if (_inputVector != Vector2.zero)
            {
                _velocity = _maxVelocity;
            }
            else
            {
                _velocity = 0;
            }
            if (_inputVector != Vector2.zero)
            {
                var contacts = _contactDirectionProvider.GetContactsByDirection(ContactDirectionProvider.CollisionDirection.Down, false);
                if (contacts.Count() > 0)
                {
                    Vector2 normal = contacts.First().normal;
                    _directionVector = CalculateSurfaceVector(_inputVector, normal);
                }
            }
            await UniTask.Yield(PlayerLoopTiming.LastFixedUpdate, token);
        }


        Vector2 CalculateSurfaceVector(Vector2 forward, Vector2 normal)
        {
            // Первый касательный вектор — повернем нормаль на 90 градусов
            Vector2 tangent = new Vector2(normal.y, -normal.x);

            // Проверим, в какую сторону надо повернуть касательный вектор,
            // чтобы он был направлен как можно ближе к forward
            if (Vector2.Dot(tangent, forward) < 0)
            {
                tangent = -tangent;
            }

            return tangent.normalized;
        }
    }

    private async UniTask MovementTask(CancellationToken token)
    {
        await UniTask.WaitForSeconds(1);
        while (!token.IsCancellationRequested)
        {
            if (CanMove())
            {
                Vector2 position = _rigidbody2D.position;
                Vector2 offset = _velocity * _directionVector;
                _rigidbody2D.MovePosition(position + offset * Time.deltaTime * 30);
                OnPositionChanged?.Invoke(_rigidbody2D.position);
            }
            // movement
            await UniTask.Yield(_timing, token);
        }
    }

    private async UniTask IsMovingMonitoringTask(CancellationToken token)
    {
        bool prevMove = IsMoving();
        while (!token.IsCancellationRequested)
        {
            await UniTask.WaitUntil(() => prevMove != IsMoving(), _timing, token);
            _movementStateMachine.UpdateState(IsMoving());
            prevMove = IsMoving();
        }
    }
    #endregion

    #region Unity Methdos
    private void Awake()
    {
        _movementCorutine = new UniTaskCoroutine(MovementTask);
        _directionMonitoringCorutine = new UniTaskCoroutine(DirectionMonitoringTask);
        _isMovingMonitoringCorutine = new UniTaskCoroutine(IsMovingMonitoringTask);

        _disposerContainer.Add(_movementCorutine);
        _disposerContainer.Add(_directionMonitoringCorutine);
        _disposerContainer.Add(_isMovingMonitoringCorutine);

        _movementStateMachine = new();
    }

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        _state = State;
    }

    private void OnEnable()
    {
        _movementCorutine.Run();
        _directionMonitoringCorutine.Run();
        _isMovingMonitoringCorutine.Run();

        _movementStateMachine.OnStateChanged += MovementStateMachine_OnStateChanged;
    }


    private void OnDisable()
    {
        _movementCorutine.Stop();
        _directionMonitoringCorutine.Stop();
        _isMovingMonitoringCorutine.Stop();

        _movementStateMachine.OnStateChanged += MovementStateMachine_OnStateChanged;
    }

    void OnDestroy()
    {
        _disposerContainer.Dispose();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + _inputVector);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + _directionVector);
    }
    #endregion
}
