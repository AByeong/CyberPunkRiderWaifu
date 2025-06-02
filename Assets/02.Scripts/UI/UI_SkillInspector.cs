using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillInspector : Singleton<UI_SkillInspector>
{
    public GameObject InspectorPanel;
    public Image SkillIcon;
    public TextMeshProUGUI SkillNameText;
    public TextMeshProUGUI SkillDescriptionText;
    public TextMeshProUGUI SkillEffectText;
    private bool _isHovered = false;
    private Vector3 _originPosition;
    private RectTransform _canvasRectTransform;
    private RectTransform _inspectorPanelRectTransform;

    private void Start()
    {
        _inspectorPanelRectTransform = InspectorPanel.GetComponent<RectTransform>();
        _canvasRectTransform = InspectorPanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        _originPosition = transform.position;
    }
    public void Hovered(Skill skill)
    {
        SkillDescriptionText.text = skill.SkillData.SkillDescription;
        SkillNameText.text = skill.SkillData.SkillName;
        SkillEffectText.text = $"{skill.SkillData.SkillDamage}배\n\n";
        SkillEffectText.text += $"{Mathf.RoundToInt(skill.SkillData.SkillRange) * 100}%\n\n";
        SkillEffectText.text += $"{skill.SkillData.CoolTime:F2}초";
        SkillIcon.sprite = skill.SkillData.Icon;
        _isHovered = true;
        UpdateInspectorPanelPosition();
    }

    public void Exit()
    {
        transform.position = _originPosition;
        _isHovered = false;       
    }
    public void UpdateInspectorPanelPosition()
    {
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform,
            Input.mousePosition, null, out anchoredPos);

        // 기본 위치에 오프셋 추가 (마우스 포인터에서 150, 0만큼 떨어뜨림)
        Vector2 finalPos = anchoredPos + new Vector2(220, -220);

        _inspectorPanelRectTransform.anchoredPosition = finalPos;
        Debug.Log("ffffffffffffffffffffffffffffff");
    }


}
