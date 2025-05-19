using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TakeDamage", story: "[Agent] [HealthPoint] Decreased [Damage]", category: "Action", id: "dd16ce5c85a8cf6de6e63cd777901454")]
public partial class TakeDamageAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Agent;
    [SerializeReference] public BlackboardVariable<int> HealthPoint;
    [SerializeReference] public BlackboardVariable<int> Damage;
    private Enemy _agent;

    protected override Status OnStart()
    {
        _agent = Agent.Value;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        _agent.GetComponent<NavMeshAgent>().isStopped = true;
        _agent.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("HealthPoint", HealthPoint - Damage);

        return Status.Success;
    }

    protected override void OnEnd()
    {
        _agent.GetComponent<NavMeshAgent>().isStopped = false;        
    }
}

