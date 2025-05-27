using UnityEngine;
[CreateAssetMenu(fileName = "ChipData", menuName = "Scriptable Objects/ChipData")]
public class ChipDataSO : ItemBaseDataSO
{
    public new ItemType ItemType => ItemType.Chip;

    public float SkillRange;
    public float ReduceCooldown;
}
