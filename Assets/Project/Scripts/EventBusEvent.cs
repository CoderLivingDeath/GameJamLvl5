using GameJamLvl5.Project.Infrastructure.EventBus;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class EventBusEvent : MonoBehaviour, IProgressionEventHandler
{
    public string Key;
    public UnityEvent OnEvent;

    [Inject] private EventBus _eventBus;

    public void HandleProgressionEvent(string key)
    {
        if (enabled && key == Key)
        {
            Debug.Log("2");
            OnEvent?.Invoke();
        }
    }

    void OnEnable()
    {
        _eventBus.Subscribe(this);
    }

    void OnDisable()
    {
        _eventBus.Unsubscribe(this);
    }
}
