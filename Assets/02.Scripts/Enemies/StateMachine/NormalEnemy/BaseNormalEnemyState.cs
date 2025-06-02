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
        if (Owner.CurrentHealthPoint <= 0 && !SuperMachine.IsCurrentState<DeadState>())
        {
            SuperMachine.ChangeState<DeadState>();
            return;
        }

        if (Owner.IsHit && !SuperMachine.IsCurrentState<DeadState>())
        {
            
            SuperMachine.ChangeState<HItState>();
            return;
        }
    }
}
