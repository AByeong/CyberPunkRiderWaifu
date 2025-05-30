using UnityEngine;

public class EliteIdleState : EliteBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Owner.IsAttacking = false;
        Owner.NavMeshAgent.enabled = true;
        Owner.Animator.SetTrigger("OnIdle");
    }

    public override void Update()
    {
        base.Update();

        float distance = Vector3.Distance(Owner.transform.position, Owner.Target.transform.position);
        if (distance <= Owner.EnemyData.AttackDistance)
        {
            Owner.AttackType = Random.Range(0, 3);
            SuperMachine.ChangeState<EliteAttackState>();
            return;
        }
    }
}