using UnityEngine;

public class KingStompSMB : StateMachineBehaviour
{
    public float ComboChance = 0.3f;
    public float Duration = 1.16f;
    private ElliteStateMachine _stateMachine;

    private float _timer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _stateMachine = animator.GetComponent<ElliteStateMachine>();
        _timer = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        _timer += Time.deltaTime;
        if (_timer >= Duration)
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
