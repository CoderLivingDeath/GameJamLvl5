using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DebugView : MonoBehaviour
{

    [SerializeField]
    private Button _closebutton;

    [SerializeField]
    private Button _showPrecepririonSelectionButton;

    [SerializeField]
    private Button _closePrecepririonSelectionButton;

    [SerializeField]
    private Button _applyRootDataButton;

    [SerializeField]
    private TMP_Text _currentRootText;

    [SerializeField]
    private TMP_Dropdown _rootDropdown;

    [SerializeField]
    private TMP_InputField _journalEntryField;

    [SerializeField]
    private Button _addJournalEntryButton;

    #region Commands

    public ICommand CloseCommand => _closeCommand;
    private ICommand _closeCommand;

    private void OnCloseCommand(object param)
    {
        Close();
    }

    private bool CanCloseCommand(object param)
    {
        return true;
    }

    public ICommand ShowCommand => _ShowCommand;
    private ICommand _ShowCommand;

    private void OnShowCommand(object param)
    {
        Show();
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
    private GameplayUIService _gameplayUIService;

    [Inject]
    private DataService _dataService;

    [Inject]
    private void Construct()
    {
        CommandsSetup();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        _closebutton.onClick.AddListener(() => _closeCommand.Execute(null));
        _showPrecepririonSelectionButton.onClick.AddListener(() => _gameplayUIService.ShowPrecepririonSelectionView());
        _closePrecepririonSelectionButton.onClick.AddListener(() => _gameplayUIService.ClosePrecepririonSelectionView());

        _addJournalEntryButton.onClick.AddListener(() => _gameplayUIService.AddEntryInJournalPopup(_journalEntryField.text));
    }

    void OnDisable()
    {

    }
}
