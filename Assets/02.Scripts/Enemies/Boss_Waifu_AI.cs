using System;
using UnityEngine;

public class Boss_Waifu_AI : EliteEnemy
{

    [SerializeField] private float runCooldownDuration = 3.0f;
    [SerializeField] private Collider _swordCollider;
    private bool isCoolingDown = false;
    private float currentCooldownTime = 0f;
    private Animator animator;
    [SerializeField] private CyberKatanaAttack _katana;

    // Add a serialized field for attack range visualization
    [SerializeField] private float debugAttackRange = 0f;

    public void SpinAttack()
    {
        Attack(20, 10);
        debugAttackRange = 5f; // Set for Gizmo visualization
    }

    public void FogAttack()
    {
        Attack(20, 20);
        debugAttackRange = 10f; // Set for Gizmo visualization
    }

    public void SlashAttack()
    {
        Attack(20, 10);
        debugAttackRange = 5f; // Set for Gizmo visualization
    }
    
    public void ResetIsAttackAndTimer()
    {
        ResetAttackTimer();
    }

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        
        if (isCoolingDown)
        {
            currentCooldownTime -= Time.deltaTime;
            if (currentCooldownTime <= 0)
            {
                isCoolingDown = false;
                currentCooldownTime = 0f;
            }
        }
    }

    private void LookAtTarget()
    {
        Vector3 direction = (GameManager.Instance.player.transform.position - transform.position).normalized;
        direction.y = 0f; // 수평만 회전하도록
        transform.forward = direction;
    }

    private void Attack(float range, int damageValue)
    {
        Vector3 sphereCenter = this.transform.position;
        Collider[] detectedColliders = Physics.OverlapSphere(sphereCenter, range);
        
        //SoundManager.Instance.Play(SoundType.Elite_Female_Step);
        LookAtTarget();
        
        foreach (Collider hitCollider in detectedColliders)
        {
            if (hitCollider.tag == "Elite" || hitCollider.tag == "Boss") continue;

            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Damage damage = new Damage();
                damage.DamageValue = 0;
                damage.DamageType = EDamageType.Airborne;
                damage.DamageForce = 2f;
                damage.From = this.gameObject;
                damageable.TakeDamage(damage);
            }
            // else if(damageable != null && hitCollider.tag == "Player")
            // {
            //     Damage damage = new Damage();
            //     damage.DamageValue = damageValue;
            //     damage.DamageType = EDamageType.Normal;
            //     damage.DamageForce = 5f;
            //     damage.From = this.gameObject;
            //     damageable.TakeDamage(damage);
            // }
        }
    }
    
    public void Running()
    {
        if (!isCoolingDown)
        {
            _animator.SetBool("Running", true);
            if(_collider != null) _collider.enabled = false; // 콜라이더가 있는 경우에만 비활성화
        }
    }

    public void NotRunning()
    {
        _animator.SetBool("Running", false);
        if(_collider != null) _collider.enabled = true; // 콜라이더가 있는 경우에만 활성화
        isCoolingDown = true;
        currentCooldownTime = runCooldownDuration;
        animator.applyRootMotion = false;
    }

    public void ColliderOn()
    {
        Debug.Log("ColliderOn");
        _swordCollider.enabled = true;
    }

    public void ColliderOff()
    {
        Debug.Log("ColliderOff");
        _swordCollider.enabled = false;
    }

    // --- Gizmo Visualization ---
    private void OnDrawGizmos()
    {
        // Only draw the Gizmo if the game is not playing, or if you specifically want to see it during runtime.
        // For debugging attack ranges, it's often most useful in the editor.
        if (!Application.isPlaying) 
        {
            Gizmos.color = Color.red;
            // Draw a wire sphere at the current object's position with the debugAttackRange radius
            Gizmos.DrawWireSphere(transform.position, debugAttackRange);
        }
        else
        {
            // You can uncomment the following lines if you want to see the Gizmo while playing
            // Gizmos.color = Color.yellow;
            // Gizmos.DrawWireSphere(transform.position, debugAttackRange);
        }
    }
}