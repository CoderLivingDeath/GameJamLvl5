using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Zenject;

public class GameplayView : MonoBehaviour
{

    [Inject]
    private SettingsPopupView _settingsPopup;

    [Inject]
    private JournalPopupView _journalPopup;

    [SerializeField]
    private Button _showSettingsPopupButton;

    [SerializeField]
    private Button _showJournalPopupButton;

    #region Commands

    public ICommand ShowJournalPopupCommand => _ShowJournalPopupCommand;
    private ICommand _ShowJournalPopupCommand;

    private void OnShowJournalPopupCommand(object param)
    {
        _journalPopup.ShowCommand.Execute(null);
    }

    private bool CanShowJournalPopupCommand(object param)
    {
        return true;
    }

    public ICommand ShowSettingPopupCommand => _ShowSettingPopupCommand;
    private ICommand _ShowSettingPopupCommand;

    private void OnShowSettingPopupCommand(object param)
    {
        _settingsPopup.ShowCommand.Execute(null);
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

    [Inject]
    private GameplayUIService _gameplayUIService;

    [Inject]
    private void Construct()
    {
        CommandsSetup();
    }

    private void OnEnable()
    {
        _showSettingsPopupButton.onClick.AddListener(() => _gameplayUIService.ShowSettingsPopup());
        _showJournalPopupButton.onClick.AddListener(() => _gameplayUIService.ShowJournalPopup(new JournalPopupView.ShowContext(false)));
    }

    private void OnDisable()
    {

    }
}
