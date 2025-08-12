using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SettingsPopupView : MonoBehaviour
{
    [SerializeField]
    private RectTransform _canvas;

    [SerializeField]
    private Button _closebutton;

    private bool _isAnimating;

    private Ease _ease = Ease.OutCubic;

    #region Commands

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

    private void CommandsSetup()
    {
        _closeCommand = new LambdaCommand(OnCloseCommand, CanCloseCommand);
        _ShowCommand = new LambdaCommand(OnShowCommand, CanShowCommand);
    }
    #endregion

    [Inject]
    private void Construct()
    {
        CommandsSetup();
    }

    private Vector2 CalculateTargetPosition(Vector2 canvasSize, Vector2 elementSize, Vector2 currentAnchoredPosition)
    {
        float targetX = (canvasSize.x / 2) + (elementSize.x / 2);
        return new Vector2(targetX, currentAnchoredPosition.y);
    }

    private async UniTask AnimateClose()
    {
        if (_isAnimating || _canvas == null)
        {
            if (_canvas == null) Debug.LogError("Canvas is not assigned in JournalPopupView!");
            return;
        }

        _isAnimating = true;

        try
        {
            RectTransform thisUIElement = (RectTransform)transform;
            Vector2 canvasSize = _canvas.rect.size;
            Vector2 elementSize = thisUIElement.rect.size;
            Vector2 targetPosition = CalculateTargetPosition(canvasSize, elementSize, thisUIElement.anchoredPosition);

            var moveTween = thisUIElement.DOAnchorPos(targetPosition, 1f).SetEase(_ease);

            await moveTween.AsyncWaitForCompletion();
        }
        finally
        {
            _isAnimating = false;
        }
    }

    private async UniTask AnimateOpen()
    {
        if (_isAnimating || _canvas == null)
        {
            if (_canvas == null) Debug.LogError("Canvas is not assigned in JournalPopupView!");
            return;
        }

        _isAnimating = true;

        try
        {
            RectTransform thisUIElement = (RectTransform)transform;
            Vector2 canvasSize = _canvas.rect.size;
            Vector2 elementSize = thisUIElement.rect.size;
            Vector2 startPosition = CalculateTargetPosition(canvasSize, elementSize, thisUIElement.anchoredPosition);

            thisUIElement.anchoredPosition = startPosition;
            var moveTween = thisUIElement.DOAnchorPos(Vector2.zero, 1f).SetEase(_ease);

            await moveTween.AsyncWaitForCompletion();
        }
        finally
        {
            _isAnimating = false;
        }
    }

    private async UniTask ShowAsync()
    {
        if (_isAnimating) return;

        gameObject.SetActive(true);
        await AnimateOpen();
    }

    private async UniTask CloseAsync()
    {
        if (_isAnimating) return;

        await AnimateClose();
        gameObject.SetActive(false);
    }


    private void Awake()
    {

        _closebutton.onClick.AddListener(() => _closeCommand.Execute(null));
    }
}
