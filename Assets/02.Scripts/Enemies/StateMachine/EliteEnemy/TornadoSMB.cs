using UnityEngine;

public class TornadoSMB : StateMachineBehaviour
{
    private ElliteStateMachine _stateMachine;
    private EliteAI_Female _eliteFemale;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _eliteFemale = animator.GetComponent<EliteAI_Female>();
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
        _eliteFemale.TornadoAttackEnd();
    }
}
