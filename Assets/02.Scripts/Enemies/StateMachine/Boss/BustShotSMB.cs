using UnityEngine;

public class BustShotSMB : StateMachineBehaviour
{
    private BossPhase1 _bossPhase1;
    private ElliteStateMachine _stateMachine;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        _bossPhase1 = animator.GetComponent<BossPhase1>();
        _stateMachine = _bossPhase1.GetComponent<ElliteStateMachine>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (stateInfo.normalizedTime >= 0.9f)
        {
            _stateMachine.ChangeState<EliteIdleState>();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        _bossPhase1.AttackTimer = 0;
    }
}
