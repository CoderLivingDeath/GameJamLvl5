using GameJamLvl5.Project.Infrastructure.EventBus;
using GameJamLvl5.Project.Infrastructure.EventBus.Subscribers;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBehaviour : MonoBehaviour, IGameplay_MovementEventHandler
{
    private Rigidbody2D _rigidbody2D;

    [Inject] private EventBus _eventBus;

    private Vector2 _InputMovementDirectionVector;

    public void HandleMovement(Vector2 direction)
    {
        _InputMovementDirectionVector = direction;
    }

    #region Unity Methods
    private void Awake()
    {
        _eventBus.Subscribe(this);
    }

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_InputMovementDirectionVector != Vector2.zero)
        {
            var offset = _InputMovementDirectionVector / 5;
            _rigidbody2D.MovePosition(_rigidbody2D.position + offset);
        }
    }
    #endregion
}