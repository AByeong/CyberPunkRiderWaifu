using JY;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayerSkill : MonoBehaviour
{
    private PlayerInput _input;

    private int _keyIndex = -1;
    private PlayerController _player;
    private StretchColliderOnly _stretchColliderOnly;
    public GameObject ActingPlayer;
    [Header("Ultimate ")] public PlayableDirector UltimatePD;
    public TimelineAsset UltimateTL;
    private void Start()
    {
        _input = GetComponent<PlayerInput>();
        _player = GetComponent<PlayerController>();
        _stretchColliderOnly = GetComponentInChildren<StretchColliderOnly>();

        UltimatePD.stopped += OnTimelineFinished;
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

        if (_input.Ultimate)
        {
            Ultimate();
        }
    }

    private void OnTimelineFinished(PlayableDirector pd)
    {
        if (pd == UltimatePD)
        {
            Debug.Log("Timeline has ended!");
            // 여기에 컷씬 종료 후 실행할 코드 작성
            _input.GainControl();
            // _input.playerControllerInputBlocked = false;
            ActingPlayer.transform.localPosition = Vector3.zero;
            DeliveryManager.Instance.UltimateGaze = 0;

        }
    }

    public void Ultimate()
    {
        if (DeliveryManager.Instance.UltimateGaze == DeliveryManager.Instance.TargetUltimate)
        {
            _input.ReleaseControl();
            _input.playerControllerInputBlocked = true;
            UltimatePD.Play(UltimateTL);

        }

    }
}
