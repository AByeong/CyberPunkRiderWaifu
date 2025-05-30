using UnityEngine;
using UnityEngine.Serialization;

public class EliteAttackState : EliteBaseState
{    
    public override void OnEnter()
    {
        base.OnEnter();
        Owner.NavMeshAgent.enabled = false;

        
    }

    
    public void IsAttacking()
    {
        Owner.IsAttacking = true;

    }
    
    public override void Update()
    {
        base.Update();

        
        
        
        float distance = Vector3.Distance(Owner.transform.position, Owner.Target.transform.position);
        
        //Debug.Log($"{distance >= Owner.EnemyData.AttackDistance} ||| {!Owner.IsAttacking}\n{distance >= Owner.EnemyData.AttackDistance&& !Owner.IsAttacking}지금은 어택 상태에서 빠져나올 수 있습니다.");
        
        
        if (distance >= Owner.EnemyData.AttackDistance && !Owner.IsAttacking)
        {
            SuperMachine.ChangeState<EliteIdleState>();
            return;
        }


        
        if (Owner.AttackTimer >= Owner.EnemyData.AttackCoolDown && !Owner.IsAttacking)
        {
            Owner.AttackType = Random.Range(0,Owner.AttackTypeNumber-1);

            // if (Owner.EnemyData.EnemyType == EEnemyType.Waifu)
            // {
            //     if (Owner.AttackType == Owner.AttackTypeNumber - 1)//맨 마지막 공격이라면
            //     {
            //         //10프로의 확률로 필살기가 날아갑니다.
            //         int random = Random.Range(0, 100);
            //         if (random >= 10)
            //         {
            //             Owner.AttackType = Random.Range(0,Owner.AttackTypeNumber-2);
            //         }
            //         
            //     }
            // }
            
            
            Owner.Animator.SetTrigger("OnAttack");
        }
        
        
    }

    
}
