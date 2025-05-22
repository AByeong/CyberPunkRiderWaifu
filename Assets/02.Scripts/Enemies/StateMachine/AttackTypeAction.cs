using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackType", story: "[attackPattern]", category: "Action", id: "873a09f81a74d00bf675cb5aa63f1ddd")]
public partial class AttackTypeAction : Action
{
    [SerializeReference] public BlackboardVariable<EliteEnemy> AttackPattern;

    protected override Status OnStart()
    {
        AttackPattern.Value.AttackType = Random.Range(0, 2);
        Debug.Log("엘리트의 공격 패턴은" + AttackPattern.Value.AttackType + "입니다.");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

