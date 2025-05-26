using UnityEngine;

public enum EtcType
{
    HPRegenBottle,
    Grenade,
    
}
[CreateAssetMenu(fileName = "EtcData", menuName = "Scriptable Objects/EtcData")]
public class EtcData : ScriptableObject
{
    public EtcType Type;
    public int Amount;
    public float ApplyAmount; 
}