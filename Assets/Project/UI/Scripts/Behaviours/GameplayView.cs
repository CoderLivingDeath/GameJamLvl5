using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class GameplayView : ViewBehaviourBase
{

    [SerializeField]
    private ViewBehaviourBase _settingsPopup;

    [SerializeField]
    private ViewBehaviourBase _journalPopup;

    [SerializeField]
    private Button _showSettingsPopupButton;

    [SerializeField]
    private Button _showJournalPopupButton;

    #region Commands


    public ICommand ShowJournalPopupCommand => _ShowJournalPopupCommand;
    private ICommand _ShowJournalPopupCommand;

    private void OnShowJournalPopupCommand(object param)
    {
        _journalPopup.ShowAsync().Forget();
    }

    private bool CanShowJournalPopupCommand(object param)
    {
        return true;
    }
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
        _ShowJournalPopupCommand = new LambdaCommand(OnShowJournalPopupCommand, CanShowJournalPopupCommand);
    }
    #endregion

    public override async UniTask ShowAsync()
    {
        gameObject.SetActive(true);
        await UniTask.CompletedTask;
    }

    public override async UniTask CloseAsync()
    {
        gameObject.SetActive(false);
        await UniTask.CompletedTask;
    }

    private void Start()
    {
        CommandsSetup();

        _showSettingsPopupButton.onClick.AddListener(() => ShowSettingPopupCommand.Execute(null));
        _showJournalPopupButton.onClick.AddListener(() => ShowJournalPopupCommand.Execute(null));
    }
}
