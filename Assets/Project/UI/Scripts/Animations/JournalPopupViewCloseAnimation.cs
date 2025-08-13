using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class JournalPopupViewCloseAnimation : UIAnimation<JournalPopupView>
{
    public float Duration;
    public Ease Ease;

    public JournalPopupViewCloseAnimation(float duration, Ease ease)
    {
        Duration = duration;
        Ease = ease;
    }

    public override async UniTask RunAsync(JournalPopupView context)
    {
        RectTransform thisUIElement = (RectTransform)context.transform;

        Vector2 canvasSize = context.Canvas.rect.size;
        Vector2 elementSize = thisUIElement.rect.size;

        Vector2 targetPosition = CalculateTargetPosition(canvasSize, elementSize, thisUIElement.anchoredPosition);

        RectTransform journalRect = (RectTransform)context.JournalGroup.transform;
        journalRect.localScale = Vector2.one;

        Color bgcolor = context.Background.color;
        // context.Background.color = new Color(bgcolor.r, bgcolor.g, bgcolor.b, 1f);

        Tween scaleTween = journalRect.DOScale(Vector3.one * 0.9f, Duration)
            .SetEase(Ease)
            .SetUpdate(true);

        context.Background.DOColor(new Color(bgcolor.r, bgcolor.g, bgcolor.b, 0f), 1f).SetEase(Ease);

        await scaleTween.AsyncWaitForCompletion();

        Tween moveTween = thisUIElement.DOAnchorPos(targetPosition, 1f)
            .SetEase(Ease);

        await moveTween.AsyncWaitForCompletion();

        Vector2 CalculateTargetPosition(Vector2 canvasSize, Vector2 elementSize, Vector2 currentAnchoredPosition)
        {
            float targetY = -canvasSize.y / 2 - elementSize.y / 2;
            return new Vector2(currentAnchoredPosition.x, targetY);
        }
    }
}
