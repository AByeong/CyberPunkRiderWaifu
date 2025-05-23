using JY;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gamekit3D
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        private const float _airborneTurnSpeedProportion = 5.4f;
        private const float _groundedRayDistance = 0.01f;
        private const float _jumpAbortSpeed = 10f;
        private const float _minEnemyDotCoeff = 0.2f;
        private const float _inverseOneEighty = 1f / 180f;
        private const float k_StickingGravityProportion = 0.3f;
        private const float k_GroundAcceleration = 20f;
        private const float k_GroundDeceleration = 25f;

        public float MaxForwardSpeed = 8f;
        public float gravity = 20f;
        public float jumpSpeed = 10f;
        public float minTurnSpeed = 400f;
        public float maxTurnSpeed = 1200f;
        public float idleTimeout = 5f;
        public bool canAttack;
        public bool m_InCombo;
        [FormerlySerializedAs("m_Animator")]
        public Animator _animator;
        [Header("Dash")]
        public float DashDistance = 5.0f;
        public float DashCooldown;
        public float DashDuration = 0.2f;
        private readonly int m_HashAirborne = Animator.StringToHash("Airborne");

        // Parameters

        private readonly int m_HashAirborneVerticalSpeed = Animator.StringToHash("AirborneVerticalSpeed");
        private readonly int m_HashAngleDeltaRad = Animator.StringToHash("AngleDeltaRad");

        // Tags
        private readonly int m_HashBlockInput = Animator.StringToHash("BlockInput");
        private readonly int m_HashDeath = Animator.StringToHash("Death");
        private readonly int m_HashEllenCombo1 = Animator.StringToHash("EllenCombo1");
        private readonly int m_HashEllenCombo2 = Animator.StringToHash("EllenCombo2");
        private readonly int m_HashEllenCombo3 = Animator.StringToHash("EllenCombo3");
        private readonly int m_HashEllenCombo4 = Animator.StringToHash("EllenCombo4");

        private readonly int m_HashEllenDeath = Animator.StringToHash("EllenDeath");
        private readonly int m_HashFootFall = Animator.StringToHash("FootFall");
        private readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        private readonly int m_HashGrounded = Animator.StringToHash("Grounded");
        private readonly int m_HashHurt = Animator.StringToHash("Hurt");
        private readonly int m_HashHurtFromX = Animator.StringToHash("HurtFromX");
        private readonly int m_HashHurtFromY = Animator.StringToHash("HurtFromY");
        private readonly int m_HashInputDetected = Animator.StringToHash("InputDetected");
        private readonly int m_HashLanding = Animator.StringToHash("Landing");

        // States
        private readonly int m_HashLocomotion = Animator.StringToHash("Locomotion");
        private readonly int m_HashMeleeAttack = Animator.StringToHash("MeleeAttack");
        private readonly int m_HashRespawn = Animator.StringToHash("Respawn");
        private readonly int m_HashRightAttack = Animator.StringToHash("RightAttack");
        private readonly int m_HashRoll = Animator.StringToHash("Roll");
        private readonly int m_HashSkill1 = Animator.StringToHash("Skill1");
        private readonly int m_HashSkill2 = Animator.StringToHash("Skill2");
        private readonly int m_HashSkill3 = Animator.StringToHash("Skill3");
        private readonly int m_HashSkill4 = Animator.StringToHash("Skill4");
        private readonly int m_HashSkill5 = Animator.StringToHash("Skill5");
        private readonly int m_HashSkill6 = Animator.StringToHash("Skill6");
        private readonly int m_HashSkill7 = Animator.StringToHash("Skill7");
        private readonly int m_HashSkill8 = Animator.StringToHash("Skill8");
        private readonly int m_HashStateTime = Animator.StringToHash("StateTime");
        private readonly int m_HashTimeoutToIdle = Animator.StringToHash("TimeoutToIdle");
        private readonly int m_HashUpper = Animator.StringToHash("Upper");
        protected CharacterController _characterController;
        private float _dashCooldownTimer;
        private float _dashTime;
        protected PlayerInput _input;
        private bool _isDashing;
        protected bool _isGrounded = true;
        private IStatsProvider _stat;

        protected float m_AngleDiff;


        protected AnimatorStateInfo m_CurrentStateInfo;
        protected float m_DesiredForwardSpeed;
        protected float m_ForwardSpeed;
        private int m_HashTriggerSkill1;
        private int m_HashTriggerSkill2;
        private int m_HashTriggerSkill3;
        private int m_HashTriggerSkill4;
        protected float m_IdleTimer;
        protected bool m_InAttack;
        protected bool m_IsAnimatorTransitioning;
        protected AnimatorStateInfo m_NextStateInfo;
        protected Collider[] m_OverlapResult = new Collider[8];
        protected AnimatorStateInfo m_PreviousCurrentStateInfo;
        protected bool m_PreviousIsAnimatorTransitioning;
        protected bool m_PreviouslyGrounded = true;
        protected AnimatorStateInfo m_PreviousNextStateInfo;
        protected bool m_ReadyToJump;
        protected Renderer[] m_Renderers;
        protected bool m_Respawning;
        protected Quaternion m_TargetRotation;
        protected float m_VerticalSpeed;
        protected bool IsMoveInput => !Mathf.Approximately(_input.MoveInput.sqrMagnitude, 0f);
        private void Awake()
        {
            _input = GetComponent<PlayerInput>();
            _animator = GetComponent<Animator>();
            _characterController = GetComponent<CharacterController>();

        }
        private async void Start()
        {
            _stat = await StatLoader.LoadFromCSVAsync("PlayerStat.csv");

        }
// // 무기 효과 (+15 공격력)
//         stats = new StatModifierDecorator(stats, StatType.AttackPower, 15);
//
// // 방어구 효과 (+10 방어)
//         stats = new StatModifierDecorator(stats, StatType.Defense, 10);
//
// // 데미지 계산
//         float damage = stats.CalculateDamage(5); // base damage 5

        private void Update()
        {
            CacheAnimatorState();

            UpdateInputBlocking();

            EquipMeleeWeapon(IsWeaponEquiped());

            _animator.SetFloat(m_HashStateTime, Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
            _animator.ResetTrigger(m_HashMeleeAttack);
            _animator.ResetTrigger(m_HashRightAttack);
            _animator.ResetTrigger(m_HashRoll);


            if (_input.Attack && canAttack)
            {
                _animator.SetTrigger(m_HashMeleeAttack);
            }

            if (_input.RightAttack && canAttack)
            {
                _animator.SetTrigger(m_HashRightAttack);
            }

            _dashCooldownTimer -= Time.deltaTime;

            if (_input.Roll && _dashCooldownTimer <= 0f && !_isDashing)
            {
                _dashCooldownTimer = DashCooldown;
                Dash();
            }

            // 대시 중일 때 매 프레임 이동
            if (_isDashing)
            {
                float dashSpeed = DashDistance / DashDuration;
                Vector3 dashDirection = transform.forward;
                Vector3 dashMove = dashDirection * dashSpeed * Time.deltaTime;
                _characterController.Move(dashMove);

                _dashTime += Time.deltaTime;
                if (_dashTime >= DashDuration)
                {
                    _isDashing = false;
                }
            }
            CalculateForwardMovement();
            CalculateVerticalMovement();

            SetTargetRotation();

            if (IsOrientationUpdated() && IsMoveInput)
            {
                UpdateOrientation();
            }


            TimeoutToIdle();

            m_PreviouslyGrounded = _isGrounded;
        }
        // Called automatically by Unity after Awake whenever the script is enabled. 
        private void OnEnable()
        {
            EquipMeleeWeapon(false);

            m_Renderers = GetComponentsInChildren<Renderer>();
        }

        private void OnDisable()
        {
            for (int i = 0; i < m_Renderers.Length; ++i)
            {
                m_Renderers[i].enabled = true;
            }
        }

        private void OnAnimatorMove()
        {
            Vector3 movement;

            if (_isGrounded)
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.position + Vector3.up * _groundedRayDistance * 0.5f, -Vector3.up);
                if (Physics.Raycast(ray, out hit, _groundedRayDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    movement = Vector3.ProjectOnPlane(_animator.deltaPosition, hit.normal);
                    Renderer groundRenderer = hit.collider.GetComponentInChildren<Renderer>();
                }
                else
                {
                    movement = _animator.deltaPosition;
                }
            }
            else
            {
                movement = m_ForwardSpeed * transform.forward * Time.deltaTime;
            }

            _characterController.transform.rotation *= _animator.deltaRotation;
            movement += m_VerticalSpeed * Vector3.up * Time.deltaTime;
            _characterController.Move(movement);
            _isGrounded = _characterController.isGrounded;

            if (!_isGrounded)
                _animator.SetFloat(m_HashAirborneVerticalSpeed, m_VerticalSpeed);

            _animator.SetBool(m_HashGrounded, _isGrounded);
        }

        public void ApplyEquipment(StatType statType, float value)
        {
            _stat = new StatModifierDecorator(_stat, statType, value);
        }
        public void RemoveEquipment(StatType statType, float value)
        {
            _stat = new StatModifierDecorator(_stat, statType, -value);
        }
        public void Dash()
        {
            _animator.SetTrigger(m_HashRoll);
            _isDashing = true;
            _dashTime = 0f;
        }


        private void CacheAnimatorState()
        {
            m_PreviousCurrentStateInfo = m_CurrentStateInfo;
            m_PreviousNextStateInfo = m_NextStateInfo;
            m_PreviousIsAnimatorTransitioning = m_IsAnimatorTransitioning;

            m_CurrentStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            m_NextStateInfo = _animator.GetNextAnimatorStateInfo(0);
            m_IsAnimatorTransitioning = _animator.IsInTransition(0);
        }

        private void UpdateInputBlocking()
        {
            bool inputBlocked = m_CurrentStateInfo.tagHash == m_HashBlockInput && !m_IsAnimatorTransitioning;
            inputBlocked |= m_NextStateInfo.tagHash == m_HashBlockInput;
            _input.playerControllerInputBlocked = inputBlocked;
        }


        private bool IsWeaponEquiped()
        {
            // 지금 실행중인 애니메이션 이름을 참조해서 트루를 반환함, 스킬 사용 중일 때 참이 자동으로 되게 할거임
            bool equipped = m_NextStateInfo.shortNameHash == m_HashEllenCombo1 || m_CurrentStateInfo.shortNameHash == m_HashEllenCombo1;
            equipped |= m_NextStateInfo.shortNameHash == m_HashEllenCombo2 || m_CurrentStateInfo.shortNameHash == m_HashEllenCombo2;
            equipped |= m_NextStateInfo.shortNameHash == m_HashEllenCombo3 || m_CurrentStateInfo.shortNameHash == m_HashEllenCombo3;
            equipped |= m_NextStateInfo.shortNameHash == m_HashEllenCombo4 || m_CurrentStateInfo.shortNameHash == m_HashEllenCombo4;
            // 올려치기 공격
            equipped |= m_NextStateInfo.shortNameHash == m_HashUpper || m_CurrentStateInfo.shortNameHash == m_HashUpper;


            // 스킬 상태일 때 
            equipped |= m_NextStateInfo.shortNameHash == m_HashSkill1 || m_CurrentStateInfo.shortNameHash == m_HashSkill1;
            equipped |= m_NextStateInfo.shortNameHash == m_HashSkill2 || m_CurrentStateInfo.shortNameHash == m_HashSkill2;
            equipped |= m_NextStateInfo.shortNameHash == m_HashSkill3 || m_CurrentStateInfo.shortNameHash == m_HashSkill3;
            equipped |= m_NextStateInfo.shortNameHash == m_HashSkill4 || m_CurrentStateInfo.shortNameHash == m_HashSkill4;
            equipped |= m_NextStateInfo.shortNameHash == m_HashSkill5 || m_CurrentStateInfo.shortNameHash == m_HashSkill5;
            equipped |= m_NextStateInfo.shortNameHash == m_HashSkill6 || m_CurrentStateInfo.shortNameHash == m_HashSkill6;
            equipped |= m_NextStateInfo.shortNameHash == m_HashSkill7 || m_CurrentStateInfo.shortNameHash == m_HashSkill7;
            equipped |= m_NextStateInfo.shortNameHash == m_HashSkill8 || m_CurrentStateInfo.shortNameHash == m_HashSkill8;

            // 구르기 중일 때
            equipped |= m_NextStateInfo.shortNameHash == m_HashRoll || m_CurrentStateInfo.shortNameHash == m_HashRoll;

            return equipped;
        }

        // Called each physics step with a parameter based on the return value of IsWeaponEquiped.
        private void EquipMeleeWeapon(bool equip)
        {
            // meleeWeapon.gameObject.SetActive(equip);
            m_InAttack = false;
            m_InCombo = equip;

            if (!equip)
                _animator.ResetTrigger(m_HashMeleeAttack);
        }


        private void CalculateForwardMovement()
        {

            Vector2 moveInput = _input.MoveInput;
            if (moveInput.sqrMagnitude > 1f)
                moveInput.Normalize();

            m_DesiredForwardSpeed = moveInput.magnitude * MaxForwardSpeed;

            float acceleration = IsMoveInput ? k_GroundAcceleration : k_GroundDeceleration;

            m_ForwardSpeed = Mathf.MoveTowards(m_ForwardSpeed, m_DesiredForwardSpeed, acceleration * Time.deltaTime);

            _animator.SetFloat(m_HashForwardSpeed, m_ForwardSpeed);
        }

        // Called each physics step.
        private void CalculateVerticalMovement()
        {
            // If jump is not currently held and Ellen is on the ground then she is ready to jump.
            if (!_input.JumpInput && _isGrounded)
                m_ReadyToJump = true;

            if (_isGrounded)
            {
                // When grounded we apply a slight negative vertical speed to make Ellen "stick" to the ground.
                m_VerticalSpeed = -gravity * k_StickingGravityProportion;

                // If jump is held, Ellen is ready to jump and not currently in the middle of a melee combo...
                if (_input.JumpInput && m_ReadyToJump && !m_InCombo)
                {
                    // ... then override the previously set vertical speed and make sure she cannot jump again.
                    m_VerticalSpeed = jumpSpeed;
                    _isGrounded = false;
                    m_ReadyToJump = false;
                }
            }
            else
            {
                // If Ellen is airborne, the jump button is not held and Ellen is currently moving upwards...
                if (!_input.JumpInput && m_VerticalSpeed > 0.0f)
                {
                    // ... decrease Ellen's vertical speed.
                    // This is what causes holding jump to jump higher that tapping jump.
                    m_VerticalSpeed -= _jumpAbortSpeed * Time.deltaTime;
                }

                // If a jump is approximately peaking, make it absolute.
                if (Mathf.Approximately(m_VerticalSpeed, 0f))
                {
                    m_VerticalSpeed = 0f;
                }

                // If Ellen is airborne, apply gravity.
                m_VerticalSpeed -= gravity * Time.deltaTime;
            }
        }

        // Called each physics step to set the rotation Ellen is aiming to have.
        private void SetTargetRotation()
        {
            Vector2 moveInput = _input.MoveInput;
            Vector3 localMovementDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            float cameraYRotation = Camera.main.transform.eulerAngles.y;

            Vector3 moveDirection = Quaternion.Euler(0f, cameraYRotation, 0f) * localMovementDirection;

            Quaternion targetRotation;

            if (Mathf.Approximately(Vector3.Dot(localMovementDirection, Vector3.forward), -1.0f))
            {
                targetRotation = Quaternion.LookRotation(Quaternion.Euler(0f, cameraYRotation, 0f) * -Vector3.forward);
            }
            else
            {
                
                if (moveDirection != Vector3.zero)
                {
                    targetRotation = Quaternion.LookRotation(moveDirection);
                }
                else
                {
                    
                    targetRotation = Quaternion.identity; 
                }
                    //targetRotation = (Quaternion.LookRotation(moveDirection).Equals(Vector3.zero)) ? Quaternion.identity : Quaternion.LookRotation(moveDirection);
                
            }

            Vector3 resultingForward = targetRotation * Vector3.forward;

            if (m_InAttack)
            {
                Vector3 centre = transform.position + transform.forward * 2.0f + transform.up;
                Vector3 halfExtents = new Vector3(3.0f, 1.0f, 2.0f);
                int layerMask = 1 << LayerMask.NameToLayer("Enemy");
                int count = Physics.OverlapBoxNonAlloc(centre, halfExtents, m_OverlapResult, targetRotation, layerMask);

                float closestDot = 0.0f;
                Vector3 closestForward = Vector3.zero;
                int closest = -1;

                for (int i = 0; i < count; ++i)
                {
                    Vector3 playerToEnemy = m_OverlapResult[i].transform.position - transform.position;
                    playerToEnemy.y = 0;
                    playerToEnemy.Normalize();

                    float d = Vector3.Dot(resultingForward, playerToEnemy);

                    if (d > _minEnemyDotCoeff && d > closestDot)
                    {
                        closestForward = playerToEnemy;
                        closestDot = d;
                        closest = i;
                    }
                }

                if (closest != -1)
                {
                    resultingForward = closestForward;
                    transform.rotation = Quaternion.LookRotation(resultingForward); // 즉시 회전
                }
            }

            float angleCurrent = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
            float targetAngle = Mathf.Atan2(resultingForward.x, resultingForward.z) * Mathf.Rad2Deg;

            m_AngleDiff = Mathf.DeltaAngle(angleCurrent, targetAngle);
            m_TargetRotation = targetRotation;
        }


        // Called each physics step to help determine whether Ellen can turn under player input.
        private bool IsOrientationUpdated()
        {
            bool updateOrientationForLocomotion = !m_IsAnimatorTransitioning && m_CurrentStateInfo.shortNameHash == m_HashLocomotion || m_NextStateInfo.shortNameHash == m_HashLocomotion;
            bool updateOrientationForAirborne = !m_IsAnimatorTransitioning && m_CurrentStateInfo.shortNameHash == m_HashAirborne || m_NextStateInfo.shortNameHash == m_HashAirborne;
            bool updateOrientationForLanding = !m_IsAnimatorTransitioning && m_CurrentStateInfo.shortNameHash == m_HashLanding || m_NextStateInfo.shortNameHash == m_HashLanding;

            return updateOrientationForLocomotion || updateOrientationForAirborne || updateOrientationForLanding || m_InCombo && !m_InAttack;
        }

        // Called each physics step after SetTargetRotation if there is move input and Ellen is in the correct animator state according to IsOrientationUpdated.
        private void UpdateOrientation()
        {
            _animator.SetFloat(m_HashAngleDeltaRad, m_AngleDiff * Mathf.Deg2Rad);

            Vector3 localInput = new Vector3(_input.MoveInput.x, 0f, _input.MoveInput.y);
            float groundedTurnSpeed = Mathf.Lerp(maxTurnSpeed, minTurnSpeed, m_ForwardSpeed / m_DesiredForwardSpeed);
            float actualTurnSpeed = _isGrounded ? groundedTurnSpeed : Vector3.Angle(transform.forward, localInput) * _inverseOneEighty * _airborneTurnSpeedProportion * groundedTurnSpeed;
            m_TargetRotation = Quaternion.RotateTowards(transform.rotation, m_TargetRotation, actualTurnSpeed * Time.deltaTime);

            transform.rotation = m_TargetRotation;
        }
        private void TimeoutToIdle()
        {
            bool inputDetected = IsMoveInput || _input.Attack || _input.JumpInput || _input.RightAttack;
            if (_isGrounded && !inputDetected)
            {
                m_IdleTimer += Time.deltaTime;

                if (m_IdleTimer >= idleTimeout)
                {
                    m_IdleTimer = 0f;
                    _animator.SetTrigger(m_HashTimeoutToIdle);
                }
            }
            else
            {
                m_IdleTimer = 0f;
                _animator.ResetTrigger(m_HashTimeoutToIdle);
            }

            _animator.SetBool(m_HashInputDetected, inputDetected);
        }

        // This is called by an animation event when Ellen swings her staff.
        public void MeleeAttackStart(int throwing = 0)
        {
            // meleeWeapon.BeginAttack(throwing != 0);
            m_InAttack = true;
        }

        // This is called by an animation event when Ellen finishes swinging her staff.
        public void MeleeAttackEnd()
        {
            // meleeWeapon.EndAttack();
            m_InAttack = false;
        }


        // Called by a state machine behaviour on Ellen's animator controller.
        public void RespawnFinished()
        {
            m_Respawning = false;

            //we set the damageable invincible so we can't get hurt just after being respawned (feel like a double punitive)
            // m_Damageable.isInvulnerable = false;
        }
        public void UseSkill(int skillNumber)
        {
            switch (skillNumber)
            {
                case 0:
                    m_HashTriggerSkill1 = Animator.StringToHash(SkillManager.Instance.EquippedSkills[0].SkillData.TriggerName);
                    _animator.SetTrigger(m_HashTriggerSkill1);
                    break;
                case 1:
                    m_HashTriggerSkill2 = Animator.StringToHash(SkillManager.Instance.EquippedSkills[1].SkillData.TriggerName);
                    _animator.SetTrigger(m_HashTriggerSkill2);
                    break;
                case 2:
                    m_HashTriggerSkill3 = Animator.StringToHash(SkillManager.Instance.EquippedSkills[2].SkillData.TriggerName);
                    _animator.SetTrigger(m_HashTriggerSkill3);
                    break;
                case 3:
                    m_HashTriggerSkill4 = Animator.StringToHash(SkillManager.Instance.EquippedSkills[3].SkillData.TriggerName);
                    _animator.SetTrigger(m_HashTriggerSkill4);
                    break;
            }

        }
        public void TakeDamage(Damage damage)
        {
            // Set the Hurt parameter of the animator.
            _animator.SetTrigger(m_HashHurt);

            // Find the direction of the damage.
            Vector3 forward = damage.From.transform.position - transform.position;
            forward.y = 0f;

            Vector3 localHurt = transform.InverseTransformDirection(forward);

            // Set the HurtFromX and HurtFromY parameters of the animator based on the direction of the damage.
            _animator.SetFloat(m_HashHurtFromX, localHurt.x);
            _animator.SetFloat(m_HashHurtFromY, localHurt.z);

        }
    }

}
