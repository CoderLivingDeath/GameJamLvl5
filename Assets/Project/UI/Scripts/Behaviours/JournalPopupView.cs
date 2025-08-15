using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameJamLvl5.Project.Infrastructure.EventBus;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class JournalPopupView : MonoBehaviour
{
    #region stickers

    public GameObject Sticker_doc;

    public GameObject Sticker_cult;

    public GameObject Sticker_island;
    #endregion

    public ReactiveCollection<string> Entries = new();

    public Ease ShowEase = Ease.OutCubic;
    public float ShowDuration = 0.3f;

    public Ease CloseEase = Ease.OutCubic;
    public float CloseDuration = 0.3f;

    public RectTransform Canvas => _canvas;
    [SerializeField]
    private RectTransform _canvas;

    public CanvasGroup JournalGroup => _journalGroup;
    [SerializeField]
    private CanvasGroup _journalGroup;

    public Image Background => _background;
    [SerializeField]
    private Image _background;

    [SerializeField]
    private Button _closeButton;

    [SerializeField] private TMP_Text[] texts = new TMP_Text[7];

    public bool IsAnimating => _isAnimating;
    private bool _isAnimating;

    private UniTaskCompletionSource _closeTcs;
    private UniTaskCompletionSource _showTcs;

    private CompositeDisposable _disposables = new CompositeDisposable();

    #region Commands

    public readonly struct ShowContext
    {
        public readonly bool DarkBackground;

        public ShowContext(bool darkBackground)
        {
            DarkBackground = darkBackground;
        }
    }

    public ICommand AddJournalEntryCommand => _AddJournalEntryCommand;
    private ICommand _AddJournalEntryCommand;

    private void OnAddJournalEntryCommand(object param)
    {
        string value = param as string;

        Entries.Add(value);
    }

    public IAsyncCommand ShowCommand => _showCommand;
    private IAsyncCommand _showCommand;

    private async UniTask OnShowCommand(object param)
    {
        var value = (ShowContext)param;

        await ShowAsync(value);
    }

    public IAsyncCommand CloseCommand => _closeCommand;
    private IAsyncCommand _closeCommand;

    private async UniTask OnCloseCommand(object param)
    {
        await CloseAsync();
    }

    private void CommandsSetup()
    {
        _closeCommand = new LambdaAsyncCommand(OnCloseCommand);
        _showCommand = new LambdaAsyncCommand(OnShowCommand);
        _AddJournalEntryCommand = new LambdaCommand(OnAddJournalEntryCommand);
    }

    #endregion

    [Inject]
    private EventBus _eventBus;

    [Inject]
    private void Construct()
    {
        CommandsSetup();

        // Подписка на добавление элементов
        Entries.ObserveAdd()
            .Subscribe(addEvent => AddText(addEvent.Value))
            .AddTo(_disposables);

        // Подписка на удаление элементов
        Entries.ObserveRemove()
            .Subscribe(removeEvent => Debug.Log("Удалён элемент: " + removeEvent.Value))
            .AddTo(_disposables);

        // Подписка на замену элементов
        Entries.ObserveReplace()
            .Subscribe(replaceEvent =>
            {
                Debug.Log($"Заменён элемент: старый={replaceEvent.OldValue}, новый={replaceEvent.NewValue}");
            })
            .AddTo(_disposables);


        ShowContext context = new(false);
        ShowAsyncWithDelay(context).Forget();
    }

    private async UniTask ShowAsync(ShowContext context)
    {
        if (_isAnimating) return;
        _isAnimating = true;

        JournalPopupViewShowAnimation animation = new(ShowDuration, ShowEase, context.DarkBackground);
        gameObject.SetActive(true);
        await animation.RunAsync(this);
        _showTcs?.TrySetResult();
        _isAnimating = false;
    }

    private async UniTask CloseAsync()
    {
        if (_isAnimating) return;
        _isAnimating = true;

        JournalPopupViewCloseAnimation animation = new(CloseDuration, CloseEase);
        await animation.RunAsync(this);
        gameObject.SetActive(false);

        _closeTcs?.TrySetResult();
        _closeTcs = null;

        _isAnimating = false;
    }

    public async UniTask BloackCloseFor(float time)
    {
        _closeButton.enabled = false;
        await UniTask.WaitForSeconds(time);
        _closeButton.enabled = true;
    }

    /// <summary>
    /// Ожидать, пока окно не будет закрыто пользователем.
    /// Повторный вызов закрывает только по следующему закрытию окна.
    /// </summary>
    public UniTask AwaitClose()
    {
        _closeTcs = new UniTaskCompletionSource();
        return _closeTcs.Task;
    }

    public UniTask AwaitShow()
    {
        _showTcs = new UniTaskCompletionSource();
        return _showTcs.Task;
    }

    private async UniTask ShowAsyncWithDelay(ShowContext context)
    {
        await UniTask.WaitForSeconds(2f);
        if (_isAnimating) return;
        _isAnimating = true;

        JournalPopupViewShowAnimation animation = new(ShowDuration, ShowEase, context.DarkBackground);
        gameObject.SetActive(true);
        await animation.RunAsync(this);
        _showTcs?.TrySetResult();
        _isAnimating = false;
    }

    private void AddText(string text)
    {
        if (Entries.Count > 7)
        {
            Debug.LogError("ДА ЁБАННЫЙ ТЫ БЛЯТЬ");
            return;
        }

        texts[Entries.Count - 1].text = text;
    }

    private void Start()
    {
        if (_closeButton == null)
        {
            Debug.LogError("CloseButton is not assigned in JournalPopupView!");
            return;
        }
    }

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(() => _closeCommand.ExecuteAsync(null));
    }

    private void OnDestroy()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveAllListeners();
        }

        _disposables.Dispose();
    }

    public void HandleEscape(bool button)
    {
        CloseCommand.ExecuteAsync(null).Forget();
    }
}
