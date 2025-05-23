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

        for (int i = 0; i < ItemIcons.Length; i++)
        {
            SkillIconSet(i);
        }
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

    public void RefreshKillTracking(string message)
    {
        KillTrackingText.text = message;
        ProgressSlider.value = DeliveryManager.Instance.KillTracker.GetCurrentKillCount(EnemyType.Total) / (float)DeliveryManager.Instance.KillTracker.GetMissionKillCount(EnemyType.Total);
        finisherIcon.StackChange(DeliveryManager.Instance.KillTracker.TotalKillCount);

    }
}
