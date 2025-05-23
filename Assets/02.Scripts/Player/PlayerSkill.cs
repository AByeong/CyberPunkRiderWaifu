using Gamekit3D;
using JY;
using UnityEngine;
public class PlayerSkill : MonoBehaviour
{
    private PlayerInput _input;

    private int _keyIndex = -1;
    private PlayerController _player;
    private void Start()
    {
        _input = GetComponent<PlayerInput>();
        _player = GetComponent<PlayerController>();

    }

    private void Update()
    {
        if (_input.Skill1)
        {
            Debug.Log("1 입력받음");
            if (SkillManager.Instance.UseSkill(KeyCode.Alpha1, out _keyIndex))
            {
                _player.UseSkill(_keyIndex); // keyIndex: 0, 1, 2, 3 중 하나
                UIManager.Instance.StageMainUI.SkillIconLoad(_keyIndex);
            }
        }

        if (_input.Skill2)
        {
            Debug.Log("2 입력받음");
            if (SkillManager.Instance.UseSkill(KeyCode.Alpha2, out _keyIndex))
            {
                _player.UseSkill(_keyIndex); // keyIndex: 0, 1, 2, 3 중 하나
                UIManager.Instance.StageMainUI.SkillIconLoad(_keyIndex);
            }
        }
        if (_input.Skill3)
        {
            Debug.Log("3 입력받음");
            if (SkillManager.Instance.UseSkill(KeyCode.Alpha3, out _keyIndex))
            {
                _player.UseSkill(_keyIndex); // keyIndex: 0, 1, 2, 3 중 하나
                UIManager.Instance.StageMainUI.SkillIconLoad(_keyIndex);
            }
        }
        if (_input.Skill4)
        {
            Debug.Log("4 입력받음");
            if (SkillManager.Instance.UseSkill(KeyCode.Alpha4, out _keyIndex))
            {
                _player.UseSkill(_keyIndex); // keyIndex: 0, 1, 2, 3 중 하나
                UIManager.Instance.StageMainUI.SkillIconLoad(_keyIndex);
            }
        }
    }
}
