using System.Collections;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TextMeshProUGUI))]
public class TypingEffect2 : MonoBehaviour
{
    public string fullText = "Hello, World!";
    public float delay = 0.1f;
    public float fontSizeStart = 48f;
    public float fontSizeEnd = 32f;
    public float fontSizeDuration = 0.3f;
    public Color targetColor = Color.cyan; // 최종 색상 (불투명도 포함)

    private TextMeshProUGUI textUI;
    private readonly StringBuilder finalText = new StringBuilder();

    private void Start()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        StartCoroutine(ShowTextWithEffect());
    }

    private IEnumerator ShowTextWithEffect()
    {
        textUI.text = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            char c = fullText[i];

            float size = fontSizeStart;
            Color currentColor = new Color(1, 1, 1, 0); // 시작 투명도 0

            // 임시 출력
            UpdateText(c, size, currentColor);

            // 폰트 크기 애니메이션
            DOTween.To(() => size, x => {
                size = x;
                UpdateText(c, size, currentColor);
            }, fontSizeEnd, fontSizeDuration).SetEase(Ease.OutQuad);

            // 색상 및 알파 애니메이션
            DOTween.To(() => currentColor, x => {
                currentColor = x;
                UpdateText(c, size, currentColor);
            }, new Color(targetColor.r, targetColor.g, targetColor.b, 1f), fontSizeDuration).SetEase(Ease.OutQuad);

            yield return new WaitForSeconds(delay);

            // 최종 정착
            string colorHex = ColorUtility.ToHtmlStringRGBA(new Color(targetColor.r, targetColor.g, targetColor.b, 1f));
            finalText.Append($"<size={fontSizeEnd}><color=#{colorHex}>{c}</color></size>");
        }

        textUI.text = finalText.ToString();
    }

    private void UpdateText(char currentChar, float size, Color color)
    {
        string colorHex = ColorUtility.ToHtmlStringRGBA(color);
        textUI.text = finalText + $"<size={(int)size}><color=#{colorHex}>{currentChar}</color></size>";
    }
}
