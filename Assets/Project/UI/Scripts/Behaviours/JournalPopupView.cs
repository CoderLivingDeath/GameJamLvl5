using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class JournalPopupView : MonoBehaviour
{
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

    [SerializeField]
    private Button _closeButton;

    [SerializeField] private TMP_Text[] texts = new TMP_Text[7];

    public bool IsAnimating => _isAnimating;
    private bool _isAnimating;

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

    public ICommand ShowCommand => _showCommand;
    private ICommand _showCommand;

    private void OnShowCommand(object param)
    {
        var value = (ShowContext)param;

        ShowAsync(value).Forget();
    }

    public ICommand CloseCommand => _closeCommand;
    private ICommand _closeCommand;

    private void OnCloseCommand(object param)
    {
        CloseAsync().Forget();
    }

    private void CommandsSetup()
    {
        _closeCommand = new LambdaCommand(OnCloseCommand);
        _showCommand = new LambdaCommand(OnShowCommand);
        _AddJournalEntryCommand = new LambdaCommand(OnAddJournalEntryCommand);
    }

    #endregion

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
    }

    private async UniTask ShowAsync(ShowContext context)
    {
        if (_isAnimating) return;
        _isAnimating = true;

        JournalPopupViewShowAnimation animation = new(ShowDuration, ShowEase, context.DarkBackground);
        gameObject.SetActive(true);
        await animation.RunAsync(this);

        _isAnimating = false;
    }

    private async UniTask CloseAsync()
    {
        if (_isAnimating) return;
        _isAnimating = true;

        JournalPopupViewCloseAnimation animation = new(CloseDuration, CloseEase);
        await animation.RunAsync(this);
        gameObject.SetActive(false);
        
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
        _closeButton.onClick.AddListener(() => _closeCommand.Execute(null));
    }

    private void OnDestroy()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveAllListeners();
        }

        _disposables.Dispose();
    }
}
