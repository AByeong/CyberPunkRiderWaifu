using DG.Tweening;
using UnityEngine;
public class UIPopupAnimator : MonoBehaviour
{
    public enum PopupEffectType
    {
        Scale,
        Fade,
        Slide,
        ScaleAndFade
    }

    [Header("애니메이션 타입 선택")]
    public PopupEffectType effectType = PopupEffectType.Scale;

    [Header("테스트용 토글 (창 열기)")]
    public bool testOpen;

    [Header("설정")]
    public float duration = 0.3f;
    public Ease ease = Ease.OutBack;

    CanvasGroup canvasGroup;
    RectTransform rectTransform;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void OnEnable()
    {
        PlayAnimation();
    }

    void Update()
    {
        // 에디터에서 테스트하기 쉽게
        if (testOpen)
        {
            testOpen = false;
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }

    public void PlayAnimation()
    {
        // 초기화
        transform.localScale = Vector3.one;
        canvasGroup.alpha = 1;
        rectTransform.anchoredPosition = Vector2.zero;

        switch (effectType)
        {
        case PopupEffectType.Scale:
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, duration).SetEase(ease).SetUpdate(true);
            break;

        case PopupEffectType.Fade:
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, duration).SetEase(Ease.Linear).SetUpdate(true);
            break;

        case PopupEffectType.Slide:
            rectTransform.anchoredPosition = new Vector2(0, -Screen.height);
            rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutCubic).SetUpdate(true);
            break;

        case PopupEffectType.ScaleAndFade:
            transform.localScale = Vector3.zero;
            canvasGroup.alpha = 0;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(Vector3.one, duration).SetEase(ease)).SetUpdate(true);
            seq.Join(canvasGroup.DOFade(1, duration * 0.8f)).SetUpdate(true);
            break;
        }
    }
}
