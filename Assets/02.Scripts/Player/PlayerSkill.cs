using JY;
using UnityEngine;
public class PlayerSkill : MonoBehaviour
{
    private PlayerInput _input;

    private int _keyIndex = -1;
    private PlayerController _player;
    private StretchColliderOnly _stretchColliderOnly;
    private void Start()
    {
        _input = GetComponent<PlayerInput>();
        _player = GetComponent<PlayerController>();
        _stretchColliderOnly = GetComponentInChildren<StretchColliderOnly>();
    }

    private void Update()
    {
        if (_input.Skill1)
        {
            if (SkillManager.Instance.UseSkill(KeyCode.Alpha1, out _keyIndex))
            {
                _player.UseSkill(_keyIndex);
                _stretchColliderOnly.StretchDown(SkillManager.Instance.EquippedSkills[_keyIndex].SkillData.SkillRange);
                if (UIManager.Instance.StageMainUI != null)
                {
                    UIManager.Instance.StageMainUI.SkillIconLoad(_keyIndex);
                }
            }
        }

        if (_input.Skill2)
        {
            if (SkillManager.Instance.UseSkill(KeyCode.Alpha2, out _keyIndex))
            {
                _player.UseSkill(_keyIndex);
                _stretchColliderOnly.StretchDown(SkillManager.Instance.EquippedSkills[_keyIndex].SkillData.SkillRange);
                if (UIManager.Instance.StageMainUI != null)
                {
                    UIManager.Instance.StageMainUI.SkillIconLoad(_keyIndex);
                }
            }
        }
        if (_input.Skill3)
        {
            if (SkillManager.Instance.UseSkill(KeyCode.Alpha3, out _keyIndex))
            {
                _player.UseSkill(_keyIndex);
                _stretchColliderOnly.StretchDown(SkillManager.Instance.EquippedSkills[_keyIndex].SkillData.SkillRange);
                if (UIManager.Instance.StageMainUI != null)
                {
                    UIManager.Instance.StageMainUI.SkillIconLoad(_keyIndex);
                }
            }
        }
        if (_input.Skill4)
        {
            if (SkillManager.Instance.UseSkill(KeyCode.Alpha4, out _keyIndex))
            {
                _player.UseSkill(_keyIndex);
                _stretchColliderOnly.StretchDown(SkillManager.Instance.EquippedSkills[_keyIndex].SkillData.SkillRange);
                if (UIManager.Instance.StageMainUI != null)
                {
                    UIManager.Instance.StageMainUI.SkillIconLoad(_keyIndex);
                }
            }
        }
    }
}
