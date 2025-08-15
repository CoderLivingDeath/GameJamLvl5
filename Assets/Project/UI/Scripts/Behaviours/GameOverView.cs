using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameOverView : MonoBehaviour
{

    public float ShowDuration = 1f;
    public Ease ShowEase = Ease.OutSine;
    public float CloseDuration = 1f;
    public Ease CloseEase = Ease.OutSine;

    [SerializeField]
    private Button _restartButton;
    [SerializeField]
    private Button _leaveButtpn;
    [SerializeField]
    private CanvasGroup _container;

    #region Commands

    public IAsyncCommand ShowCommand => _ShowCommand;
    private IAsyncCommand _ShowCommand;

    private async UniTask OnShowCommand(object param)
    {
        await ShowAsync();
    }

    public IAsyncCommand CloseCommand => _CloseCommand;
    private IAsyncCommand _CloseCommand;

    private async UniTask OnCloseCommand(object param)
    {
        await CloseAsync();
    }

    private void CommandSetup()
    {
        _ShowCommand = new LambdaAsyncCommand(OnShowCommand);
        _CloseCommand = new LambdaAsyncCommand(OnCloseCommand);

    }
    #endregion

    [Inject] private SceneManagerService _sceneManagerService;

    private async UniTask ShowAsync()
    {
        _container.alpha = 0;
        gameObject.SetActive(true);

        await _container.DOFade(1f, ShowDuration).SetEase(ShowEase).AsyncWaitForCompletion().AsUniTask();
    }

    private async UniTask CloseAsync()
    {
        _container.alpha = 1;

        await _container.DOFade(0f, ShowDuration).SetEase(ShowEase).AsyncWaitForCompletion().AsUniTask();
        await UniTask.WaitForSeconds(1);
        gameObject.SetActive(false);
    }

    [Inject]
    private void Construct()
    {
        CommandSetup();
        _leaveButtpn.onClick.AddListener(async () =>
        {
            await CloseCommand.ExecuteAsync(null);

            Application.Quit();
        });

        _restartButton.onClick.AddListener(async () =>
        {
            _sceneManagerService.RestartGameplayLevelAsync().Forget();
            await CloseCommand.ExecuteAsync(null);

            //TODO: реализовать перезагрузку сцены
            Debug.Log("не реализованно");
        });

        
    }
}
