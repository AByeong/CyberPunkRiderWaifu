using UnityEngine;

public class EliteIdleState : EliteBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();

        Owner.NavMeshAgent.enabled = true;
        Owner.Animator.SetTrigger("OnIdle");
    }

    public override void Update()
    {
        base.Update();

        float distance = Vector3.Distance(Owner.transform.position, Owner.Target.transform.position);
        if (distance <= Owner.EnemyData.AttackDistance)
        {
            Owner.AttackType = Random.Range(0, 2);
            Debug.Log("엘리트의 공격 패턴은" + Owner.AttackType + "입니다.");
            SuperMachine.ChangeState<EliteAttackState>();
            return;
        }
    }
}
