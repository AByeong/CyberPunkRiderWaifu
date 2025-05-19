using System;
using Unity.Behavior;

[BlackboardEnum]
public enum EEnemyState
{
	Idle,
    Attack,
	Hit,
	InAir,
	Downed,
	Dead
}
