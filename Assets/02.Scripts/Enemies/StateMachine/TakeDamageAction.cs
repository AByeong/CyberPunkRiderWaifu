using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TakeDamage", story: "[Agent] [HealthPoint] Take [DamageValue] [DamageForce]", category: "Action", id: "dd16ce5c85a8cf6de6e63cd777901454")]
public partial class TakeDamageAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Agent;
    [SerializeReference] public BlackboardVariable<int> HealthPoint;
    [SerializeReference] public BlackboardVariable<int> DamageValue;
    [SerializeReference] public BlackboardVariable<Vector3> DamageForce;

    private BehaviorGraphAgent _behaviorGraphAgent;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private Rigidbody _rigidbody;

    protected override Status OnStart()
    {
        _navMeshAgent = Agent.Value.GetComponent<NavMeshAgent>();
        _rigidbody = Agent.Value.GetComponent<Rigidbody>();
        _animator = Agent.Value.GetComponentInChildren<Animator>();
        _behaviorGraphAgent = Agent.Value.GetComponent<BehaviorGraphAgent>();

        _rigidbody.isKinematic = false;
        _navMeshAgent.enabled = false;

        _behaviorGraphAgent.BlackboardReference.SetVariableValue("HealthPoint", HealthPoint - DamageValue);
        _animator.SetFloat("HitType", (float)UnityEngine.Random.Range(1, 3));
        _animator.SetTrigger("OnHit");
        _rigidbody.AddForce(DamageForce.Value, ForceMode.Impulse);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
        _rigidbody.isKinematic = true;
        _navMeshAgent.enabled = true;
    }
}

