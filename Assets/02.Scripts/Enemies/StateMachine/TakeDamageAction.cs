using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TakeDamage", story: "[Agent] [HealthPoint] Take [DamageValue] [DamageForce]", category: "Action", id: "dd16ce5c85a8cf6de6e63cd777901454")]
public partial class TakeDamageAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Agent;

    protected override Status OnStart()
    {
        Agent.Value.Animator.SetFloat("HitType", (float)UnityEngine.Random.Range(1, 3));
        Agent.Value.Animator.SetTrigger("OnHit");

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

