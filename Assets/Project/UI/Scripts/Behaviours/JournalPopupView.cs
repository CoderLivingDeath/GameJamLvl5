using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class JournalPopupView : ViewBehaviourBase
{
    [SerializeField]
    private RectTransform _canvas;

    [SerializeField]
    private Button _closeButton;

    #region Commands

    public ICommand ShowCommand => _ShowCommand;
    private ICommand _ShowCommand;

    private void OnShowCommand(object param)
    {
        ShowAsync().Forget();
    }

    private bool CanShowCommand(object param)
    {
        return true;
    }

    public ICommand CloseCommand => _closeCommand;
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
        _ShowCommand = new LambdaCommand(OnShowCommand, CanShowCommand);
    }

    #endregion

    private Vector2 CalculateTargetPosition(Vector2 canvasSize, Vector2 elementSize, Vector2 currentAnchoredPosition)
    {
        float targetX = (canvasSize.x / 2) + (elementSize.x / 2);
        return new Vector2(targetX, currentAnchoredPosition.y);
    }

    private async UniTask AnimateClose()
    {
        if (_canvas == null)
        {
            Debug.LogError("Canvas is not assigned in JournalPopupView!");
            return;
        }

        RectTransform thisUIElement = (RectTransform)transform;
        Vector2 canvasSize = _canvas.rect.size;
        Vector2 elementSize = thisUIElement.rect.size;
        Vector2 targetPosition = CalculateTargetPosition(canvasSize, elementSize, thisUIElement.anchoredPosition);

        await thisUIElement.DOAnchorPos(targetPosition, 1f).AsyncWaitForCompletion();
    }

    public override async UniTask ShowAsync()
    {
        gameObject.SetActive(true);

        RectTransform thisRectTransform = (RectTransform)transform;
        await thisRectTransform.DOAnchorPos(Vector2.zero, 1f).AsyncWaitForCompletion();
    }

    public override async UniTask CloseAsync()
    {
        await AnimateClose();
        gameObject.SetActive(false);
    }

    private void Start()
    {
        if (_closeButton == null)
        {
            Debug.LogError("CloseButton is not assigned in JournalPopupView!");
            return;
        }

        CommandsSetup();
        _closeButton.onClick.AddListener(() => _closeCommand.Execute(null));
    }

    private void OnDestroy()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveAllListeners();
        }
    }
}