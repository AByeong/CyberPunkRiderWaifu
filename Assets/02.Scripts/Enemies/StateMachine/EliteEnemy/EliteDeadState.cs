using UnityEngine;

public class EliteDeadState : EliteBaseState
{
    private float _deadTimer;

    public override void OnEnter()
    {
        base.OnEnter();

        Owner.NavMeshAgent.enabled = false;
        Owner.Animator.SetTrigger("OnDown");
        Owner.GetComponent<Collider>().enabled = false;

        switch (Owner.EnemyData.EnemyType)
        {
            case EEnemyType.Normal:
                DeliveryManager.Instance.KillTracker.IncreaseKillCount(EnemyType.Normal);
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
            Owner.Pool.ReturnObject(Owner.gameObject);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        _deadTimer = 0f;
    }
}
