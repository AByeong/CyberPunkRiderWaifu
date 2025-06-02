using DG.Tweening;
using TMPro;
using UnityEngine;

public class QuestBar : MonoBehaviour
{
    
    public TextMeshProUGUI MissionName;
    public TextMeshProUGUI MissionDescription;
    
    public TextMeshProUGUI CurrentKill;
    public TextMeshProUGUI MissionKill;
    
    public void Appear()
    {
        if (!this.gameObject.activeInHierarchy)
        {
            this.gameObject.SetActive(true);
            this.transform.localScale = new Vector3(0, 1, 1);
            this.transform.DOScale(new Vector3(1, 1, 1), 0.1f);
        }
    }

    public void Set(int current, int mission, string description, string title)
    {
        MissionName.text = title;
        MissionDescription.text = description;
        MissionKill.text = mission.ToString();
        CurrentKill.text = current.ToString();
        
    }
    
    
}
