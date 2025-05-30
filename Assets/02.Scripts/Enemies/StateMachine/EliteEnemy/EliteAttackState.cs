using UnityEngine;
using UnityEngine.Serialization;

public class EliteAttackState : EliteBaseState
{    
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

        
        if (Owner.AttackTimer >= Owner.EnemyData.AttackCoolDown && !Owner.IsAttacking)
        {
            Owner.IsAttacking = true;
            Owner.AttackType = Random.Range(0, 3);
            Owner.Animator.SetTrigger("OnAttack");
        }
        
        
    }

    
}
