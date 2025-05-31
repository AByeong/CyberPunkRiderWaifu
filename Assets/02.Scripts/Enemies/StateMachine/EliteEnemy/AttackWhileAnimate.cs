using UnityEngine;

public class AttackWhileAnimate : StateMachineBehaviour
{
    private ElliteStateMachine _stateMachine;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _stateMachine = animator.GetComponent<ElliteStateMachine>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (stateInfo.normalizedTime >= 0.9f)
        {
            _stateMachine.ChangeState<EliteIdleState>();
        }
    }

}
