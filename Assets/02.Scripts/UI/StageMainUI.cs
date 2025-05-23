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

    public TextMeshProUGUI KillTrackingText;

    public void StageMainInit()
    {
        Debug.Log("MainUI Init");
        for (int s = 0; s < SkillIcons.Length; s++)
        {
            SkillIconSet(s);
        }
        ProgressSlider.value = 0;
    }

    private void SkillIconSet(int index)
    {

        SkillIcons[index].RestrictCondition = SkillManager.Instance.EquippedSkill[index].SkillData.CoolTime;
        SkillIcons[index].IconImageSprite = SkillManager.Instance.EquippedSkill[index].SkillData.Icon;
    }
    
    public void SkillIconLoad(int index)
    {
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

    public void RefreshKillTracking(string message)
    {
        KillTrackingText.text = message;
        ProgressSlider.value = (float)DeliveryManager.Instance.KillTracker.GetCurrentKillCount(EnemyType.Total)/(float)DeliveryManager.Instance.KillTracker.GetMissionKillCount(EnemyType.Total);
        finisherIcon.StackChange(DeliveryManager.Instance.KillTracker.TotalKillCount);
        
    }
    
}
