using System.Collections;
using UnityEngine;

namespace JY
{
    public class PlayerInput : MonoBehaviour
    {
        [HideInInspector]
        public bool playerControllerInputBlocked;

        private WaitForSeconds _AttackInputWait;
        private Coroutine _AttackWaitCoroutine;
        protected Vector2 _camera;
        protected bool _isAttack;
        protected bool _isExternalInputBlocked;
        protected bool _isJump;
        protected bool _isPause;
        protected bool _isRightAttack;
        protected bool _isRoll;
        protected bool _isSkill1;
        protected bool _isSkill2;
        protected bool _isSkill3;
        protected bool _isSkill4;
        protected bool _isUltimate;
        protected Vector2 _movement;

        public Vector2 MoveInput
        {
            get
            {
                if (playerControllerInputBlocked || _isExternalInputBlocked)
                {
                    return Vector2.zero;
                }

                return _movement;
            }
        }
        public bool JumpInput => _isJump && !playerControllerInputBlocked && !_isExternalInputBlocked;

        public bool Attack => _isAttack && !playerControllerInputBlocked && !_isExternalInputBlocked;

        public bool RightAttack => _isRightAttack && !playerControllerInputBlocked && !_isExternalInputBlocked;

        public bool Roll => _isRoll && !playerControllerInputBlocked && !_isExternalInputBlocked;
        public bool Pause => _isPause;

        public bool Skill1 => _isSkill1 && !playerControllerInputBlocked && !_isExternalInputBlocked;

        public bool Skill2 => _isSkill2 && !playerControllerInputBlocked && !_isExternalInputBlocked;

        public bool Skill3 => _isSkill3 && !playerControllerInputBlocked && !_isExternalInputBlocked;

        public bool Skill4 => _isSkill4 && !playerControllerInputBlocked && !_isExternalInputBlocked;

   public bool Ultimate => _isUltimate && !playerControllerInputBlocked && !_isExternalInputBlocked;

        private void Update()
        {
            _movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            _camera.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            _isJump = Input.GetButton("Jump");

            if (Input.GetButtonDown("Fire1")) StartInputWait(AttackWait());
            if (Input.GetButtonDown("Fire2")) StartInputWait(RightAttackWait());
            if (Input.GetKeyDown(KeyCode.LeftShift)) StartInputWait(RollWait());

            // 키 입력 처리
            if (Input.GetKeyDown(KeyCode.Alpha1)) StartInputWait(Skill1Wait());
            if (Input.GetKeyDown(KeyCode.Alpha2)) StartInputWait(Skill2Wait());
            if (Input.GetKeyDown(KeyCode.Alpha3)) StartInputWait(Skill3Wait());
            if (Input.GetKeyDown(KeyCode.Alpha4)) StartInputWait(Skill4Wait());
            if (Input.GetKeyDown(KeyCode.T)) StartInputWait(UltimateWait());

            // ESC 처리(일시정지 팝업, 조작 잠금/해제)는 UIManager가 담당한다.
            // 여기서 GainControl()을 호출하면 팝업이 열린 상태에서도 조작 잠금이 풀린다.
            _isPause = Input.GetKeyDown(KeyCode.Escape);
        }

        // 모든 입력이 코루틴 핸들 하나를 공유하므로, 이전 코루틴을 중단할 때 그 코루틴이
        // 끄지 못한 플래그를 직접 내려야 입력이 true로 고정되지 않는다.
        private void StartInputWait(IEnumerator routine)
        {
            if (_AttackWaitCoroutine != null)
            {
                StopCoroutine(_AttackWaitCoroutine);
                ClearBufferedInputs();
            }

            _AttackWaitCoroutine = StartCoroutine(routine);
        }

        private void ClearBufferedInputs()
        {
            _isAttack = false;
            _isRightAttack = false;
            _isRoll = false;
            _isSkill1 = false;
            _isSkill2 = false;
            _isSkill3 = false;
            _isSkill4 = false;
            _isUltimate = false;
        }

        private IEnumerator AttackWait()
        {
            _isAttack = true;

            yield return _AttackInputWait;

            _isAttack = false;
        }

        private IEnumerator RightAttackWait()
        {
            _isRightAttack = true;

            yield return _AttackInputWait;

            _isRightAttack = false;
        }
        private IEnumerator Skill1Wait()
        {
            _isSkill1 = true;

            yield return _AttackInputWait;

            _isSkill1 = false;
        }
        private IEnumerator Skill2Wait()
        {
            _isSkill2 = true;

            yield return _AttackInputWait;

            _isSkill2 = false;
        }
        private IEnumerator Skill3Wait()
        {
            _isSkill3 = true;

            yield return _AttackInputWait;

            _isSkill3 = false;
        }
        private IEnumerator Skill4Wait()
        {
            _isSkill4 = true;

            yield return _AttackInputWait;

            _isSkill4 = false;
        }

        private IEnumerator UltimateWait()
        {
            _isUltimate = true;
            yield return _AttackInputWait;

            _isUltimate = false;
        }
        private IEnumerator RollWait()
        {
            _isRoll = true;

            yield return _AttackInputWait;

            _isRoll = false;
        }

        public bool HaveControl()
        {
            return !_isExternalInputBlocked;
        }

        public void ReleaseControl()
        {
            _isExternalInputBlocked = true;
        }

        public void GainControl()
        {
            _isExternalInputBlocked = false;
        }
    }
}
