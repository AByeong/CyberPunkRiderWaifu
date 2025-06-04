using JY;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class StageMainUI : MonoBehaviour
{
    [Header("아이콘")]

    public Icon[] SkillIcons;
    public Icon[] ItemIcons;
    public Icon finisherIcon;

    [Header("진행바")]
    public Slider ProgressSlider;
    
    
    [Header("HP")]
    public Slider HPSlider;
    public TextMeshProUGUI TextHP;
    
    
    [Header("퀘스트 바")]
    public QuestBar NormalQuestBar;
    public QuestBar EliteQuestBar;
    public QuestBar BossQuestBar;
    public QuestBar ElevatorQuestBar;
    
    [Header("소모 아이템")]
    public TextMeshProUGUI HPNumber;
    public TextMeshProUGUI UltimateNumber;
    public TextMeshProUGUI CooltimeNumber;
    
    
    [Header("스테이지 진행도")]
    public Image[] StageIcons;
    [SerializeField] private Color _acrivateColor;
    
    public void StageMainInit()
    {
        Debug.Log("MainUI Init");
        for (int s = 0; s < SkillIcons.Length; s++)
        {
            SkillIconSet(s);
        }
        ProgressSlider.value = 0;

        for (int i = 0; i < ItemIcons.Length; i++)
        {
            SkillIconSet(i);
        }
        
        QuestBarClear();
        RefreshKillTracking();
        RefreshHPbar();
        RefreshProgressbar();
        ActivateStage(DeliveryManager.Instance.CurrentSector);
        RefreshItem();
        ConsumableItemManager.Instance.UI = this;

    }

    private void SkillIconSet(int index)
    {

        for (int i = 0; i < SkillIcons.Length; i++)
        {
            SkillIcons[i].RestrictCondition = SkillManager.Instance.EquippedSkills[i].SkillData.CoolTime;
            SkillIcons[i].IconImage.sprite = SkillManager.Instance.EquippedSkills[i].SkillData.Icon;
        }
        
    }

    public void ActivateStage(int stage)
    {
        for (int i = 0; i < StageIcons.Length; i++)
        {
            if(i == stage) StageIcons[i].color = _acrivateColor;
        else
        {
            StageIcons[i].color = Color.white;
        }
            
        }
    }
    
    public void SkillIconLoad(int index)
    {
        Debug.Log(SkillManager.Instance.EquippedSkills[index].SkillData.SkillName);
        SkillIcons[index].StartCooltime();
    }

    public void ItemIconLoad(int index)
    {
        ItemIcons[index].StartCooltime();
    }

    public void RefreshProgressbar(int current, int max)
    {
        ProgressSlider.maxValue = max;
        ProgressSlider.value = current;

    }

    public void RefreshItem()
    {
        HPNumber.text = ConsumableItemManager.Instance.ConsumableItems[0].ToString();
        UltimateNumber.text = ConsumableItemManager.Instance.ConsumableItems[1].ToString();
        CooltimeNumber.text = ConsumableItemManager.Instance.ConsumableItems[2].ToString();

    }

    public void RefreshProgressbar()
    {
        if (DeliveryManager.Instance.KillTracker.MissionKillCount.Boss != 0)
        
        {
            ProgressSlider.maxValue = DeliveryManager.Instance.KillTracker.MissionKillCount.Normal +
                                      DeliveryManager.Instance.KillTracker.MissionKillCount.Elite +
                                      DeliveryManager.Instance.KillTracker.MissionKillCount.Boss +
                                      DeliveryManager.Instance.KillTracker.MissionKillCount.Normal;
            ProgressSlider.value = DeliveryManager.Instance.KillTracker.CurrentKillCount.Normal +
                                   DeliveryManager.Instance.KillTracker.CurrentKillCount.Elite +
                                   DeliveryManager.Instance.KillTracker.CurrentKillCount.Boss +
                                   DeliveryManager.Instance.KillTracker.CurrentKillCount.Normal;
        }
    }
    
    public void FinisherIconLoad()
    {
        finisherIcon.StartCooltime();
    }

    public void Refresh()
    {
        RefreshProgressbar();
        RefreshKillTracking();
        RefreshHPbar();
        RefreshItem();
        
    }
    public void RefreshHPbar()
    {
        HPSlider.maxValue = GameManager.Instance.player.MaxHealth;
        HPSlider.value = GameManager.Instance.player.CurrentHealth;
        // Debug.Log(GameManager.Instance.player.CurrentHealth);
        TextHP.text = $"{GameManager.Instance.player.CurrentHealth}/{GameManager.Instance.player.MaxHealth}";
    }
    
    public void QuestBarClear()
    {
        NormalQuestBar.gameObject.SetActive(false);
        EliteQuestBar.gameObject.SetActive(false);
        BossQuestBar.gameObject.SetActive(false);
        ElevatorQuestBar.gameObject.SetActive(false);
    }
    
    public void RefreshKillTracking()
    {
        if (DeliveryManager.Instance.KillTracker.MissionKillCount.Normal > 0 )
        {
            NormalQuestBar.Appear();
        }
        
        if (NormalQuestBar.gameObject.activeInHierarchy)
        {
            NormalQuestBar.Set(DeliveryManager.Instance.KillTracker.CurrentKillCount.Normal, DeliveryManager.Instance.KillTracker.MissionKillCount.Normal,"일반 몬스터 처치", "여기 들어오시면 안 돼요!");
        }
        
        if (DeliveryManager.Instance.KillTracker.MissionKillCount.Elite > 0)
        {
            EliteQuestBar.Appear();
        }
        if (EliteQuestBar.gameObject.activeInHierarchy)
        {
            EliteQuestBar.Set(DeliveryManager.Instance.KillTracker.CurrentKillCount.Elite, DeliveryManager.Instance.KillTracker.MissionKillCount.Elite, "엘리트 몬스터 처치", "긴급상황! 긴급상황!");
        }
        
            
        
        if (DeliveryManager.Instance.KillTracker.MissionKillCount.Boss > 0 )
        {
            BossQuestBar.Appear();
        }
        if (BossQuestBar.gameObject.activeInHierarchy)
        {
            BossQuestBar.Set(DeliveryManager.Instance.KillTracker.CurrentKillCount.Boss, DeliveryManager.Instance.KillTracker.MissionKillCount.Boss,"보스 처치", "서장님 도와줘요!");
        }

    }

    public void RefreshUltimateGaze()
    {
        finisherIcon.Loading.fillAmount =
            (float)DeliveryManager.Instance.UltimateGaze / (float)DeliveryManager.Instance.TargetUltimate;
    }
}
