using UnityEngine;

[CreateAssetMenu(fileName = "DeliveryMissionDataSO", menuName = "Scriptable Objects/DeliveryMissionDataSO")]
public class DeliveryMissionDataSO : ScriptableObject
{
    public string ID;
    public string Name;
    public string Description;
    public Sprite Thumbnail;
    public int Difficulty;
    public DeliveryRewardSO Reward;
    public DeliverystageDataSO DeliverystageData;
    
    
    
}
