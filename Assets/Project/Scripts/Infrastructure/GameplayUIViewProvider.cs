using Zenject;

public class GameplayUIViewsProvider
{
    public SettingsPopupView SettingsPopupView => _settingsPopupView;
    [Inject] private SettingsPopupView _settingsPopupView;

    public JournalPopupView JournalPopupView => _journalPopupView;
    [Inject] private JournalPopupView _journalPopupView;

    public GameplayView GameplayView => _gameplayView;
    [Inject] private GameplayView _gameplayView;

    public PerceptionSelectionView PerceptionSelectionView => _perceptionSelectionView;

    [Inject] private PerceptionSelectionView _perceptionSelectionView;

    public DebugView DebugView => _debugView;
    [Inject] private DebugView _debugView;

    public GameplayUIViewsProvider()
    {

    }
}