using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

public class PerceptionSelectionView : MonoBehaviour
{
    public CanvasGroup CanvasGroup => _canvasGroup;
    [SerializeField]
    private CanvasGroup _canvasGroup;

    public RawImage ItemImage => _itemImage;
    [SerializeField]
    private RawImage _itemImage;

    public Button SelectionOneButton => _SelectionOneButton;
    [SerializeField]
    private Button _SelectionOneButton;
    private string OneSelectId;

    public Button SelectionTwoButton => _SelectionTwoButton;
    [SerializeField]
    private Button _SelectionTwoButton;
    private string TwoSelectId;

    public Button SelectionThreeButton => _SelectionThreeButton;
    [SerializeField]
    private Button _SelectionThreeButton;
    private string ThreeSelectId;

    public Button SelectionFourButton => _SelectionFourButton;
    [SerializeField]
    private Button _SelectionFourButton;
    private string FourSelectId;

    public bool IsAnimating => _isAnimating;
    private bool _isAnimating;

    #region Commands

    [Serializable]
    public readonly struct SetupContext
    {
        public readonly Sprite Sprite;

        public readonly string SelectionOneText;
        public readonly string SelectionTwoText;
        public readonly string SelectionThreeText;
        public readonly string SelectionFourText;

        public readonly string OneSelectId;
        public readonly string TwoSelectId;
        public readonly string ThreeSelectId;
        public readonly string FourSelectId;

        public SetupContext(Sprite sprite, string selectionOneText, string selectionTwoText, string selectionThreeText, string selectionFourText, string oneSelectId, string twoSelectId, string threeSelectId, string fourSelectId)
        {
            Sprite = sprite;
            SelectionOneText = selectionOneText;
            SelectionTwoText = selectionTwoText;
            SelectionThreeText = selectionThreeText;
            SelectionFourText = selectionFourText;
            OneSelectId = oneSelectId;
            TwoSelectId = twoSelectId;
            ThreeSelectId = threeSelectId;
            FourSelectId = fourSelectId;
        }
    }

    public ICommand SetupCommand => _SetupCommand;
    private ICommand _SetupCommand;

    private void OnSetupCommand(SetupContext param)
    {
        if (_SelectionOneButton == null || _SelectionTwoButton == null ||
            _SelectionThreeButton == null || _SelectionFourButton == null)
        {
            Debug.LogError("OnSetupCommand: Одна или несколько ссылок на кнопки не инициализированы");
            return;
        }

        OneSelectId = param.OneSelectId;
        TwoSelectId = param.TwoSelectId;
        ThreeSelectId = param.ThreeSelectId;
        FourSelectId = param.FourSelectId;

        // Обновляем текст кнопок (находим TMP_Text в дочерних объектах кнопок)
        SetButtonText(_SelectionOneButton, param.SelectionOneText);
        SetButtonText(_SelectionTwoButton, param.SelectionTwoText);
        SetButtonText(_SelectionThreeButton, param.SelectionThreeText);
        SetButtonText(_SelectionFourButton, param.SelectionFourText);

        if (param.Sprite == null) return;
        // Обновляем текстуру
        _itemImage.texture = param.Sprite.texture;
        _itemImage.SetNativeSize();

        RectTransform rt = _itemImage.rectTransform;
        RectTransform parentRect = rt.parent as RectTransform;
        if (parentRect != null)
        {
            // Устанавливаем anchor и pivot в центр (если нужно)
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            // Сбрасываем позицию к центру родителя
            rt.anchoredPosition = Vector2.zero;
        }

        // Высота родительского элемента
        float parentHeight = parentRect.rect.height;

        // Размеры спрайта в пикселях
        float spriteWidth = param.Sprite.rect.width;
        float spriteHeight = param.Sprite.rect.height;

        // Расчёт ширины с сохранением пропорций:
        float width = parentHeight * (spriteWidth / spriteHeight);
        float height = parentHeight;

        // Применяем размеры к дочернему элементу
        rt.sizeDelta = new Vector2(width, height);
    }

    private void SetButtonText(Button button, string text)
    {
        TMP_Text tmpText = button.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = text ?? string.Empty;
        }
        else
        {
            Debug.LogError($"SetButtonText: В кнопке '{button.name}' отсутствует компонент TMP_Text");
        }
    }


    public ICommand SetItemImage => _setItemImage;
    private ICommand _setItemImage;

    private void OnSetItemImage(Texture param)
    {
        if (param == null)
        {
            Debug.LogError("Texture is null in OnSetItemImage!");
            return;
        }

        _itemImage.texture = param;
    }

    public ICommand CloseCommand => _closeCommand;
    private ICommand _closeCommand;

    private void OnCloseCommand(object param)
    {
        CloseAsync().Forget();
    }

    public ICommand ShowCommand => _showCommand;
    private ICommand _showCommand;

    private void OnShowCommand(object param)
    {
        ShowAsync().Forget();
    }

    private void CommandsSetup()
    {
        _setItemImage = new LambdaCommand((param) => OnSetItemImage(param as Texture));
        _closeCommand = new LambdaCommand(OnCloseCommand);
        _showCommand = new LambdaCommand(OnShowCommand);
        _SetupCommand = new LambdaCommand((param) => OnSetupCommand((SetupContext)param));
    }

    #endregion

    private async UniTask Show_FadeAnimation()
    {
        if (_isAnimating) return;
        _isAnimating = true;

        try
        {
            _canvasGroup.alpha = 0;
            await _canvasGroup.DOFade(1f, 1f).AsyncWaitForCompletion();
        }
        finally
        {
            _isAnimating = false;
        }
    }

    private async UniTask Close_FadeAnimation()
    {
        if (_isAnimating) return;
        _isAnimating = true;

        try
        {
            _canvasGroup.alpha = 1;
            await _canvasGroup.DOFade(0f, 1f).AsyncWaitForCompletion();
        }
        finally
        {
            _isAnimating = false;
        }
    }

    [Inject]
    private void Construct()
    {
        CommandsSetup();
    }

    public async UniTask<string> AwaitForSelect()
    {
        // Create a task completion source to handle the async wait
        var tcs = new UniTaskCompletionSource<string>();

        // Temporary handlers to capture button clicks and return button text
        void OnSelectionOne()
        {
            if (_isAnimating) return;
            tcs.TrySetResult(OneSelectId);
        }
        void OnSelectionTwo()
        {
            if (_isAnimating) return;
            tcs.TrySetResult(TwoSelectId);
        }
        void OnSelectionThree()
        {
            if (_isAnimating) return;
            tcs.TrySetResult(ThreeSelectId);
        }
        void OnSelectionFour()
        {
            if (_isAnimating) return;
            tcs.TrySetResult(FourSelectId);
        }

        try
        {
            // Add temporary listeners for button clicks
            _SelectionOneButton.onClick.AddListener(OnSelectionOne);
            _SelectionTwoButton.onClick.AddListener(OnSelectionTwo);
            _SelectionThreeButton.onClick.AddListener(OnSelectionThree);
            _SelectionFourButton.onClick.AddListener(OnSelectionFour);

            // Wait for one of the buttons to be clicked
            string result = await tcs.Task;

            return result;
        }
        finally
        {
            // Clean up listeners to prevent memory leaks
            _SelectionOneButton.onClick.RemoveListener(OnSelectionOne);
            _SelectionTwoButton.onClick.RemoveListener(OnSelectionTwo);
            _SelectionThreeButton.onClick.RemoveListener(OnSelectionThree);
            _SelectionFourButton.onClick.RemoveListener(OnSelectionFour);
        }
    }

    private async UniTask ShowAsync()
    {
        if (_isAnimating) return;

        this.gameObject.SetActive(true);
        await Show_FadeAnimation();
    }

    private async UniTask CloseAsync()
    {
        if (_isAnimating) return;

        await Close_FadeAnimation();
        this.gameObject.SetActive(false);
    }

    private Tweener[] _buttonTweenersX;
    private Tweener[] _buttonTweenersY;

    private void AnimateButtons()
    {
        // Если уже есть запущенные анимации — останавливаем
        if (_buttonTweenersX != null)
            foreach (var t in _buttonTweenersX) t.Kill();

        if (_buttonTweenersY != null)
            foreach (var t in _buttonTweenersY) t.Kill();

        var buttons = new RectTransform[]
        {
        _SelectionOneButton.transform as RectTransform,
        _SelectionTwoButton.transform as RectTransform,
        _SelectionThreeButton.transform as RectTransform,
        _SelectionFourButton.transform as RectTransform
        };

        _buttonTweenersX = new Tweener[buttons.Length];
        _buttonTweenersY = new Tweener[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            var rect = buttons[i];
            if (rect == null) continue;

            Vector2 origPos = rect.anchoredPosition;

            float xMove = Random.Range(10f, 25f);
            float yMove = Random.Range(5f, 15f);
            float duration = Random.Range(1.2f, 2.5f);
            float delay = Random.Range(0f, 1.5f);

            _buttonTweenersX[i] = rect.DOAnchorPosX(origPos.x + xMove, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetDelay(delay);

            _buttonTweenersY[i] = rect.DOAnchorPosY(origPos.y + yMove, duration * 1.25f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetDelay(delay + duration * 0.5f);
        }
    }

    private void OnEnable()
    {
        AnimateButtons();
    }

    private void OnDisable()
    {
        if (_buttonTweenersX != null)
            foreach (var t in _buttonTweenersX) t.Kill();

        if (_buttonTweenersY != null)
            foreach (var t in _buttonTweenersY) t.Kill();
    }
}