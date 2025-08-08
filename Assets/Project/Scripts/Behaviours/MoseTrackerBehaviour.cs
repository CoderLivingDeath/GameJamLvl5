using Template.Project.Infrastructure.EventBus;
using Template.Project.Infrastructure.EventBus.Subscribers;
using UnityEngine;
using Zenject;

public class MoseTrackerBehaviour : MonoBehaviour, IMouseTrackEventHandler
{
    [SerializeField] private Vector2 _mousePosition;

    // инъекция зависимосткй
    [Inject] EventBus _eventBus;

    private Vector2 ScreenToWorldMousePosition(Vector2 pos)
    {
        Camera camera = Camera.main;
        return camera.ScreenToWorldPoint(pos);
    }

    #region Event handlers
    public void HandleMouseTrack(Vector2 mousePosition)
    {
        _mousePosition = ScreenToWorldMousePosition(mousePosition);
    }
    #endregion

    #region Unity methods
    private void Awake()
    {
        // регистрация обработчика в системе событий
        _eventBus.Subscribe(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(_mousePosition, 1f);
    }
    #endregion
}
