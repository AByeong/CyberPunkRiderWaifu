using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.Mathematics;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FallDown", story: "[Agent] Fall Down during [time]", category: "Action", id: "489506b19253f5437376072eb11a4d8e")]
public partial class FallDownAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Agent;
    [SerializeReference] public BlackboardVariable<float> FallTime;

    private float timer;


    protected override Status OnStart()
    {

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        timer += Time.deltaTime;
        math.lerp(Agent.Value.position.y, 0, timer/FallTime);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

