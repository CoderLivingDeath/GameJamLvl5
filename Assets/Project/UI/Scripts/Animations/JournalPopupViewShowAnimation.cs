using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class JournalPopupViewShowAnimation : UIAnimation<JournalPopupView>
{
    public float Duration;
    public Ease Ease;

    public bool DarkBackground;

    public JournalPopupViewShowAnimation(float duration, Ease ease, bool darkBackground)
    {
        Duration = duration;
        Ease = ease;
        DarkBackground = darkBackground;
    }

    public override async UniTask RunAsync(JournalPopupView context)
    {
        RectTransform thisUIElement = (RectTransform)context.transform;
        Vector2 canvasSize = context.Canvas.rect.size;
        Vector2 elementSize = thisUIElement.rect.size;
        Vector2 startPosition = CalculateTargetPosition(canvasSize, elementSize, thisUIElement.anchoredPosition);

        thisUIElement.anchoredPosition = startPosition;
        RectTransform journalRect = (RectTransform)context.JournalGroup.transform;
        journalRect.localScale = Vector2.one * 0.9f;

        Color bgcolor = context.Background.color;
        context.Background.color = new Color(bgcolor.r, bgcolor.g, bgcolor.b, 0f);

        Tween moveTween = thisUIElement.DOAnchorPos(Vector2.zero, Duration)
            .SetEase(Ease);
        if (DarkBackground)
        {
            context.Background.DOColor(new Color(bgcolor.r, bgcolor.g, bgcolor.b, 1f), 1f).SetEase(Ease);
        }

        await moveTween.AsyncWaitForCompletion().AsUniTask();


        Tween scaleTween = journalRect.DOScale(Vector3.one, 0.3f)
            .SetEase(Ease);

        await scaleTween.AsyncWaitForCompletion().AsUniTask();

        Vector2 CalculateTargetPosition(Vector2 canvasSize, Vector2 elementSize, Vector2 currentAnchoredPosition)
        {
            float targetY = -canvasSize.y / 2 - elementSize.y / 2;
            return new Vector2(currentAnchoredPosition.x, targetY);
        }
    }
}