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

        // 🔄 타겟 바라보기
        Vector3 direction = Owner.Target.transform.position - Owner.transform.position;
        direction.y = 0; // 수평 회전만
        if (direction.sqrMagnitude > 0.01f) // 방향이 0이 아닐 때만 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Owner.transform.rotation = Quaternion.Slerp(Owner.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

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
            if (distance < Owner.EnemyData.AttackDistance)
            {
                Damage damage = new Damage();
                damage.DamageValue = Owner.EnemyData.Damage;
                Owner.Target.GetComponent<PlayerHit>().TakeDamage(damage);
            }
        }
    }

}
