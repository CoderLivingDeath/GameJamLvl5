using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementBehaviour : MonoBehaviour
{
    [SerializeField]
    private float _velocity;
    [SerializeField]
    private Vector2 _inputVector;
    [SerializeField]
    private Vector2 _directionVector;

    [SerializeField]
    private PlayerLoopTiming _timing = PlayerLoopTiming.FixedUpdate;

    private Rigidbody2D _rigidbody2D;

    [SerializeField]
    private ContactDirectionProvider _contactDirectionProvider;

    private UniTaskCoroutine _movementCorutine;
    private UniTaskCoroutine _directionMonitoringCorutine;

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
        _inputVector = direction;
    }

    public bool IsMoving()
    {
        return _velocity != 0;
    }

    #region Corutines

    private async UniTask DirectionMonitoringTask(CancellationToken token)
    {
        await UniTask.WaitForSeconds(1);
        while (!token.IsCancellationRequested)
        {
            if (_inputVector != Vector2.zero)
            {
                _velocity = 0.3f;
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
                _rigidbody2D.MovePosition(position + offset);
            }
            // movement
            await UniTask.Yield(_timing, token);
        }
    }
    #endregion

    #region Unity Methdos
    private void Awake()
    {
        _movementCorutine = new UniTaskCoroutine(MovementTask);
        _directionMonitoringCorutine = new UniTaskCoroutine(DirectionMonitoringTask);

        _disposerContainer.Add(_movementCorutine);
        _disposerContainer.Add(_directionMonitoringCorutine);
    }

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _movementCorutine.Run();
        _directionMonitoringCorutine.Run();
    }

    private void OnDisable()
    {
        _movementCorutine.Stop();
        _directionMonitoringCorutine.Stop();
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
