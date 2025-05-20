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
    private Enemy _enemy;
    private EEnemyType _enemyType;
    protected override Status OnStart()
    {
        //킬카운트 상승
        _enemy = Agent.Value.GetComponent<Enemy>();
        _enemyType = _enemy.EnemyData.EnemyType;
        switch (_enemyType)
        {
            case EEnemyType.Normal:
                DeliveryManager.Instance.KillTracker.CurrentKillCount.Normal++;
                break;
            
            case EEnemyType.Elite:
                DeliveryManager.Instance.KillTracker.CurrentKillCount.Elite++;
                break;
            
            case EEnemyType.Boss:
                DeliveryManager.Instance.KillTracker.CurrentKillCount.Boss++;
                break;
            
        }

        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
        //풀링에 반환
        if (_enemyType == EEnemyType.Normal)
        {
            _enemy.Pool.ReturnObject(this.GameObject);
        }
        
        
    }
}

