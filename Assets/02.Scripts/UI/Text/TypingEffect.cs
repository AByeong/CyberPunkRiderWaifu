using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TextMeshProUGUI))]
public class TypingEffect : MonoBehaviour
{
    [Header("Typing Settings")]
    public float delay = 0.05f;
    public float fontScaleStart = 1.5f;
    public float fontScaleEnd = 1f;
    public float fontSizeDuration = 0.3f;

    [Header("Color Gradient")]
    public Gradient gradient;

    private TextMeshProUGUI textUI;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        textUI = GetComponent<TextMeshProUGUI>();
    }

    public void Typing(string fullText)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            DOTween.Kill(this); // 모든 tween 제거
        }

        textUI.text = ""; // 초기화

        typingCoroutine = StartCoroutine(ShowTextWithEffect(fullText));
    }

    private IEnumerator ShowTextWithEffect(string fullText)
    {
        int totalLength = fullText.Length;

        for (int i = 0; i < totalLength; i++)
        {
            char c = fullText[i];
            textUI.text += c;

            // 문자 위치 비율
            float t = totalLength > 1 ? i / (float)(totalLength - 1) : 0f;
            Color charColor = gradient.Evaluate(t);

            // 처음엔 흰색
            textUI.color = Color.white;
            textUI.transform.localScale = Vector3.one * fontScaleStart;

            // 색상 애니메이션: 흰색 → gradient color
            textUI.DOColor(charColor, fontSizeDuration)
                .SetEase(Ease.OutQuad)
                .SetTarget(this);

            // 스케일 애니메이션
            textUI.transform.DOScale(Vector3.one * fontScaleEnd, fontSizeDuration)
                .SetEase(Ease.OutQuad)
                .SetTarget(this);

            yield return new WaitForSeconds(delay);
        }

        typingCoroutine = null;
    }
}
