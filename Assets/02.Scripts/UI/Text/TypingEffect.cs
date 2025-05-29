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
    public Color targetColor = Color.cyan;

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
        textUI.color = new Color(targetColor.r, targetColor.g, targetColor.b, 1f); // 기본 색

        typingCoroutine = StartCoroutine(ShowTextWithEffect(fullText));
    }

    private IEnumerator ShowTextWithEffect(string fullText)
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            char c = fullText[i];
            textUI.text += c;

            // TMP 텍스트 전체가 아닌 개별 문자에 애니메이션 적용하려면 → 어렵고 TextMeshPro TextInfo를 써야 함
            // 대신, 전체 텍스트에 시각적 효과 적용 (scale + fade)로 표현

            textUI.transform.localScale = Vector3.one * fontScaleStart;
            textUI.color = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);

            textUI.transform.DOScale(Vector3.one * fontScaleEnd, fontSizeDuration)
                .SetEase(Ease.OutQuad)
                .SetTarget(this);

            textUI.DOFade(1f, fontSizeDuration)
                .SetEase(Ease.OutQuad)
                .SetTarget(this);

            yield return new WaitForSeconds(delay);
        }

        typingCoroutine = null;
    }
}
