using System.Collections.Generic;
using UnityEngine;

public class EliteDeadState : EliteBaseState
{
    private float _deadTimer;

    public override void OnEnter()
    {
        base.OnEnter();

        Owner.NavMeshAgent.enabled = false;
        // Owner.Animator.SetFloat("DeadType", Random.Range(0, 3));
        // Owner.Animator.SetTrigger("OnDead");
        Owner.Animator.SetFloat("DownType", Random.Range(0, 3));
        Owner.Animator.SetTrigger("OnDown");
        Owner.Collider.enabled = false;

        switch (Owner.EnemyData.EnemyType)
        {
            case EEnemyType.Elite:
                DeliveryManager.Instance.KillTracker.IncreaseKillCount(EnemyType.Elite);
                break;

            case EEnemyType.Boss:
                DeliveryManager.Instance.KillTracker.IncreaseKillCount(EnemyType.Boss);
                break;
        }

        Dictionary<DropItemType, int> dropPlan = Owner.GetDrops();
        ItemDropManager.Instance.DropItems(dropPlan, transform.position, transform.forward);
    }

    public override void Update()
    {
        base.Update();
        _deadTimer += Time.deltaTime;
        if (_deadTimer >= 3f)
        {
            _deadTimer = 0f;
            DeliveryManager.Instance.KillCount++;
            Owner.Pool.ReturnObject(Owner.gameObject);
            SuperMachine.ChangeState<EliteIdleState>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
