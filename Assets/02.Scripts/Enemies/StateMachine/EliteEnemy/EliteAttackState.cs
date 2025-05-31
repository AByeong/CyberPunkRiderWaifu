using UnityEngine;
using UnityEngine.Serialization;

public class EliteAttackState : EliteBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Owner.NavMeshAgent.enabled = false;
        if (Owner.AttackType == -1)
        {
            Owner.AttackType = Random.Range(0,Owner.AttackTypeNumber-1);
        }

    }



    public override void Update()
    {
        base.Update();

        
        
        
        float distance = Vector3.Distance(Owner.transform.position, Owner.Target.transform.position);
        
        //Debug.Log($"{distance >= Owner.EnemyData.AttackDistance} ||| {!Owner.IsAttacking}\n{distance >= Owner.EnemyData.AttackDistance&& !Owner.IsAttacking}지금은 어택 상태에서 빠져나올 수 있습니다.");
        
        
        if (distance >= Owner.EnemyData.AttackDistance && !Owner.IsAttacking)
        {
            SuperMachine.ChangeState<EliteIdleState>();
            return;
        }


        if (Owner.AttackTimer >= Owner.EnemyData.AttackCoolDown && !Owner.IsAttacking)
        {
            
            
            
            Owner.Animator.SetTrigger("OnAttack");
        }
    }


}
