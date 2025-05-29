using UnityEngine;

namespace JY
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerSound))]
    [RequireComponent(typeof(GroundCheck))]
    public class PlayerController : MonoBehaviour
    {
        private const float AirborneTurnSpeedProportion = 5.4f;
        private const float GroundedRayDistance = 0.01f;
        private const float JumpAbortSpeed = 10f;
        private const float MinEnemyDotCoeff = 0.2f;
        private const float InverseOneEighty = 1f / 180f;
        private const float StickingGravityProportion = 0.3f;
        private const float GroundAcceleration = 20f;
        private const float GroundDeceleration = 25f;
        public float MaxAirAttackGraceTime = 0.3f;

        public float MaxForwardSpeed;
        public float AttackPower;
        public float Gravity = 20f;
        public float JumpSpeed = 15f;
        public float MaxTurnSpeed = 1200f;
        public float MinTurnSpeed = 400f;
        public float IdleTimeout = 5f;
        public bool CanAttack;
        public bool IsInCombo;
        public bool IsAirCombo;
        [Header("Dash")]
        public float DashDistance => MaxForwardSpeed * 0.6f;
        public float DashCooldown;
        public float DashDuration = 0.2f;
        public bool NoGravity;

        public IStatsProvider Stat { get; private set; }

        private readonly int _hashAirAttack = Animator.StringToHash("AirAttack");
        private readonly int _hashAirborne = Animator.StringToHash("Airborne");

        // Parameters

        private readonly int _hashAirborneVerticalSpeed = Animator.StringToHash("AirborneVerticalSpeed");
        private readonly int _hashAngleDeltaRad = Animator.StringToHash("AngleDeltaRad");

        // Tags
        private readonly int _hashBlockInput = Animator.StringToHash("BlockInput");
        private readonly int _hashDeath = Animator.StringToHash("Death");
        private readonly int _hashEllenCombo1 = Animator.StringToHash("EllenCombo1");
        private readonly int _hashEllenCombo2 = Animator.StringToHash("EllenCombo2");
        private readonly int _hashEllenCombo3 = Animator.StringToHash("EllenCombo3");
        private readonly int _hashEllenCombo4 = Animator.StringToHash("EllenCombo4");

        private readonly int _hashEllenDeath = Animator.StringToHash("EllenDeath");
        private readonly int _hashFootFall = Animator.StringToHash("FootFall");
        private readonly int _hashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        private readonly int _hashGrounded = Animator.StringToHash("Grounded");
        private readonly int _hashHurt = Animator.StringToHash("Hurt");
        private readonly int _hashHurtFromX = Animator.StringToHash("HurtFromX");
        private readonly int _hashHurtFromY = Animator.StringToHash("HurtFromY");
        private readonly int _hashInputDetected = Animator.StringToHash("InputDetected");
        private readonly int _hashIsAirCombo = Animator.StringToHash("IsAirCombo");
        private readonly int _hashLanding = Animator.StringToHash("Landing");

        // States
        private readonly int _hashLocomotion = Animator.StringToHash("Locomotion");
        private readonly int _hashMeleeAttack = Animator.StringToHash("MeleeAttack");
        private readonly int _hashRespawn = Animator.StringToHash("Respawn");
        private readonly int _hashRightAttack = Animator.StringToHash("RightAttack");
        private readonly int _hashRoll = Animator.StringToHash("Roll");
        private readonly int _hashRootMotion = Animator.StringToHash("RootMotion");
        private readonly int _hashSkill1 = Animator.StringToHash("Skill1");
        private readonly int _hashSkill2 = Animator.StringToHash("Skill2");
        private readonly int _hashSkill3 = Animator.StringToHash("Skill3");
        private readonly int _hashSkill4 = Animator.StringToHash("Skill4");
        private readonly int _hashSkill5 = Animator.StringToHash("Skill5");
        private readonly int _hashSkill6 = Animator.StringToHash("Skill6");
        private readonly int _hashSkill7 = Animator.StringToHash("Skill7");
        private readonly int _hashSkill8 = Animator.StringToHash("Skill8");
        private readonly int _hashStateTime = Animator.StringToHash("StateTime");
        private readonly int _hashTimeoutToIdle = Animator.StringToHash("TimeoutToIdle");
        private readonly int _hashUpper = Animator.StringToHash("Upper");
        private readonly Collider[] _overlapResult = new Collider[8];
        private float _airAttackGraceTime;
        private bool _airSkillExecuted;

        private float _angleDiff;
        private Animator _animator;
        private CharacterController _characterController;

        private AnimatorStateInfo _currentStateInfo;
        private float _dashCooldownTimer;
        private float _dashSpeed;
        private float _dashTime;
        private float _desiredForwardSpeed;
        private float _forwardSpeed;
        private GroundCheck _groundCheck;
        private int _hashTriggerSkill1;
        private int _hashTriggerSkill2;
        private int _hashTriggerSkill3;
        private int _hashTriggerSkill4;
        private float _idleTimer;
        private bool _inAttack;
        private PlayerInput _input;
        private bool _isAirborneAttacking;
        // private bool _isAirCombo;
        private bool _isAnimatorTransitioning;
        private bool _isDashing;
        private bool _isGrounded;
        private bool _isRespawning;
        private AnimatorStateInfo _nextStateInfo;
        private PlayerSound _playerSound;
        private AnimatorStateInfo _previousCurrentStateInfo;
        private bool _previousIsAnimatorTransitioning;
        private bool _previouslyGrounded = true;
        private AnimatorStateInfo _previousNextStateInfo;
        private bool _readyToJump;
        private Renderer[] _renderers;
        private bool _rootMotionGroundedStart;
        

        private Quaternion _targetRotation;
        private float _verticalSpeed;
        private bool _wasInRootMotionState;
        private bool IsMoveInput => !Mathf.Approximately(_input.MoveInput.sqrMagnitude, 0f);
        private void Awake()
        {
            _input = GetComponent<PlayerInput>();
            _animator = GetComponent<Animator>();
            _characterController = GetComponent<CharacterController>();
            _groundCheck = GetComponent<GroundCheck>();
            _playerSound = GetComponent<PlayerSound>();
        }
        private async void Start()
        {
            Stat = await StatLoader.LoadFromCSVAsync("PlayerStat.csv");
            RefreshStat();
        }
        
        private void Update()
        {
            CacheAnimatorState();

            UpdateInputBlocking();

            EquipMeleeWeapon(IsPerformingAction());

            _animator.SetFloat(_hashStateTime, Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
            _animator.ResetTrigger(_hashMeleeAttack);
            _animator.ResetTrigger(_hashRightAttack);
            _animator.ResetTrigger(_hashRoll);


            if (_input.Attack && CanAttack)
            {
                _animator.SetTrigger(_hashMeleeAttack);
            }

            if (_input.RightAttack && CanAttack)
            {
                _animator.SetTrigger(_hashRightAttack);
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
                _dashSpeed = DashDistance / DashDuration;
                Vector3 dashDirection = transform.forward;
                Vector3 dashMove = dashDirection * _dashSpeed * Time.deltaTime;
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
            CheckIsAirCombo();
            SetIsAirCombo();
            bool isNowInRootMotion = IsRootMotion();

            if (isNowInRootMotion && !_wasInRootMotionState)
            {
                _rootMotionGroundedStart = _characterController.isGrounded;
            }

            _wasInRootMotionState = isNowInRootMotion;

            _previouslyGrounded = _isGrounded;

        }

        private void OnEnable()
        {
            EquipMeleeWeapon(false);

            _renderers = GetComponentsInChildren<Renderer>();
        }
        private void OnDisable()
        {
            for (int i = 0; i < _renderers.Length; ++i)
            {
                _renderers[i].enabled = true;
            }
        }

        private void OnAnimatorMove()
        {
            Vector3 movement;

            bool isAirSkill = IsAirSkill();
            bool isRootMotion = IsRootMotion();
            bool isAttacking = IsPerformingAction(); // 일반 공격 판정
            bool hasMovementInput = _input.MoveInput != Vector2.zero;

            if (_groundCheck.IsGrounded)
            {
                if (isAirSkill)
                {
                    movement = _animator.deltaPosition;

                    if (_animator.deltaPosition.y > 0.01f) // 루트모션에 Y 이동이 있을 때만
                    {
                        _isGrounded = false;
                    }
                }
                else
                {
                    RaycastHit hit;
                    Ray ray = new Ray(transform.position + Vector3.up * GroundedRayDistance * 0.5f, -Vector3.up);
                    if (Physics.Raycast(ray, out hit, GroundedRayDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                    {
                        movement = Vector3.ProjectOnPlane(_animator.deltaPosition, hit.normal);
                    }
                    else
                    {
                        if (isAttacking)
                        {
                            movement = hasMovementInput ? _animator.deltaPosition : Vector3.zero;
                        }
                        else
                        {
                            movement = _animator.deltaPosition;
                        }
                    }
                }
            }
            else
            {
                if (isAirSkill)
                {
                    movement = _animator.deltaPosition;
                }
                else
                {
                    movement = _forwardSpeed * transform.forward * Time.deltaTime;
                }
            }

            _characterController.transform.rotation *= _animator.deltaRotation;

            _isAirborneAttacking = !_groundCheck.IsGrounded && IsInCombo;
            bool ignoreGravity = IsAirSkill() || _isAirborneAttacking || _airAttackGraceTime > 0f;

            if (isRootMotion)
            {
                movement = new Vector3(_animator.deltaPosition.x * 1.5f, 0, _animator.deltaPosition.z * 1.5f);

                if (!_rootMotionGroundedStart)
                {
                    // 공중 시작 → 중력 적용
                    _verticalSpeed -= Gravity * 0.5f * Time.deltaTime;
                    movement += Vector3.up * (_animator.deltaPosition.y + _verticalSpeed * Time.deltaTime);
                }
                else
                {
                    // 지상 시작 → 중력 적용 없음
                    movement += Vector3.up * _animator.deltaPosition.y;
                }
            }
            else if (!ignoreGravity)
            {
                // 기본적으로 중력 적용
                movement += Vector3.up * (_verticalSpeed * Time.deltaTime);
            }


            _characterController.Move(movement);


            if (!_groundCheck.IsGrounded)
            {
                _animator.SetFloat(_hashAirborneVerticalSpeed, _verticalSpeed);
            }

            _animator.SetBool(_hashGrounded, _groundCheck.IsGrounded);
        }
        private void CheckIsAirCombo()
        {

            IsAirCombo = IsAirSkill() || _isAirborneAttacking;
        }

        public void ApplyEquipment(StatType statType, float value)
        {
            Stat = new StatModifierDecorator(Stat, statType, value);
            RefreshStat();
        }
        public void RemoveEquipment(StatType statType, float value)
        {
            Stat = new StatModifierDecorator(Stat, statType, -value);
            RefreshStat();
        }
        public void Dash()
        {
            _animator.SetTrigger(_hashRoll);
            _animator.CrossFade(_hashRoll, 0.05f);
            _isDashing = true;
            _dashTime = 0f;
        }


        private void CacheAnimatorState()
        {
            _previousCurrentStateInfo = _currentStateInfo;
            _previousNextStateInfo = _nextStateInfo;
            _previousIsAnimatorTransitioning = _isAnimatorTransitioning;

            _currentStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            _nextStateInfo = _animator.GetNextAnimatorStateInfo(0);
            _isAnimatorTransitioning = _animator.IsInTransition(0);
        }

        private void UpdateInputBlocking()
        {
            bool inputBlocked = _currentStateInfo.tagHash == _hashBlockInput && !_isAnimatorTransitioning;
            inputBlocked |= _nextStateInfo.tagHash == _hashBlockInput;
            _input.playerControllerInputBlocked = inputBlocked;
        }


        private bool IsPerformingAction()
        {
            // 지금 실행중인 애니메이션 이름을 참조해서 트루를 반환함, 스킬 사용 중일 때 참이 자동으로 되게 할거임
            bool equipped = _nextStateInfo.shortNameHash == _hashEllenCombo1 || _currentStateInfo.shortNameHash == _hashEllenCombo1;
            equipped |= _nextStateInfo.shortNameHash == _hashEllenCombo2 || _currentStateInfo.shortNameHash == _hashEllenCombo2;
            equipped |= _nextStateInfo.shortNameHash == _hashEllenCombo3 || _currentStateInfo.shortNameHash == _hashEllenCombo3;
            equipped |= _nextStateInfo.shortNameHash == _hashEllenCombo4 || _currentStateInfo.shortNameHash == _hashEllenCombo4;
            // 올려치기 공격
            equipped |= _nextStateInfo.shortNameHash == _hashUpper || _currentStateInfo.shortNameHash == _hashUpper;


            // 스킬 상태일 때 
            equipped |= _nextStateInfo.shortNameHash == _hashSkill1 || _currentStateInfo.shortNameHash == _hashSkill1;
            equipped |= _nextStateInfo.shortNameHash == _hashSkill2 || _currentStateInfo.shortNameHash == _hashSkill2;
            equipped |= _nextStateInfo.shortNameHash == _hashSkill3 || _currentStateInfo.shortNameHash == _hashSkill3;
            equipped |= _nextStateInfo.shortNameHash == _hashSkill4 || _currentStateInfo.shortNameHash == _hashSkill4;
            equipped |= _nextStateInfo.shortNameHash == _hashSkill5 || _currentStateInfo.shortNameHash == _hashSkill5;
            equipped |= _nextStateInfo.shortNameHash == _hashSkill6 || _currentStateInfo.shortNameHash == _hashSkill6;
            equipped |= _nextStateInfo.shortNameHash == _hashSkill7 || _currentStateInfo.shortNameHash == _hashSkill7;
            equipped |= _nextStateInfo.shortNameHash == _hashSkill8 || _currentStateInfo.shortNameHash == _hashSkill8;

            // 구르기 중일 때
            equipped |= _nextStateInfo.shortNameHash == _hashRoll || _currentStateInfo.shortNameHash == _hashRoll;

            return equipped;
        }

        // Called each physics step with a parameter based on the return value of IsWeaponEquiped.
        private void EquipMeleeWeapon(bool equip)
        {
            // meleeWeapon.gameObject.SetActive(equip);
            _inAttack = false;
            IsInCombo = equip;

            if (!equip)
                _animator.ResetTrigger(_hashMeleeAttack);
        }


        private void CalculateForwardMovement()
        {

            Vector2 moveInput = _input.MoveInput;
            if (IsAirCombo)
            {
                moveInput = Vector2.zero;
            }
            if (moveInput.sqrMagnitude > 1f)
            {
                moveInput.Normalize();
            }

            _desiredForwardSpeed = moveInput.magnitude * MaxForwardSpeed;

            float acceleration = IsMoveInput ? GroundAcceleration : GroundDeceleration;

            _forwardSpeed = Mathf.MoveTowards(_forwardSpeed, _desiredForwardSpeed, acceleration * Time.deltaTime);

            _animator.SetFloat(_hashForwardSpeed, _forwardSpeed);
        }
        private bool IsAirSkill()
        {
            return _currentStateInfo.tagHash == _hashAirAttack || _nextStateInfo.tagHash == _hashAirAttack;
        }

        private bool IsRootMotion()
        {
            return _currentStateInfo.tagHash == _hashRootMotion || _nextStateInfo.tagHash == _hashRootMotion;
        }
        public void TriggerAirSkillJump()
        {
            if (!_airSkillExecuted)
            {
                _verticalSpeed = 0;
                _isGrounded = false;
                _readyToJump = false;
                _airSkillExecuted = true;
                _airAttackGraceTime = MaxAirAttackGraceTime;
            }
        }
        private void CalculateVerticalMovement()
        {
            if (_airAttackGraceTime > 0f)
            {
                _airAttackGraceTime -= Time.deltaTime;
            }

            if (!_input.JumpInput && _groundCheck.IsGrounded)
            {
                _readyToJump = true;
            }

            if (!IsAirSkill())
            {
                _airSkillExecuted = false;
            }

            if (_groundCheck.IsGrounded)
            {
                _verticalSpeed = -Gravity * StickingGravityProportion;

                // 여기서 점프 실행
                if (_input.JumpInput && _readyToJump && !IsInCombo)
                {
                    _playerSound.Play(EPlayerState.Jump);
                    _verticalSpeed = JumpSpeed;
                    _readyToJump = false;
                }
            }
            else
            {
                if (_input.Attack)
                {
                    return;
                }

                if (!_input.JumpInput && _verticalSpeed > 0.0f)
                {
                    _verticalSpeed -= JumpAbortSpeed * Time.deltaTime;
                }

                if (Mathf.Approximately(_verticalSpeed, 0f))
                {
                    _verticalSpeed = 0f;
                }

                _isAirborneAttacking = !_groundCheck.IsGrounded && IsInCombo;
                bool ignoreGravity = IsAirSkill() || _isAirborneAttacking || _airAttackGraceTime > 0f;
                if (!ignoreGravity)
                {
                    _verticalSpeed -= Gravity * Time.deltaTime;
                }


            }
        }

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

            }

            Vector3 resultingForward = targetRotation * Vector3.forward;

            if (_inAttack)
            {
                Vector3 centre = transform.position + transform.forward * 2.0f + transform.up;
                Vector3 halfExtents = new Vector3(3.0f, 1.0f, 2.0f);
                int layerMask = 1 << LayerMask.NameToLayer("Enemy");
                int count = Physics.OverlapBoxNonAlloc(centre, halfExtents, _overlapResult, targetRotation, layerMask);

                float closestDot = 0.0f;
                Vector3 closestForward = Vector3.zero;
                int closest = -1;

                for (int i = 0; i < count; ++i)
                {
                    Vector3 playerToEnemy = _overlapResult[i].transform.position - transform.position;
                    playerToEnemy.y = 0;
                    playerToEnemy.Normalize();

                    float d = Vector3.Dot(resultingForward, playerToEnemy);

                    if (d > MinEnemyDotCoeff && d > closestDot)
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

            _angleDiff = Mathf.DeltaAngle(angleCurrent, targetAngle);
            _targetRotation = targetRotation;
        }


        private bool IsOrientationUpdated()
        {
            bool updateOrientationForLocomotion = !_isAnimatorTransitioning && _currentStateInfo.shortNameHash == _hashLocomotion || _nextStateInfo.shortNameHash == _hashLocomotion;
            bool updateOrientationForAirborne = !_isAnimatorTransitioning && _currentStateInfo.shortNameHash == _hashAirborne || _nextStateInfo.shortNameHash == _hashAirborne;
            bool updateOrientationForLanding = !_isAnimatorTransitioning && _currentStateInfo.shortNameHash == _hashLanding || _nextStateInfo.shortNameHash == _hashLanding;

            return updateOrientationForLocomotion || updateOrientationForAirborne || updateOrientationForLanding || IsInCombo && !_inAttack;
        }

        private void UpdateOrientation()
        {
            if (_isAirborneAttacking)
                return;

            _animator.SetFloat(_hashAngleDeltaRad, _angleDiff * Mathf.Deg2Rad);

            Vector3 localInput = new Vector3(_input.MoveInput.x, 0f, _input.MoveInput.y);
            float groundedTurnSpeed = Mathf.Lerp(MaxTurnSpeed, MinTurnSpeed, _forwardSpeed / _desiredForwardSpeed);
            float actualTurnSpeed = _isGrounded ? groundedTurnSpeed : Vector3.Angle(transform.forward, localInput) * InverseOneEighty * AirborneTurnSpeedProportion * groundedTurnSpeed;
            _targetRotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, actualTurnSpeed * Time.deltaTime);

            transform.rotation = _targetRotation;
        }
        private void TimeoutToIdle()
        {
            bool inputDetected = IsMoveInput || _input.Attack || _input.JumpInput || _input.RightAttack;
            if (_isGrounded && !inputDetected)
            {
                _idleTimer += Time.deltaTime;

                if (_idleTimer >= IdleTimeout)
                {
                    _idleTimer = 0f;
                    _animator.SetTrigger(_hashTimeoutToIdle);
                }
            }
            else
            {
                _idleTimer = 0f;
                _animator.ResetTrigger(_hashTimeoutToIdle);
            }

            _animator.SetBool(_hashInputDetected, inputDetected);
        }

        private void SetIsAirCombo()
        {
            _animator.SetBool(_hashIsAirCombo, IsAirCombo);
        }

        public void MeleeAttackStart(int throwing = 0)
        {
            _inAttack = true;
        }

        public void MeleeAttackEnd()
        {
            _inAttack = false;
        }


        public void RespawnFinished()
        {
            _isRespawning = false;

        }
        public void UseSkill(int skillNumber)
        {
            switch (skillNumber)
            {
                case 0:
                    _hashTriggerSkill1 = Animator.StringToHash(SkillManager.Instance.EquippedSkill1.SkillData.TriggerName);
                    _playerSound.Play(SkillManager.Instance.EquippedSkill1.SkillData.PlayerState);
                    _animator.SetTrigger(_hashTriggerSkill1);
                    break;
                case 1:
                    _hashTriggerSkill2 = Animator.StringToHash(SkillManager.Instance.EquippedSkill2.SkillData.TriggerName);
                    _playerSound.Play(SkillManager.Instance.EquippedSkill1.SkillData.PlayerState);
                    _animator.SetTrigger(_hashTriggerSkill2);
                    break;
                case 2:
                    _hashTriggerSkill3 = Animator.StringToHash(SkillManager.Instance.EquippedSkill3.SkillData.TriggerName);
                    _playerSound.Play(SkillManager.Instance.EquippedSkill1.SkillData.PlayerState);
                    _animator.SetTrigger(_hashTriggerSkill3);
                    break;
                case 3:
                    _hashTriggerSkill4 = Animator.StringToHash(SkillManager.Instance.EquippedSkill4.SkillData.TriggerName);
                    _playerSound.Play(SkillManager.Instance.EquippedSkill1.SkillData.PlayerState);
                    _animator.SetTrigger(_hashTriggerSkill4);
                    break;
            }

        }
        public void TakeDamage(Damage damage, bool ApproveAnimation)
        {

            if (ApproveAnimation)
                // Set the Hurt parameter of the animator.
            {
                _animator.SetTrigger(_hashHurt);
            }

            // Find the direction of the damage.
            Vector3 forward = damage.From.transform.position - transform.position;
            forward.y = 0f;

            Vector3 localHurt = transform.InverseTransformDirection(forward);

            // Set the HurtFromX and HurtFromY parameters of the animator based on the direction of the damage.
            _animator.SetFloat(_hashHurtFromX, localHurt.x);
            _animator.SetFloat(_hashHurtFromY, localHurt.z);

        }
        public void RefreshStat()
        {
            AttackPower = Stat.GetStat(StatType.AttackPower);
            MaxForwardSpeed = Stat.GetStat(StatType.Speed);
        }
    }

}
