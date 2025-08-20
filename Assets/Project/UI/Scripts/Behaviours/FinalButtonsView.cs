using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FinalButtonsView : MonoBehaviour
{
    public Button RestartButton;
    public Button QuitButton;

    public CanvasGroup Container;

    [Inject]
    private SceneManagerService sceneManagerService;

    private Tweener[] _restartQuitTweenersX;
    private Tweener[] _restartQuitTweenersY;

    private void AnimateRestartQuitButtons()
    {
        // Останавливаем текущие анимации, если они есть
        if (_restartQuitTweenersX != null)
            foreach (var t in _restartQuitTweenersX) t.Kill();

        if (_restartQuitTweenersY != null)
            foreach (var t in _restartQuitTweenersY) t.Kill();

        var buttons = new RectTransform[]
        {
        RestartButton.transform as RectTransform,
        QuitButton.transform as RectTransform
        };

        _restartQuitTweenersX = new Tweener[buttons.Length];
        _restartQuitTweenersY = new Tweener[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            var rect = buttons[i];
            if (rect == null) continue;

            Vector2 origPos = rect.anchoredPosition;

            float xMove = Random.Range(10f, 25f);
            float yMove = Random.Range(5f, 15f);
            float duration = Random.Range(1.2f, 2.5f);
            float delay = Random.Range(0f, 1.5f);

            _restartQuitTweenersX[i] = rect.DOAnchorPosX(origPos.x + xMove, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetDelay(delay);

            _restartQuitTweenersY[i] = rect.DOAnchorPosY(origPos.y + yMove, duration * 1.25f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetDelay(delay + duration * 0.5f);
        }
    }

    void Start()
    {
        RestartButton.onClick.AddListener(() => sceneManagerService.RestartGameplayLevelAsync().Forget());
        QuitButton.onClick.AddListener(() => Application.Quit());
        
        AnimateRestartQuitButtons();
    }

    public async UniTask AnimateFade(float delay)
    {
        await UniTask.WaitForSeconds(delay);
        await Container.DOFade(1, 1f).AsyncWaitForCompletion().AsUniTask();
    }

    void OnEnable()
    {
        Container.alpha = 0;
        AnimateFade(3).Forget();

    }
}
