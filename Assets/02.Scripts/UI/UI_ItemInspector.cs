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
        CubeStatText.text = "";

        if (item.Data is EquipmentDataSO equipData)
        {
            foreach (var cubeStat in item.CubeStats)
            {
                switch (cubeStat.StatType)
                {
                    case StatType.MaxHealth:
                        CubeStatText.text += $"�ִ����� {Mathf.RoundToInt(cubeStat.Value * 100)}%\n";
                        break;
                    case StatType.AttackPower:
                        CubeStatText.text += $"���ݷ� {Mathf.RoundToInt(cubeStat.Value * 100)}%\n";
                        break;
                    case StatType.Defense:
                        CubeStatText.text += $"���� {Mathf.RoundToInt(cubeStat.Value * 100)}%\n";
                        break;
                    case StatType.Speed:
                        CubeStatText.text += $"�̵��ӵ� {Mathf.RoundToInt(cubeStat.Value * 100)}%\n";
                        break;
                    case StatType.AttackSpeed:
                        CubeStatText.text += $"���ݼӵ� {Mathf.RoundToInt(cubeStat.Value * 100)}%\n";
                        break;
                    case StatType.CritChance:
                        CubeStatText.text += $"ġ��Ÿ Ȯ�� {Mathf.RoundToInt(cubeStat.Value * 100)}%\n";
                        break;
                    case StatType.CritDamage:
                        CubeStatText.text += $"ġ��Ÿ ���� {Mathf.RoundToInt(cubeStat.Value * 100)}%\n";
                        break;
                }
                Debug.Log("UI_CubeStattext ȣ�⤷����������������");
            }
            switch (equipData.EquipmentType)
            {
                case EquipmentType.Weapon:
                    ItemNameText.text = "����";
                    StatNameText.text = "���ݷ�\n���ݼӵ�\nġ��Ÿ Ȯ��\nġ��Ÿ ����";
                    StatValueText.text = $"{(int)item.AttackPower}\n{Mathf.RoundToInt(item.AttackSpeed * 100)}\n{Mathf.RoundToInt(item.CritChance * 100)}\n{Mathf.RoundToInt(item.CritDamage * 100)}";
                    // TODO : CubeStat �ۼ�, ��Ʈ ȿ�� �ۼ�
                    break;
                case EquipmentType.Armor:
                    ItemNameText.text = "����";
                    StatNameText.text = "����";
                    StatValueText.text = $"{Mathf.RoundToInt(item.Defense * 100)}";
                    break;
                case EquipmentType.Head:
                    ItemNameText.text = "����";
                    StatNameText.text = "�ִ�����";
                    StatValueText.text = $"{(int)item.MaxHealth}";
                    break;
                case EquipmentType.Boots:
                    ItemNameText.text = "�Ź�";
                    StatNameText.text = "�̵��ӵ�";
                    StatValueText.text = $"{(int)item.Speed}";
                    break;
            }
        }
        else if (item.Data is ChipDataSO chipData)
        {
            ItemNameText.text = "��ųĨ";
            StatNameText.text = "��ų��������\n��Ÿ�Ӱ���";
            StatValueText.text = $"{Mathf.RoundToInt(item.SkillRange * 100)}\n";
            StatValueText.text += string.Format("{0:N2}", item.ReduceCooldown);
            Debug.Log($"Cooldown = {item.ReduceCooldown}");
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

        // �⺻ ��ġ�� ������ �߰� (���콺 �����Ϳ��� 40, 40��ŭ ����߸�)
        Vector2 finalPos = anchoredPos + new Vector2(-150, 0);

        _inspectorPanelRectTransform.anchoredPosition = finalPos;
        Debug.Log("ddddddddddddddddddddddddddddddd");
    }
}
