using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class StageMainUI : MonoBehaviour
{
    [Header("메인 UI")]
    public Slider HPSlider;
    public Slider ProgressSlider;
    public Icon[] SkillIcons;
    public Icon[] ItemIcons;
    public Icon finisherIcon;

    [Header("퀘스트 바")]
    public QuestBar NormalQuestBar;
    public QuestBar EliteQuestBar;
    public QuestBar BossQuestBar;
    public QuestBar ElevatorQuestBar;
    
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
        
        NormalQuestBar.gameObject.SetActive(false);
        EliteQuestBar.gameObject.SetActive(false);
        BossQuestBar.gameObject.SetActive(false);
        ElevatorQuestBar.gameObject.SetActive(false);
        
        
    }

    private void SkillIconSet(int index)
    {

        for (int i = 0; i < SkillIcons.Length; i++)
        {
            SkillIcons[i].RestrictCondition = SkillManager.Instance.EquippedSkills[i].SkillData.CoolTime;
            SkillIcons[i].IconImage.sprite = SkillManager.Instance.EquippedSkills[i].SkillData.Icon;
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

    public void FinisherIconLoad()
    {
        finisherIcon.StartCooltime();
    }

    public void RefreshKillTracking()
    {
        
        if (NormalQuestBar.gameObject.activeInHierarchy)
        {
            NormalQuestBar.CurrentKill.text = DeliveryManager.Instance.KillTracker.CurrentKillCount.Normal.ToString();
        }

        if (EliteQuestBar.gameObject.activeInHierarchy)
        {
            EliteQuestBar.CurrentKill.text = DeliveryManager.Instance.KillTracker.CurrentKillCount.Elite.ToString();
        }

        if (BossQuestBar.gameObject.activeInHierarchy)
        {
            BossQuestBar.CurrentKill.text = DeliveryManager.Instance.KillTracker.CurrentKillCount.Boss.ToString();
        }

        if (DeliveryManager.Instance.KillTracker.IsMissionCompleted())
        {
            ElevatorQuestBar.gameObject.SetActive(true);
        }
       

    }
}
