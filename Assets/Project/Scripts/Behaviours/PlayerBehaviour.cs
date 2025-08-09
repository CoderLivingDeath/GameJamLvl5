using GameJamLvl5.Project.Infrastructure.EventBus;
using GameJamLvl5.Project.Infrastructure.EventBus.Subscribers;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(MovementBehaviour))]
public class PlayerBehaviour : MonoBehaviour, IGameplay_MovementEventHandler
{
    [Inject] private EventBus _eventBus;

    private MovementBehaviour _movementBehaviour;

    public void HandleMovement(Vector2 direction)
    {
        _movementBehaviour.Move(direction);
    }

    #region Unity Methods
    private void Awake()
    {
        _eventBus.Subscribe(this);
    }

    private void Start()
    {
        _movementBehaviour = GetComponent<MovementBehaviour>();
    }
    #endregion
}