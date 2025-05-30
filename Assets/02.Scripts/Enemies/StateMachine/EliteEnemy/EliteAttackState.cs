using UnityEngine;

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
        if (distance >= Owner.EnemyData.AttackDistance)
        {
            SuperMachine.ChangeState<EliteIdleState>();
            return;
        }


        if (Owner.AttackTimer >= Owner.EnemyData.AttackCoolDown)
        {
            Owner.AttackType = Random.Range(0, Owner.AttackTypeNumber);
            Owner.Animator.SetTrigger("OnAttack");
        }
    }


}
