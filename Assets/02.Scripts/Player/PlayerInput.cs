using System.Collections;
using UnityEngine;

namespace JY
{

    public class PlayerInput : MonoBehaviour
    {

        private const float k_AttackInputDuration = 0.03f;
        [HideInInspector]
        public bool playerControllerInputBlocked;
        protected Vector2 _camera;
        protected bool _isAttack;
        protected bool _isExternalInputBlocked;
        protected bool _isJump;
        protected bool _isPause;
        protected bool _isRightAttack;
        protected bool _isRoll;
        protected bool _isSkill1;
        protected bool _isSkill2;

        protected Vector2 _movement;

        private WaitForSeconds m_AttackInputWait;
        private Coroutine m_AttackWaitCoroutine;
        public Vector2 MoveInput {
            get
            {
                if (playerControllerInputBlocked || _isExternalInputBlocked)
                    return Vector2.zero;
                return _movement;
            }
        }

        public Vector2 CameraInput {
            get
            {
                if (playerControllerInputBlocked || _isExternalInputBlocked)
                    return Vector2.zero;
                return _camera;
            }
        }

        public bool JumpInput => _isJump && !playerControllerInputBlocked && !_isExternalInputBlocked;

        public bool Attack => _isAttack && !playerControllerInputBlocked && !_isExternalInputBlocked;

        public bool RightAttack => _isRightAttack && !playerControllerInputBlocked && !_isExternalInputBlocked;

        public bool Roll => _isRoll && !playerControllerInputBlocked && !_isExternalInputBlocked;
        public bool Pause => _isPause;

        public bool Skill1 => _isSkill1 && !playerControllerInputBlocked && !_isExternalInputBlocked;

        public bool Skill2 => _isSkill2 && !playerControllerInputBlocked && !_isExternalInputBlocked;
        private void Update()
        {
            _movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            _camera.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            _isJump = Input.GetButton("Jump");

            if (Input.GetButtonDown("Fire1"))
            {
                if (m_AttackWaitCoroutine != null)
                    StopCoroutine(m_AttackWaitCoroutine);

                m_AttackWaitCoroutine = StartCoroutine(AttackWait());
            }

            if (Input.GetButtonDown("Fire2"))
            {
                if (m_AttackWaitCoroutine != null)
                    StopCoroutine(m_AttackWaitCoroutine);

                m_AttackWaitCoroutine = StartCoroutine(RightAttackWait());
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Debug.Log("Left Shift Pressed");
                if (m_AttackWaitCoroutine != null)
                    StopCoroutine(m_AttackWaitCoroutine);

                m_AttackWaitCoroutine = StartCoroutine(RollWait());
            }

            // 키 입력 처리
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (m_AttackWaitCoroutine != null)
                    StopCoroutine(m_AttackWaitCoroutine);

                m_AttackWaitCoroutine = StartCoroutine(Skill1Wait());
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (m_AttackWaitCoroutine != null)
                    StopCoroutine(m_AttackWaitCoroutine);

                m_AttackWaitCoroutine = StartCoroutine(Skill2Wait());
            }
            // if (Input.GetKeyDown(KeyCode.Alpha3))
            // {
            //     UseSkill(KeyCode.Alpha3);
            // }
            // if (Input.GetKeyDown(KeyCode.Alpha4))
            // {
            //     UseSkill(KeyCode.Alpha4);
            // }
            // _isPause = Input.GetButtonDown("Pause");
        }

        private IEnumerator AttackWait()
        {
            _isAttack = true;

            yield return m_AttackInputWait;

            _isAttack = false;
        }

        private IEnumerator RightAttackWait()
        {
            _isRightAttack = true;

            yield return m_AttackInputWait;

            _isRightAttack = false;
        }
        private IEnumerator Skill1Wait()
        {
            _isSkill1 = true;

            yield return m_AttackInputWait;

            _isSkill1 = false;
        }
        private IEnumerator Skill2Wait()
        {
            _isSkill2 = true;

            yield return m_AttackInputWait;

            _isSkill2 = false;
        }
        private IEnumerator RollWait()
        {
            _isRoll = true;

            yield return m_AttackInputWait;

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
