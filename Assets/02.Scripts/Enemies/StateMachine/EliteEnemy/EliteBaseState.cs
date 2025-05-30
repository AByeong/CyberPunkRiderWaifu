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
        if (Owner.CurrentHealthPoint <= 0 && !SuperMachine.IsCurrentState<EliteDeadState>())
        {
            SuperMachine.ChangeState<EliteDeadState>();
            return;
        }

        if (Owner.IsHit && !SuperMachine.IsCurrentState<EliteDeadState>())
        {
            if (SuperMachine.IsCurrentState<EliteAttackState>())
            {
                Owner.IsHit = false;
                return;    
            }

            SuperMachine.ChangeState<EliteHitState>();
            return;
        }

    }
}
