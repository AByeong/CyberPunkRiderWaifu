using RobustFSM.Base;

public class EliteBaseState : MonoState
{
    public EliteEnemy Owner
    {
        get
        {
            return ((ElliteStateMachine)SuperMachine).Owner;
        }
    }

    public virtual void Update()
    {
        CheckGlobalTransitions();
    }

    // HP 0 → 사망, 피격 → 피격 상태로 전이. 전이했으면 true를 반환하므로 파생 상태는 즉시 return 해야 한다.
    protected bool CheckGlobalTransitions()
    {
        if (Owner.CurrentHealthPoint <= 0 && !SuperMachine.IsCurrentState<EliteDeadState>())
        {
            SuperMachine.ChangeState<EliteDeadState>();
            return true;
        }

        if (Owner.IsHit && !SuperMachine.IsCurrentState<EliteDeadState>())
        {
            if (SuperMachine.IsCurrentState<EliteAttackState>())
            {
                Owner.IsHit = false;
                return false;
            }

            SuperMachine.ChangeState<EliteHitState>();
            return true;
        }

        return false;
    }
}
