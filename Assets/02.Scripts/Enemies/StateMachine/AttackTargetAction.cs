using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackTarget", story: "[Agent] attacks [Target]", category: "Action", id: "750d2ecbba0965c545c769cae58c48a2")]
public partial class AttackTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private IDamageable _target;
    private Enemy _agent;
    private Damage _damage = new Damage();

    protected override Status OnStart()
    {
        _target = Target.Value.GetComponent<IDamageable>();
        _agent = Agent.Value.GetComponent<Enemy>();

        _damage.From = _agent.gameObject;
        _damage.DamageType = EDamageType.TODO;
        _damage.DamageForce = _agent.EnemyData.AttackForce;
        _damage.DamageValue = _agent.EnemyData.Damage;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        
        if (_target != null)
        {
            _target.TakeDamage(_damage);
        }

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

