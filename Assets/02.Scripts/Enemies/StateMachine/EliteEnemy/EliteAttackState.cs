using UnityEngine;

public class EliteAttackState : EliteBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Owner.NavMeshAgent.enabled = false;
        Owner.AttackType = Random.Range(0, Owner.AttackTypeNumber);
        Owner.Animator.SetBool("IsAttack", true);
        Debug.Log($"{Owner.AttackType}의 패턴으로 공격합니다.");
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
    }


}
