using UnityEngine;

public class EliteDownState : EliteBaseState
{
    private float _downTimer;

    public override void OnEnter()
    {
        base.OnEnter();
        
        Owner.NavMeshAgent.enabled = false;
        Owner.Animator.SetTrigger("OnDown");
    }

    public override void Update()
    {
        base.Update();
        _downTimer += Time.deltaTime;
        if (_downTimer >= Owner.EnemyData.DownTime)
        {
            Owner.Animator.SetTrigger("OnStand");
            SuperMachine.ChangeState<EliteIdleState>();
            return;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        _downTimer = 0f;
    }
}
