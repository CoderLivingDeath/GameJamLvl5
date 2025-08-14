using Cysharp.Threading.Tasks;
using EditorAttributes;

public class GameplayUIService
{
    private GameplayUIViewsProvider _gameplayUiViewsProvider;

    public GameplayUIService(GameplayUIViewsProvider gameplayUiViewsProvider)
    {
        _gameplayUiViewsProvider = gameplayUiViewsProvider;
    }

    public async UniTask ShowJournalPopup(JournalPopupView.ShowContext context)
    {
        await _gameplayUiViewsProvider.JournalPopupView.ShowCommand.ExecuteAsync(context);
    }

    public async UniTask CloseJournalPopup()
    {
        await _gameplayUiViewsProvider.JournalPopupView.CloseCommand.ExecuteAsync(null);
    }

    public void AddEntryInJournalPopup(string entry)
    {
        _gameplayUiViewsProvider.JournalPopupView.AddJournalEntryCommand.Execute(entry);
    }

    public void ShowSettingsPopup()
    {
        _gameplayUiViewsProvider.SettingsPopupView.ShowCommand.Execute(null);
    }

    public void CloseSettingsPopup()
    {
        _gameplayUiViewsProvider.SettingsPopupView.CloseCommand.Execute(null);
    }

    public void SetupPrecepririonSelectionView(PerceptionSelectionView.SetupContext context)
    {
        _gameplayUiViewsProvider.PerceptionSelectionView.SetupCommand.Execute(context);
    }

    public PerceptionSelectionScope ShowPrecepririonSelectionView()
    {
        _gameplayUiViewsProvider.PerceptionSelectionView.ShowCommand.Execute(null);
        return new PerceptionSelectionScope(_gameplayUiViewsProvider.PerceptionSelectionView);
    }

    public void ClosePrecepririonSelectionView()
    {
        _gameplayUiViewsProvider.PerceptionSelectionView.CloseCommand.Execute(null);
    }

    public void ShowDebug()
    {
        _gameplayUiViewsProvider.DebugView.ShowCommand.Execute(null);
    }

    public void CloseDebug()
    {
        _gameplayUiViewsProvider.DebugView.CloseCommand.Execute(null);
    }

    public async UniTask ShowGameOverView()
    {
        await _gameplayUiViewsProvider.GameOverView.ShowCommand.ExecuteAsync(null);
    }

    public async UniTask CloseGameOverView()
    {
        await _gameplayUiViewsProvider.GameOverView.CloseCommand.ExecuteAsync(null);
    }
}

public class PerceptionSelectionScope
{
    private PerceptionSelectionView _view;

    public PerceptionSelectionScope(PerceptionSelectionView view)
    {
        _view = view;
    }

    public async UniTask<string> AwaitForSelect()
    {
        return await _view.AwaitForSelect();
    }
}