using UnityEngine;

public class AttackState : BaseNormalEnemyState
{
    private float attackTimer;

    public override void OnEnter()
    {
        base.OnEnter();
        attackTimer = Owner.EnemyData.AttackCoolDown;
        Owner.NavMeshAgent.enabled = false;
    }

    public override void Update()
    {
        base.Update();

        float distance = Vector3.Distance(Owner.transform.position, Owner.Target.transform.position);
        if (distance >= Owner.EnemyData.AttackDistance)
        {
            SuperMachine.ChangeState<IdleState>();
            return;
        }

        attackTimer += Time.deltaTime;
        if (attackTimer >= Owner.EnemyData.AttackCoolDown)
        {
            Owner.Animator.SetTrigger("OnAttack");
            attackTimer = 0;
        }
    }
}
