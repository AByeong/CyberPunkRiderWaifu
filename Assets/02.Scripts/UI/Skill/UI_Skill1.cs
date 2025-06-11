using UnityEngine;
public class UI_Skill1 : MonoBehaviour
{
    public GameObject SkillText;

    private void Awake()
    {
        SkillText.SetActive(false);
    }

    private void Update()
    {
        Active();
    }
    private void Active()
    {
        if (DeliveryManager.Instance.UltimateGaze == DeliveryManager.Instance.TargetUltimate)
        {
            SkillText.SetActive(true);
        }
        else
        {
            SkillText.SetActive(false);
        }
    }

}
