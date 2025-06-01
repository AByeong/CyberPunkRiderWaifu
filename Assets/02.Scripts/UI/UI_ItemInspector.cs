using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemInspector : Singleton<UI_ItemInspector>
{
    public GameObject InspectorPanel;
    public TextMeshProUGUI RarityText;
    public TextMeshProUGUI ItemNameText;
    public TextMeshProUGUI StatNameText;
    public TextMeshProUGUI StatValueText;
    public TextMeshProUGUI CubeStatText;
    public TextMeshProUGUI SetEffectText;
    public Image ItemIcon;

    private RectTransform _canvasRectTransform;
    private RectTransform _inspectorPanelRectTransform;

    private Vector3 _originPosition;

    public bool _isHovered = false;
    public void Hovered(Item item)
    {
        switch(item.Data.ItemRarity)
        {
            case ItemRarity.Normal:
                RarityText.color = Color.white;
                RarityText.text = $"Normal";
                break;
            case ItemRarity.Rare:
                RarityText.color = Color.blue;
                RarityText.text = $"Rare";
                break;
            case ItemRarity.Unique:
                RarityText.color = Color.magenta;
                RarityText.text = $"Unique";
                break;
        }


        ItemIcon.sprite = item.Data.Icon;

        if(item.Data is EquipmentDataSO equipData)
        {
            switch (equipData.EquipmentType)
            {
                case EquipmentType.Weapon:
                    ItemNameText.text = "무기";
                    StatNameText.text = "공격력\n공격속도\n치명타 확률\n치명타 배율";
                    StatValueText.text = $"{(int)item.AttackPower}\n{(int)item.AttackSpeed}\n{(int)item.CritChance}\n{(int)item.CritDamage}";
                    // TODO : CubeStat 작성, 세트 효과 작성
                    break;
                case EquipmentType.Armor:
                    ItemNameText.text = "갑옷";
                    StatNameText.text = "방어력";
                    StatValueText.text = $"{(int)item.Defense}";
                    break;
                case EquipmentType.Head:
                    ItemNameText.text = "투구";
                    StatNameText.text = "최대생명력";
                    StatValueText.text = $"{(int)item.MaxHealth}";
                    break;
                case EquipmentType.Boots:
                    ItemNameText.text = "신발";
                    StatNameText.text = "이동속도";
                    StatValueText.text = $"{(int)item.Speed}";
                    break;
            }
        }
        else if(item.Data is ChipDataSO chipData)
        {
            ItemNameText.text = "스킬칩";
            StatNameText.text = "스킬범위증가\n쿨타임감소";
            StatValueText.text = $"{(int)item.SkillRange}\n";
            StatValueText.text += string.Format("{0:N2}", item.ReduceCooldown);
        }
        _isHovered = true;

        UpdateInspectorPanelPosition();
    }
    public void HoverExit()
    {
        transform.position = _originPosition;
        _isHovered = false;
        //InspectorPanel.SetActive(false);
    }

    private void Start()
    {
        _inspectorPanelRectTransform = InspectorPanel.GetComponent<RectTransform>();
        _canvasRectTransform = InspectorPanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        _originPosition = transform.position;
    }
    private void Update()
    {
        //if (_isHovered == true)
        //{
        //    UpdateInspectorPanelPosition();
        //}
    }


    public void UpdateInspectorPanelPosition()
    {
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform,
             Input.mousePosition, null, out anchoredPos);

        // 기본 위치에 오프셋 추가 (마우스 포인터에서 40, 40만큼 떨어뜨림)
        Vector2 finalPos = anchoredPos + new Vector2(-150, 0);

        _inspectorPanelRectTransform.anchoredPosition = finalPos;
        Debug.Log("ddddddddddddddddddddddddddddddd");
    }
}
