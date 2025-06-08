using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TypingEffect : MonoBehaviour
{
    [Header("Typing Settings")]
    public float Delay = 0.1f;
    public float FontScaleStart = 2f;
    public float FontScaleEnd = 1f;
    public float FontSizeDuration = 0.3f;

    [Header("Color Gradient")]
    public Gradient Gradient;

    [Header("Critical Typing Settings")]
    public float CriticalDelay = 0.12f;
    public float CriticalFontScaleStart = 4f;
    public float CriticalFontScaleEnd = 2f;
    public float CriticalFontSizeDuration = 0.4f;

    [Header("Critical Color Gradient")]
    public Gradient CriticalGradient;

    private TextMeshProUGUI _textUI;
    private Coroutine _typingCoroutine;

    private void Awake()
    {
        _textUI = GetComponent<TextMeshProUGUI>();
    }

    public void Typing(string fullText)
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            DOTween.Kill(this);
        }

        _textUI.text = "";
        _typingCoroutine = StartCoroutine(ShowTextWithEffect(fullText));
    }

    public void TypingCritical(string fullText)
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            DOTween.Kill(this);
        }

        _textUI.text = "";
        _typingCoroutine = StartCoroutine(ShowTextWithCriticalEffect(fullText));
    }

    private IEnumerator ShowTextWithEffect(string fullText)
    {
        int totalLength = fullText.Length;

        for (int i = 0; i < totalLength; i++)
        {
            char c = fullText[i];
            _textUI.text += c;

            float t = totalLength > 1 ? i / (float)(totalLength - 1) : 0f;
            Color charColor = Gradient.Evaluate(t);

            _textUI.color = Color.white;
            _textUI.transform.localScale = Vector3.one * FontScaleStart;

            _textUI.DOColor(charColor, FontSizeDuration)
                .SetEase(Ease.OutQuad)
                .SetTarget(this);

            _textUI.transform.DOScale(Vector3.one * FontScaleEnd, FontSizeDuration)
                .SetEase(Ease.OutQuad)
                .SetTarget(this);

            yield return new WaitForSeconds(Delay);
        }

        _typingCoroutine = null;
    }

    private IEnumerator ShowTextWithCriticalEffect(string fullText)
    {
        int totalLength = fullText.Length;

        for (int i = 0; i < totalLength; i++)
        {
            char c = fullText[i];
            _textUI.text += c;

            float t = totalLength > 1 ? i / (float)(totalLength - 1) : 0f;
            Color charColor = CriticalGradient.Evaluate(t);

            _textUI.color = Color.white;
            _textUI.transform.localScale = Vector3.one * CriticalFontScaleStart;

            _textUI.DOColor(charColor, CriticalFontSizeDuration)
                .SetEase(Ease.OutQuad)
                .SetTarget(this);

            _textUI.transform.DOScale(Vector3.one * CriticalFontScaleEnd, CriticalFontSizeDuration)
                .SetEase(Ease.OutQuad)
                .SetTarget(this);

            yield return new WaitForSeconds(CriticalDelay);
        }

        _typingCoroutine = null;
    }
}
