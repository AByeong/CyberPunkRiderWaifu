using UnityEngine;

public class IdleState : BaseNormalEnemyState
{

    public override void OnEnter()
    {
        base.OnEnter();

        Owner.NavMeshAgent.enabled = true;
        Owner.Animator.SetTrigger("OnIdle");
    }

    public override void Update()
    {
        if (CheckGlobalTransitions()) return;
        
        float distance = Vector3.Distance(Owner.Target.transform.position, Owner.transform.position);
        if (distance <= Owner.EnemyData.AttackDistance)
        {
            SuperMachine.ChangeState<AttackState>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
