using JY;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    private const float _attackInputDuration = 0.03f;

    // It is advised you don't change them without fully understanding what they do in code.
    private const float k_AirborneTurnSpeedProportion = 5.4f;
    private const float k_GroundedRayDistance = 1f;
    private const float k_JumpAbortSpeed = 10f;
    private const float k_MinEnemyDotCoeff = 0.2f;
    private const float k_InverseOneEighty = 1f / 180f;
    private const float k_StickingGravityProportion = 0.3f;
    private const float k_GroundAcceleration = 20f;
    private const float k_GroundDeceleration = 25f;
    public static PlayerController instance;

    public float maxForwardSpeed = 8f; // How fast Ellen can run.
    public float gravity = 20f; // How fast Ellen accelerates downwards when airborne.
    public float jumpSpeed = 10f; // How fast Ellen takes off when jumping.
    public float minTurnSpeed = 400f; // How fast Ellen turns when moving at maximum speed.
    public float maxTurnSpeed = 1200f; // How fast Ellen turns when stationary.
    public float idleTimeout = 5f; // How long before Ellen starts considering random idles.
    public bool canAttack; // Whether or not Ellen can swing her staff.

    public Animator Animator;
    // public Inventory Inventory;
    // public Equipment Equipment;
    public CharacterController CharacterController;
    public bool respawning;
    private readonly int _hashInputDetected = Animator.StringToHash("InputDetected");

    // Animator Parameters
    private readonly int _hashMeleeAttack = Animator.StringToHash("MeleeAttack");
    private readonly int _hashStateTime = Animator.StringToHash("StateTime");
    private readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
    private readonly int m_HashGrounded = Animator.StringToHash("Grounded");
    private readonly int m_HashTimeoutToIdle = Animator.StringToHash("TimeoutToIdle");
    private WaitForSeconds _attackInputWait;

    private Coroutine _attackWaitCoroutine;
    private bool _isGrounded;
    // public PlayerSO PlayerData;
    // public PlayerStats Stats;
    private PlayerInput _playerInput;

    protected AnimatorStateInfo m_CurrentStateInfo; // Information about the base layer of the animator cached.
    protected float m_DesiredForwardSpeed;
    protected float m_ForwardSpeed;
    protected float m_IdleTimer;
    protected bool m_InCombo;
    protected bool m_IsAnimatorTransitioning;
    protected AnimatorStateInfo m_NextStateInfo;
    protected AnimatorStateInfo m_PreviousCurrentStateInfo; // Information about the base layer of the animator from last frame.
    protected bool m_PreviousIsAnimatorTransitioning;
    protected AnimatorStateInfo m_PreviousNextStateInfo;
    protected bool m_ReadyToJump;
    protected float m_VerticalSpeed;
    public object meleeWeapon;


    protected bool IsMoveInput => !Mathf.Approximately(_playerInput.MoveInput.sqrMagnitude, 0f);

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        _attackInputWait = new WaitForSeconds(_attackInputDuration);
        _playerInput = GetComponent<PlayerInput>();
        CharacterController = GetComponent<CharacterController>();
        instance = this;
    }
    public void Start()
    {
        // Stats = new PlayerStats(PlayerData);
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        CacheAnimatorState();

        Animator.SetFloat(_hashStateTime, Mathf.Repeat(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 2f));
        Animator.ResetTrigger(_hashMeleeAttack);

        if (_playerInput.Attack)
            Animator.SetTrigger(_hashMeleeAttack);

        CalculateForwardMovement();
        CalculateVerticalMovement();


        TimeoutToIdle();

    }

    private void OnAnimatorMove()
    {
        Vector3 movement;

        // If Ellen is on the ground...
        if (_isGrounded)
        {
            // ... raycast into the ground...
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * 1f * 0.5f, -Vector3.up);
            if (Physics.Raycast(ray, out hit, 1f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                // ... and get the movement of the root motion rotated to lie along the plane of the ground.
                movement = Vector3.ProjectOnPlane(Animator.deltaPosition, hit.normal);

                // Also store the current walking surface so the correct audio is played.
                Renderer groundRenderer = hit.collider.GetComponentInChildren<Renderer>();
                // m_CurrentWalkingSurface = groundRenderer ? groundRenderer.sharedMaterial : null;
            }
            else
            {
                // If no ground is hit just get the movement as the root motion.
                // Theoretically this should rarely happen as when grounded the ray should always hit.
                movement = Animator.deltaPosition;
                // m_CurrentWalkingSurface = null;
            }
        }
        else
        {
            // If not grounded the movement is just in the forward direction.
            movement = m_ForwardSpeed * transform.forward * Time.deltaTime;
        }

        // Rotate the transform of the character controller by the animation's root rotation.
        CharacterController.transform.rotation *= Animator.deltaRotation;

        // Add to the movement with the calculated vertical speed.
        movement += m_VerticalSpeed * Vector3.up * Time.deltaTime;

        // Move the character controller.
        CharacterController.Move(movement);

        // After the movement store whether or not the character controller is grounded.
        _isGrounded = CharacterController.isGrounded;

        // If Ellen is not on the ground then send the vertical speed to the animator.
        // This is so the vertical speed is kept when landing so the correct landing animation is played.
        // if (!_isGrounded)
        // Animator.SetFloat(m_HashAirborneVerticalSpeed, m_VerticalSpeed);

        // Send whether or not Ellen is on the ground to the animator.
        Animator.SetBool(m_HashGrounded, _isGrounded);
    }
    private void CacheAnimatorState()
    {
        m_PreviousCurrentStateInfo = m_CurrentStateInfo;
        m_PreviousNextStateInfo = m_NextStateInfo;
        m_PreviousIsAnimatorTransitioning = m_IsAnimatorTransitioning;

        m_CurrentStateInfo = Animator.GetCurrentAnimatorStateInfo(0);
        m_NextStateInfo = Animator.GetNextAnimatorStateInfo(0);
        m_IsAnimatorTransitioning = Animator.IsInTransition(0);
    }

    private void TimeoutToIdle()
    {
        bool inputDetected = _playerInput.Attack;
        if (_isGrounded && !inputDetected)
        {
            m_IdleTimer += Time.deltaTime;

            if (m_IdleTimer >= idleTimeout)
            {
                m_IdleTimer = 0f;
                Animator.SetTrigger(m_HashTimeoutToIdle);
            }
        }
        else
        {
            m_IdleTimer = 0f;
            Animator.ResetTrigger(m_HashTimeoutToIdle);
        }

        Animator.SetBool(_hashInputDetected, inputDetected);
    }

    private void CalculateVerticalMovement()
    {
        // If jump is not currently held and Ellen is on the ground then she is ready to jump.
        if (!_playerInput.JumpInput && _isGrounded)
            m_ReadyToJump = true;

        if (_isGrounded)
        {
            // When grounded we apply a slight negative vertical speed to make Ellen "stick" to the ground.
            m_VerticalSpeed = -gravity * k_StickingGravityProportion;

            // If jump is held, Ellen is ready to jump and not currently in the middle of a melee combo...
            if (_playerInput.JumpInput && m_ReadyToJump && !m_InCombo)
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
            if (!_playerInput.JumpInput && m_VerticalSpeed > 0.0f)
            {
                // ... decrease Ellen's vertical speed.
                // This is what causes holding jump to jump higher that tapping jump.
                m_VerticalSpeed -= k_JumpAbortSpeed * Time.deltaTime;
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

    private void CalculateForwardMovement()
    {
        // Cache the move input and cap it's magnitude at 1.
        Vector2 moveInput = _playerInput.MoveInput;
        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        // Calculate the speed intended by input.
        m_DesiredForwardSpeed = moveInput.magnitude * maxForwardSpeed;

        // Determine change to speed based on whether there is currently any move input.
        float acceleration = IsMoveInput ? k_GroundAcceleration : k_GroundDeceleration;

        // Adjust the forward speed towards the desired speed.
        m_ForwardSpeed = Mathf.MoveTowards(m_ForwardSpeed, m_DesiredForwardSpeed, acceleration * Time.deltaTime);

        // Set the animator parameter to control what animation is being played.
        Animator.SetFloat(m_HashForwardSpeed, m_ForwardSpeed);
    }
}
