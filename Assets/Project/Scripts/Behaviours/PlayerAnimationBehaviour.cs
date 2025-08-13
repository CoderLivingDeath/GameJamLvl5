using System;
using System.Linq;
using UnityEngine;

public class PlayerAnimationBehaviour : MonoBehaviour
{

    public const string PLAYER_ANIMATION_TRIGGER_IDLE = "IsIdle";
    public const string PLAYER_ANIMATION_TRIGGER_MOVE = "IsMove";

    public bool FlipToDirection => _flipToDirection;
    [SerializeField]
    private bool _flipToDirection = true;

    private string[] _stateTriggers = new string[]
    {
    PLAYER_ANIMATION_TRIGGER_IDLE,
    PLAYER_ANIMATION_TRIGGER_MOVE
    };

    public Animator Animator => _animator;
    [SerializeField]
    private Animator _animator;

    public PlayerBehaviour PlayerBehaviour => _playerBehaviour;
    [SerializeField]
    private PlayerBehaviour _playerBehaviour;

    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    public MovementBehaviour MovementBehaviour => _movementBehaviour;
    [SerializeField]
    private MovementBehaviour _movementBehaviour;

    public void SetStateTrigger(string triggerName)
    {
        ResetAllStateTriggers();
        _animator.SetTrigger(triggerName);
    }

    private void ResetAllStateTriggers()
    {
        foreach (var trigger in _stateTriggers)
        {
            _animator.ResetTrigger(trigger);
        }
    }
    private void SetIdle()
    {
        SetStateTrigger(PLAYER_ANIMATION_TRIGGER_IDLE);
    }

    private void SetMove()
    {
        SetStateTrigger(PLAYER_ANIMATION_TRIGGER_MOVE);
    }

    public void SwitchAnimationState(MovementStateMachine.State state)
    {
        switch (state)
        {
            case MovementStateMachine.State.Idle:
                SetIdle();
                break;
            case MovementStateMachine.State.Move:
                SetMove();
                break;
        }
    }

    private void ValidateAnimator(Animator animator)
    {
        // Проверяем существование параметра IsIdle
        if (!ParameterExists(animator, PLAYER_ANIMATION_TRIGGER_IDLE, AnimatorControllerParameterType.Trigger))
        {
            Debug.LogError($"Параметр аниматора '{PLAYER_ANIMATION_TRIGGER_IDLE}' не найден или имеет неверный тип!");
        }

        // Проверяем существование параметра IsMove
        if (!ParameterExists(animator, PLAYER_ANIMATION_TRIGGER_MOVE, AnimatorControllerParameterType.Trigger))
        {
            Debug.LogError($"Параметр аниматора '{PLAYER_ANIMATION_TRIGGER_MOVE}' не найден или имеет неверный тип!");
        }
    }

    private bool ParameterExists(Animator animator, string paramName, AnimatorControllerParameterType paramType)
    {
        if (animator == null || !animator.isInitialized || animator.parameters == null)
        {
            return false;
        }

        foreach (var param in animator.parameters)
        {
            if (param.name == paramName && param.type == paramType)
            {
                return true;
            }
        }
        return false;
    }


    private void _movementBehaviour_PositionChanged(Vector2 pos)
    {
        Vector2 direction = _playerBehaviour.MovementBehaviour.DirectionVector;

        // Проверяем направление движения по горизонтали
        bool shouldFlip = direction.x < 0;

        _spriteRenderer.flipX = shouldFlip;
    }

    private void _movementBehaviour_StateChanged(MovementStateMachine.State obj)
    {
        SwitchAnimationState(obj);
    }


    #region Unity Methods

    private void Start()
    {
        ValidateAnimator(_animator);
    }

    private void OnEnable()
    {
        if (FlipToDirection)
        {
            _movementBehaviour.OnPositionChanged += _movementBehaviour_PositionChanged;
        }
        _movementBehaviour.OnStateChanged += _movementBehaviour_StateChanged;
    }


    private void OnDisable()
    {
        if (FlipToDirection)
        {
            _movementBehaviour.OnPositionChanged -= _movementBehaviour_PositionChanged;
        }
        _movementBehaviour.OnStateChanged -= _movementBehaviour_StateChanged;
    }


    #endregion
}