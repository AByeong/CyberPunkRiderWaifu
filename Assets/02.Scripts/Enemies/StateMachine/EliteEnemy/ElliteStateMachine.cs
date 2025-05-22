using RobustFSM.Base;

public class ElliteStateMachine : MonoFSM<EliteEnemy>
{
    public override void AddStates()
    {
        AddState<EliteIdleState>();
        AddState<EliteAttackState>();
        AddState<EliteHitState>();
        AddState<EliteDownState>();
        AddState<EliteDeadState>();

        SetInitialState<EliteIdleState>();
    }
}
