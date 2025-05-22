using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RandonType", story: "[attacktype]", category: "Action", id: "34abcbbe91e47e36a89cb04407587125")]
public partial class RandonTypeAction : Action
{
    [SerializeReference] public BlackboardVariable<EliteEnemy> Attacktype;
    protected override Status OnStart()
    {
        
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

