using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "NavMeshON", story: "Switch [Agent] NavMesh On", category: "Action", id: "52e673bbd63e4231b427b794fce93c4d")]
public partial class NavMeshOnAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Agent;

    protected override Status OnStart()
    {
        Agent.Value.NavMeshAgent.enabled = true;
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

