using UnityEngine;

public class EliteIdleState : EliteBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Owner.NavMeshAgent.enabled = true;
        Owner.Animator.SetBool("IsAttack", false);
        Owner.AttackTimer = 0;
    }

    public override void Update()
    {
        base.Update();

        float distance = Vector3.Distance(Owner.transform.position, Owner.Target.transform.position);
        if (distance < Owner.EnemyData.AttackDistance && Owner.AttackTimer >= Owner.EnemyData.AttackCoolDown)
        {
            SuperMachine.ChangeState<EliteAttackState>();
            return;
        }
    }
}