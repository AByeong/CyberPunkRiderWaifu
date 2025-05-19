using UnityEngine;
using System.Collections;

namespace JY
{

    public class PlayerInput : MonoBehaviour
    {
        [HideInInspector]
        public bool playerControllerInputBlocked;

        protected Vector2 _movement;
        protected Vector2 _camera;
        protected bool _isJump;
        protected bool _isAttack;
        protected bool _isPause;
        protected bool _isExternalInputBlocked;

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

        public bool JumpInput {
            get { return _isJump && !playerControllerInputBlocked && !_isExternalInputBlocked; }
        }

        public bool Attack {
            get { return _isAttack && !playerControllerInputBlocked && !_isExternalInputBlocked; }
        }

        public bool Pause {
            get { return _isPause; }
        }

        WaitForSeconds m_AttackInputWait;
        Coroutine m_AttackWaitCoroutine;

        const float k_AttackInputDuration = 0.03f;

        void Update()
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

            // _isPause = Input.GetButtonDown("Pause");
        }

        IEnumerator AttackWait()
        {
            _isAttack = true;

            yield return m_AttackInputWait;

            _isAttack = false;
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