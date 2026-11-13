using RobustFSM.Base;

public class BaseNormalEnemyState : MonoState
{
    public NormalEnemy Owner
    {
        get
        {
            return ((NormalStateMachine)SuperMachine).Owner;
        }
    }

    public virtual void Update()
    {
        CheckGlobalTransitions();
    }

    // HP 0 → 사망, 피격 → 피격 상태로 전이. 전이했으면 true를 반환하므로 파생 상태는 즉시 return 해야 한다.
    protected bool CheckGlobalTransitions()
    {
        if (Owner.CurrentHealthPoint <= 0 && !SuperMachine.IsCurrentState<DeadState>())
        {
            SuperMachine.ChangeState<DeadState>();
            return true;
        }

        if (Owner.IsHit && !SuperMachine.IsCurrentState<DeadState>())
        {
            SuperMachine.ChangeState<HItState>();
            return true;
        }

        return false;
    }
}
