using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackTarget", story: "[Agent] attacks [Target]", category: "Action", id: "750d2ecbba0965c545c769cae58c48a2")]
public partial class AttackTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    private IDamageable _target;
    private Damage _damage = new Damage();

    protected override Status OnStart()
    {
        _target = Target.Value.GetComponent<IDamageable>();

        _damage.From = Agent.Value.gameObject;
        _damage.DamageType = EDamageType.TODO;
        _damage.DamageForce = Agent.Value.EnemyData.AttackForce;
        _damage.DamageValue = Agent.Value.EnemyData.Damage;
        if (_target != null)
        {
            _target.TakeDamage(_damage);
        }

        Agent.Value.NavMeshAgent.isStopped = true;

        

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

