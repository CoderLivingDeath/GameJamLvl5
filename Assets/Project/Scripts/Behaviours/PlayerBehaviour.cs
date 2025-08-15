using GameJamLvl5.Project.Infrastructure.EventBus;
using GameJamLvl5.Project.Infrastructure.EventBus.Subscribers;
using GameJamLvl5.Project.Scripts.Services.InputService;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(MovementBehaviour), typeof(InteractionBehaviour), typeof(AudioSource))]
public class PlayerBehaviour : MonoBehaviour, IGameplay_MovementEventHandler, IGameplay_InteractEventHandler
{
    public AudioSource StepsAudio => _stepsAudio;
    [SerializeField]
    private AudioSource _stepsAudio;

    public MovementBehaviour MovementBehaviour => _movementBehaviour;
    private MovementBehaviour _movementBehaviour;

    public InteractionBehaviour InteractionBehaviour => _interactionBehaviour;
    private InteractionBehaviour _interactionBehaviour;
    [Inject] private EventBus _eventBus;
    [Inject] private InputService _inputService;

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

    private void InputService_OnDisabled(string filter)
    {
        _movementBehaviour.Move(Vector2.zero);
    }

    void OnEnable()
    {
        _inputService.OnDisabledBy += InputService_OnDisabled;
    }

    void OnDisable()
    {
        _inputService.OnDisabledBy -= InputService_OnDisabled;
    }
    #endregion
}