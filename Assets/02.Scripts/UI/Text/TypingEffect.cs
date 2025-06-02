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

        // 타이핑 시작 전에 투명도 1로 복원
        textUI.alpha = 1f;

        for (int i = 0; i < totalLength; i++)
        {
            char c = fullText[i];
            textUI.text += c;

            float t = totalLength > 1 ? i / (float)(totalLength - 1) : 0f;
            Color charColor = gradient.Evaluate(t);

            textUI.color = Color.white;
            textUI.transform.localScale = Vector3.one * fontScaleStart;

            textUI.DOColor(charColor, fontSizeDuration)
                .SetEase(Ease.OutQuad)
                .SetTarget(this);

            textUI.transform.DOScale(Vector3.one * fontScaleEnd, fontSizeDuration)
                .SetEase(Ease.OutQuad)
                .SetTarget(this);
            
            yield return new WaitForSeconds(delay);
        }
        
        // 타이핑 완료 후 텍스트 전체를 서서히 페이드아웃
        yield return new WaitForSeconds(0.5f); // 사라지기 전 잠깐 대기 (선택적)
        textUI.DOFade(0f, 1f)
            .SetEase(Ease.OutQuad)
            .SetTarget(this);

        typingCoroutine = null;
    }

}
