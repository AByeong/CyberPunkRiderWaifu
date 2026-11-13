using System.Collections.Generic;
using System.IO;
using JY;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.PlayerLoop;

public enum EConsumableItemType
{
    HPRecovery, UltCooldown, AllSkillCooldown, Count
}
public class ConsumableItemManager : Singleton<ConsumableItemManager>
{
    public List<int> ConsumableItems = new List<int> { 0, 0, 0 };
    public StageMainUI UI;

    private string saveFilePath;

    protected override void Awake()
    {
        base.Awake();
        saveFilePath = Path.Combine(Application.persistentDataPath, "consumable_items.json");
    }

    public void AddItem(int itemIndex, int amount)
    {
        ConsumableItems[itemIndex] += amount;
        Save();
    }

    void Update()
    {
        // 로비(배달 중 아님)나 일시정지 중에는 소모품을 쓰지 않는다.
        // 로비에서 DeliveryManager.Instance에 접근하면 빈 DeliveryManager가 자동 생성되어 미션이 망가진다.
        if (!UIManager.Instance.isInDelivery || Time.timeScale == 0f) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseItem((int)EConsumableItemType.HPRecovery);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseItem((int)EConsumableItemType.UltCooldown);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            UseItem((int)EConsumableItemType.AllSkillCooldown);
        }
    }
    public void UseItem(int itemIndex)
    {
        if (ConsumableItems[itemIndex] <= 0)
        {
            Debug.Log("소모품 아이템 갯수 부족");
            return;
        }

        if (itemIndex == (int)EConsumableItemType.HPRecovery)
        {
            if (GameManager.Instance.player.CurrentHealth >= Mathf.RoundToInt(GameManager.Instance.player.MaxHealth))
            {
                Debug.Log("체력 만땅인데용ㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇ");
                return;
            }
            Debug.Log("체력 회복");
            if (GameManager.Instance.player.CurrentHealth >= Mathf.RoundToInt(GameManager.Instance.player.MaxHealth * 0.8f))
            {
                // CurrentHealth가 MaxHealth의 80%보다 높을 때의 처리
                GameManager.Instance.player.CurrentHealth = GameManager.Instance.player.MaxHealth;
            }
            else
            {
                int regenValue = Mathf.RoundToInt(GameManager.Instance.player.MaxHealth * 0.2f);
                GameManager.Instance.player.CurrentHealth += regenValue;
            }
            
            UIManager.Instance.StageMainUI.RefreshHPbar();
        }
        else if (itemIndex == (int)EConsumableItemType.UltCooldown)
        {
            if (DeliveryManager.Instance.UltimateGaze >= 100)
            {
                return;
            }
            Debug.Log("쿨감 20%");
            //UI.finisherIcon.Loading.fillAmount += 0.2f;
            DeliveryManager.Instance.UltimateGaze += 20;
            // 궁극기 쿨타임 감소
        }
        else if (itemIndex == (int)EConsumableItemType.AllSkillCooldown)
        {
            Debug.Log("스킬 쿨타임 0초");
            // 모든 스킬 쿨타임 0초
            SkillManager.Instance.AllSkillCooltimeZero();
        }

        ConsumableItems[itemIndex] -= 1;
        Save();

        StageMainUI ui = UI != null ? UI : UIManager.Instance.StageMainUI;
        if (ui != null) ui.RefreshItem();
    }

    public void Save()
    {
        try
        {
            string json = JsonConvert.SerializeObject(ConsumableItems, Formatting.Indented);
            File.WriteAllText(saveFilePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save: " + e.Message);
        }
    }

    public void Load()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                ConsumableItems = JsonConvert.DeserializeObject<List<int>>(json);
                
                if (ConsumableItems == null || ConsumableItems.Count != 3)
                {
                    ConsumableItems = new List<int> { 0, 0, 0 };
                }
            }
            else
            {
                ConsumableItems = new List<int> { 0, 0, 0 };
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load: " + e.Message);
            ConsumableItems = new List<int> { 0, 0, 0 };
        }
    }

    void Start()
    {
        Load();
    }

}