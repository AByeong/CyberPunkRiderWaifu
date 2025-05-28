using UnityEngine;

public class EliteAttackState : EliteBaseState
{
    private float _attackTimer;

    public override void OnEnter()
    {
        base.OnEnter();
        _attackTimer = Owner.EnemyData.AttackCoolDown;
        // Owner.NavMeshAgent.isStopped = true;
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

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= Owner.EnemyData.AttackCoolDown)
        {
            Owner.Animator.SetTrigger("OnAttack");
            _attackTimer = 0;
        }
    }

    
}
