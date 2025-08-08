using UnityEngine;
using Template.Project.Infrastructure.EventBus;
namespace Template.Project.Infrastructure.EventBus.Subscribers
{
    public interface IMouseTrackEventHandler : IGlobalSubscriber
    {
        void HandleMouseTrack(Vector2 mousePosition);
    }

}