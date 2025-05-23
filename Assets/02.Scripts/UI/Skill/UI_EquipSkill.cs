using UnityEngine;

namespace _02.Scripts.UI.Skill
{
    public class UI_EquipSkill : UI_Skill
    {
        public Item[] slot;

        public void SetChipOption(Item item)
        {
            _skillCooldown *= item.ChipData.ReduceCooldown;
            _skillRange *= item.ChipData.SkillRange;

            Debug.Log(_skillRange + "칩 할당됨!!");
        }

        public void RemoveChipOption(Item item)
        {

        }
    }
}
