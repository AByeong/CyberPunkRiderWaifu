using UnityEngine;
using UnityEngine.Serialization;

public class EliteAttackState : EliteBaseState
{
    public float _attackTimer;

    
    
    public override void OnEnter()
    {
        base.OnEnter();
        Owner.NavMeshAgent.enabled = false;

        
    }

    
    
    public override void Update()
    {
        base.Update();

        
        float distance = Vector3.Distance(Owner.transform.position, Owner.Target.transform.position);
        

        if (distance >= Owner.EnemyData.AttackDistance && !Owner.IsAttacking)
        {
            SuperMachine.ChangeState<EliteIdleState>();
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= Owner.EnemyData.AttackCoolDown)
        {
            Owner.Animator.SetTrigger("OnAttack");
            Owner.AttackTimer = 0;
        }
        
        
    }

    
}
