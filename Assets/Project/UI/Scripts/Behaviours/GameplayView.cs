using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class GameplayView : ViewBehaviourBase
{
    // Reactive property representing whether the popup is currently shown
    private readonly ReactiveProperty<bool> _isShown = new ReactiveProperty<bool>(false);

    // Observable that emits true when popup is shown
    public IObservable<bool> OnShow => _isShown.Where(isShown => isShown);

    // Observable that emits true when popup is closed
    public IObservable<bool> OnClose => _isShown.Where(isShown => !isShown);

    [SerializeField]
    private ViewBehaviourBase _settingsPopup;
    [SerializeField]
    private Button _showSettingsPopupButton;

    #region Commands
    public ICommand ShowSettingPopupCommand => _ShowSettingPopupCommand;
    private ICommand _ShowSettingPopupCommand;

    private void OnShowSettingPopupCommand(object param)
    {
        _settingsPopup.ShowAsync().Forget();
    }

    private bool CanShowSettingPopupCommand(object param)
    {
        return true;
    }
    private void CommandsSetup()
    {
        _ShowSettingPopupCommand = new LambdaCommand(OnShowSettingPopupCommand, CanShowSettingPopupCommand);
    }
    #endregion

    public override async UniTask ShowAsync()
    {
        _isShown.Value = true;

        gameObject.SetActive(true);
        await UniTask.CompletedTask;
    }

    public override async UniTask CloseAsync()
    {
        _isShown.Value = false;

        gameObject.SetActive(false);
        await UniTask.CompletedTask;
    }

    private void Start()
    {
        CommandsSetup();

        _showSettingsPopupButton.onClick.AddListener(() => _ShowSettingPopupCommand.Execute(null));
    }
}
