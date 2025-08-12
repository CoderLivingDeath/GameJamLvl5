using GameJamLvl5.Project.Infrastructure.EventBus;
using GameJamLvl5.Project.Infrastructure.EventBus.Subscribers;
using UnityEngine;
using Zenject;

public class MouseTrackerBehaviour : MonoBehaviour, IGameplay_Mouse_PositionEventHandler
{
    [SerializeField] private Vector2 _mousePosition;

    [Inject] EventBus _eventBus;

    private Vector2 ScreenToWorldMousePosition(Vector2 pos)
    {
        Camera camera = Camera.main;
        return camera.ScreenToWorldPoint(pos);
    }

    #region Event handlers

    public void HandleMousePosition(Vector2 position)
    {
        _mousePosition = ScreenToWorldMousePosition(position);
    }
    #endregion

    #region Unity methods
    private void Awake()
    {
        _eventBus.Subscribe(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(_mousePosition, 1f);
    }

    #endregion
}
