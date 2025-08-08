using GameJamLvl5.Project.Infrastructure.EventBus;
using GameJamLvl5.Project.Infrastructure.EventBus.Subscribers;
using UnityEngine;
using Zenject;

public class EventTestBehaviour : MonoBehaviour, IGameplay_InteractEventHandler, IGameplay_Mouse_PositionEventHandler, IGameplay_MovementEventHandler
{

    [Inject] private EventBus _eventBus;

    public void HandleInteract(bool button)
    {
        Debug.Log("Handle Interact, " + button);
    }

    public void HandleMousePosition(Vector2 position)
    {
        Debug.Log("Handle mouse position, " + position);
    }

    public void HandleMovement(Vector2 direction)
    {
        Debug.Log("Handle movement, " + direction);
    }

    void Start()
    {
        _eventBus.Subscribe(this);
    }
}
