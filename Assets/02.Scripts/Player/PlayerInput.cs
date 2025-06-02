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

            if (Input.GetButtonDown("Fire1"))
            {
                if (_AttackWaitCoroutine != null)
                    StopCoroutine(_AttackWaitCoroutine);

                _AttackWaitCoroutine = StartCoroutine(AttackWait());
            }

            if (Input.GetButtonDown("Fire2"))
            {
                if (_AttackWaitCoroutine != null)
                    StopCoroutine(_AttackWaitCoroutine);

                _AttackWaitCoroutine = StartCoroutine(RightAttackWait());
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (_AttackWaitCoroutine != null)
                    StopCoroutine(_AttackWaitCoroutine);

                _AttackWaitCoroutine = StartCoroutine(RollWait());
            }

            // 키 입력 처리
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (_AttackWaitCoroutine != null)
                    StopCoroutine(_AttackWaitCoroutine);

                _AttackWaitCoroutine = StartCoroutine(Skill1Wait());
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (_AttackWaitCoroutine != null)
                    StopCoroutine(_AttackWaitCoroutine);

                _AttackWaitCoroutine = StartCoroutine(Skill2Wait());
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (_AttackWaitCoroutine != null)
                    StopCoroutine(_AttackWaitCoroutine);

                _AttackWaitCoroutine = StartCoroutine(Skill3Wait());
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (_AttackWaitCoroutine != null)
                    StopCoroutine(_AttackWaitCoroutine);

                _AttackWaitCoroutine = StartCoroutine(Skill4Wait());
            }
            _isPause = Input.GetKeyDown(KeyCode.Escape);

            if (Input.GetKeyDown(KeyCode.T))
            {
                if (_AttackWaitCoroutine != null)
                {
                    StopCoroutine(_AttackWaitCoroutine);
                }

                _AttackWaitCoroutine = StartCoroutine(UltimateWait());
            }

            if (_isPause)
            {
                GainControl();
                _isPause = false;
            }
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
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
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
