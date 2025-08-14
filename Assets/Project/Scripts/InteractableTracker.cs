using UnityEngine;
using UnityEngine.Events;

public class InteractableTracker : MonoBehaviour
{
    public UnityEvent OnTrackEnter;
    public UnityEvent OnTrackExit;
    public bool _isTracked;

    public void TrackEnter()
    {
        if (enabled)
            OnTrackEnter?.Invoke();
    }

    public void TrackExit()
    {
        if (enabled)
            OnTrackExit?.Invoke();
    }
}
