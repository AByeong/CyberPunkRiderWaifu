using UnityEngine;

public class OnAttackBehaviour : StateMachineBehaviour
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

        if (stateInfo.normalizedTime >= 1f && _stateMachine.IsCurrentState<EliteAttackState>())
        {
            _stateMachine.ChangeState<EliteIdleState>();
        }
    }

    
}
