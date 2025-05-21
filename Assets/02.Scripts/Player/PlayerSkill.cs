using Gamekit3D;
using JY;
using UnityEngine;
public class PlayerSkill : MonoBehaviour
{
    private PlayerInput _input;
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
            SkillManager.Instance.UseSkill(KeyCode.Alpha1);
            _player.UseSkill(KeyCode.Alpha1);
        }
    }
}
