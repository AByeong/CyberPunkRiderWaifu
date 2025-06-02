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
        this.transform.localScale =new Vector3(0,1,1);
        this.transform.DOScale(new Vector3(1,1,1), 0.1f);
    }
    
    
}
