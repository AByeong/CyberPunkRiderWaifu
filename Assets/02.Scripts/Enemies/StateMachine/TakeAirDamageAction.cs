using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Take AirDamage", story: "[Agent] Take AirDamage", category: "Action", id: "0bdd4f6be5f35ae5e8d0060524a27c9a")]
public partial class TakeAirDamageAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Agent;

    protected override Status OnStart()
    {
        // Agent.Value.NavMeshAgent.isStopped = true;
        Agent.Value.NavMeshAgent.enabled = false;
        Agent.Value.transform.position += new Vector3(0, 1.5f, 0);
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

