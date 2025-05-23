using RobustFSM.Base;

public class NormalStateMachine : MonoFSM<NormalEnemy>
{
    public override void AddStates()
    {
        AddState<IdleState>();
        AddState<AttackState>();
        AddState<HItState>();
        AddState<DownedState>();
        AddState<DeadState>();

        SetInitialState<IdleState>();
    }
}
