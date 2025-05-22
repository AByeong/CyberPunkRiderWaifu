using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RenturnToPool", story: "[Agent] Return to Pool", category: "Action", id: "4a79ff8a142653e6bd74b77a86fabb29")]
public partial class RenturnToPoolAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Agent;

    protected override Status OnStart()
    {
        //풀링에 반환
        
            Agent.Value.Pool.ReturnObject(this.GameObject);
        
        
        
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

