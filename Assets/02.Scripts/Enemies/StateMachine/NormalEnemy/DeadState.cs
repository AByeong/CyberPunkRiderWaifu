using System.Collections.Generic;
using UnityEngine;

public class DeadState : BaseNormalEnemyState
{
    private float _deadTimer;

    public override void OnEnter()
    {
        base.OnEnter();

        Owner.NavMeshAgent.enabled = false;
        Owner.Animator.SetFloat("DeadType", Random.Range(0, 3));
        Owner.Animator.SetTrigger("OnDead");
        Owner.GetComponent<Collider>().enabled = false;

        switch (Owner.EnemyData.EnemyType)
        {
            case EEnemyType.Normal:
                DeliveryManager.Instance.KillTracker.IncreaseKillCount(EnemyType.Normal);
                var dropPlan = new Dictionary<DropItemType, int>
                {
                    
                    { DropItemType.Gold, 1 }
                };
                ItemDropManager.Instance.DropItems(dropPlan, transform.position, transform.forward);
                break;

            case EEnemyType.Elite:
                DeliveryManager.Instance.KillTracker.IncreaseKillCount(EnemyType.Elite);
                break;

            case EEnemyType.Boss:
                DeliveryManager.Instance.KillTracker.IncreaseKillCount(EnemyType.Boss);
                break;
        }
    }

    public override void Update()
    {
        base.Update();
        _deadTimer += Time.deltaTime;
        if (_deadTimer >= 3f)
        {
            Owner.NavMeshAgent.ResetPath();
            Owner.NavMeshAgent.velocity = Vector3.zero;
            Owner.Pool.ReturnObject(Owner.gameObject);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        _deadTimer = 0f;
    }
}
