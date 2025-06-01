using UnityEngine;

public class AttackWhileAnimate : StateMachineBehaviour
{
    private EliteEnemy _owner;
    private ElliteStateMachine _stateMachine;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _owner = animator.GetComponent<EliteEnemy>();
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

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        _owner.AttackTimer = 0;
    }

}
