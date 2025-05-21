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

    public void RefreshKillTrackingText(string message)
    {
        KillTrackingText.text = message;
    }
    
}
