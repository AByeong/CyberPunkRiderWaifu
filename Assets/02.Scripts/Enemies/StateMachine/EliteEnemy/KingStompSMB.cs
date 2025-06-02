using UnityEngine;

public class KingStompSMB : StateMachineBehaviour
{
    public float ComboChance = 0.3f;
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
            if (Random.Range(0f, 1f) <= ComboChance)
            {
                animator.SetTrigger("OnCombo");
            }
            else
            {
                _stateMachine.ChangeState<EliteIdleState>();
            }
        }
    }
}
