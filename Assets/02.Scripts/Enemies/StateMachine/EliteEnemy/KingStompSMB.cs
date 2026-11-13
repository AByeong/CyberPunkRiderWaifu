using UnityEngine;

public class KingStompSMB : StateMachineBehaviour
{
    public float ComboChance = 0.3f;
    public float Duration = 1.16f;
    private ElliteStateMachine _stateMachine;

    private float _timer;
    private bool _decided; // 콤보 여부는 상태당 한 번만 판정

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _stateMachine = animator.GetComponent<ElliteStateMachine>();
        _timer = 0;
        _decided = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (_decided) return;

        _timer += Time.deltaTime;
        if (_timer >= Duration)
        {
            _decided = true;

            // 공격 도중 피격/사망으로 FSM이 다른 상태로 넘어갔다면 건드리지 않는다
            if (!_stateMachine.IsCurrentState<EliteAttackState>()) return;

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
