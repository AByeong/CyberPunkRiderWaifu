using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackTarget", story: "Agent attacks [Target]", category: "Action", id: "750d2ecbba0965c545c769cae58c48a2")]
public partial class AttackTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    private IDamageable _target;

    protected override Status OnStart()
    {
        _target = Target.Value.GetComponent<IDamageable>();
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Damage damage = new Damage() { From = GameObject, DamageType = EDamageType.TODO, DamageForce = 2f, DamageValue = 1 };
        if (_target != null)
        {
            _target.TakeDamage(damage);
        }
        Debug.Log("Attack");

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

