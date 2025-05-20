using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Die", story: "[Agent] Dead", category: "Action", id: "b32a11392f6b37ff554c515d0a64df96")]
public partial class DieAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private Animator _animator;

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

