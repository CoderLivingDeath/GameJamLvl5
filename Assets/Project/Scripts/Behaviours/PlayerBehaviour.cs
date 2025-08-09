using GameJamLvl5.Project.Infrastructure.EventBus;
using GameJamLvl5.Project.Infrastructure.EventBus.Subscribers;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(MovementBehaviour), typeof(InteractionBehaviour))]
public class PlayerBehaviour : MonoBehaviour, IGameplay_MovementEventHandler, IGameplay_InteractEventHandler
{
    [Inject] private EventBus _eventBus;

    private MovementBehaviour _movementBehaviour;
    private InteractionBehaviour _interactionBehaviour;

    public void HandleInteract(bool button)
    {
        _interactionBehaviour.Interact();
    }

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
        _interactionBehaviour = GetComponent<InteractionBehaviour>();
    }
    #endregion
}