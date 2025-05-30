using UnityEngine;

public class EliteAttackState : EliteBaseState
{
    private float _attackTimer;

    private bool _isAttacking = false;
    public override void OnEnter()
    {
        base.OnEnter();
        Owner.NavMeshAgent.enabled = false;

        _attackTimer = Owner.EnemyData.AttackCoolDown;
        Owner.AttackType = Random.Range(0, 3);
    }

    public void AttackStart()
    {
        _isAttacking = true;
    }

    public void AttackEnd()
    {
        _isAttacking = false;
    }
    
    public override void Update()
    {
        base.Update();

        float distance = Vector3.Distance(Owner.transform.position, Owner.Target.transform.position);
        if (distance >= Owner.EnemyData.AttackDistance && !_isAttacking)
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
