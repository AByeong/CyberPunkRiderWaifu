using RobustFSM.Base;
using UnityEngine;

public class NormalStateMachine : MonoFSM<NormalEnemy>
{

    public Vector3 DownOffset;
    
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
