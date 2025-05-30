using UnityEngine;

public class EliteIdleState : EliteBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Owner.NavMeshAgent.enabled = true;
        Owner.Animator.SetTrigger("OnIdle");

        if (Owner.Target != null)
        {
            Owner.Target = GameManager.Instance.player.gameObject;
        }
    }

    public override void Update()
    {
        Owner.AttackTimer += Time.deltaTime;
        base.Update();

        if (Owner.Target == null)
        {
            Owner.Target = GameManager.Instance.player.gameObject;
        }

        float distance = Vector3.Distance(Owner.transform.position, Owner.Target.transform.position);
        Debug.Log($"{Owner.gameObject.name}이 적과의 거리 : {distance},플레이어의 위치 {Owner.Target.transform.position} ,어택까지 남은 거리 : {distance <= Owner.EnemyData.AttackDistance}");
        
        
        if (distance <= Owner.EnemyData.AttackDistance)
        {
            Owner.AttackType = Random.Range(0, 3);
            Debug.Log($"{Owner.AttackType}의 패턴으로 공격합니다.");
            SuperMachine.ChangeState<EliteAttackState>();
            return;
        }
    }
}