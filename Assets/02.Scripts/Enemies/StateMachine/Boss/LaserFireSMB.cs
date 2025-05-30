using UnityEngine;

public class LaserFireSMB : StateMachineBehaviour
{
    private BossPhase1 _bossPhase1;
    private ElliteStateMachine _stateMachine;

    public float LaserDuration;
    public float PreDelay;
    private float _laserTimer;

    private bool _isLasing = false;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        _bossPhase1 = animator.GetComponent<BossPhase1>();
        _stateMachine = _bossPhase1.GetComponent<ElliteStateMachine>();
        _laserTimer = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        _laserTimer += Time.deltaTime;
        if (_laserTimer >= PreDelay && !_isLasing)
        {
            _isLasing = true;
            _bossPhase1.StartLaser();
        }

        if (_laserTimer >= LaserDuration + PreDelay)
        {
            _laserTimer = 0;
            _bossPhase1.LaserEnd();
            _stateMachine.ChangeState<EliteIdleState>();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        _bossPhase1.AttackTimer = 0;
        _laserTimer = 0;
        _isLasing = false;
    }
}
