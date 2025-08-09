using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopupView : ViewBehaviourBase
{
    private readonly ReactiveProperty<bool> _isShown = new ReactiveProperty<bool>(false);
    public IObservable<bool> OnShow => _isShown.Where(isShown => isShown);
    public IObservable<bool> OnClose => _isShown.Where(isShown => !isShown);

    [SerializeField]
    private RectTransform _canvas;

    [SerializeField]
    private Button _closebutton;

    #region Commands

    public ICommand CloseCommand => CloseCommand;
    private ICommand _closeCommand;
    private void OnCloseCommand(object param)
    {
        CloseAsync().Forget();
    }

    private bool CanCloseCommand(object param)
    {
        return true;
    }

    private void CommandsSetup()
    {
        _closeCommand = new LambdaCommand(OnCloseCommand, CanCloseCommand);
    }
    #endregion

    private Vector2 CalculateTargetPosition(Vector2 canvasSize, Vector2 elementSize, Vector2 currentAnchoredPosition)
    {
        float targetX = (canvasSize.x / 2) + (elementSize.x / 2);
        return new Vector2(targetX, currentAnchoredPosition.y);
    }

    private async UniTask AnimateClose()
    {
        RectTransform thisUIElement = (RectTransform)transform;
        Vector2 canvasSize = _canvas.rect.size;
        Vector2 elementSize = thisUIElement.rect.size;
        Vector2 targetPosition = CalculateTargetPosition(canvasSize, elementSize, thisUIElement.anchoredPosition);

        await thisUIElement.DOAnchorPos(targetPosition, 1f).AsyncWaitForCompletion();
    }

    public override async UniTask ShowAsync()
    {
        _isShown.Value = true;

        gameObject.SetActive(true);

        RectTransform thisRectTransform = (RectTransform)transform;
        thisRectTransform.DOAnchorPos(Vector2.zero, 1f);

        await UniTask.CompletedTask;
    }

    public override async UniTask CloseAsync()
    {
        _isShown.Value = false;

        await AnimateClose();
        gameObject.SetActive(false);
    }

    private void Start()
    {
        CommandsSetup();

        _closebutton.onClick.AddListener(() => _closeCommand.Execute(null));
    }
}
