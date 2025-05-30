using System;
using UnityEngine;

public class Boss_Waifu_AI : EliteEnemy
{

    [SerializeField] private float runCooldownDuration = 3.0f;
    [SerializeField] private Collider _swordCollider;
    private bool isCoolingDown = false;
    private float currentCooldownTime = 0f;
private Animator animator;
    public  void AttackStart()
    {
        IsAttacking = true;
    }

    public  void AttackEnd()
    {
        
        IsAttacking = false;
        
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
 
    




public void JumpAttack()
{
    //animator.applyRootMotion = true;
    SoundManager.Instance.Play(SoundType.Elite_Female_KingStamp);
    _eliteStateMachine.ChangeState<EliteAttackState>();
  
}


    public void Nanmu()
    {
        
        Debug.Log("난무 시작");
        SoundManager.Instance.Play(SoundType.Elite_Electricity);
        LookAtTarget();

        _eliteStateMachine.ChangeState<EliteAttackState>();
        
    }



    public void SpinAttack()
    {
        SoundManager.Instance.Play(SoundType.Elite_Female_Tornado);
        LookAtTarget();

        _eliteStateMachine.ChangeState<EliteAttackState>();
        
    }


    public void Spell()
    {
        animator.applyRootMotion = true;
    }

    public void SpellEnd()
    {
        animator.applyRootMotion = false;
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
        _swordCollider.enabled = true;
    }

    public void ColliderOff()
    {
        _swordCollider.enabled = false;
    }


    
    
    
    
    
}
