using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class DeadState : BaseNormalEnemyState
{
    private float _deadTimer;

    private float _fallTime = 0.5f;
    private float _defaultFallbackLandY = 0.7f;
    private float _groundRaycastDistance = 10f;
    private float _landingYOffset = 1f;

    public override void OnEnter()
    {
        base.OnEnter();

        Owner.NavMeshAgent.enabled = false;

        Owner.SwitchToRagdoll();

        // Owner.Animator.SetFloat("DeadType", Random.Range(0, 3));
        // Owner.Animator.SetTrigger("OnDead");
        Owner.Collider.enabled = false;

        // if (Owner.IsInAir)
        // {
        //     float finalFallY = _defaultFallbackLandY;
        //     NavMeshHit hit;
        //     if (NavMesh.SamplePosition(Owner.transform.position, out hit, _groundRaycastDistance, NavMesh.AllAreas))
        //     {
        //         finalFallY = hit.position.y + _landingYOffset;
        //     }
        //     Vector3 fallPos = new Vector3(transform.position.x, finalFallY, transform.position.z);

        //     Owner.transform.DOMove(fallPos, _fallTime).SetEase(Ease.InQuad);
        //     Owner.IsInAir = false;    
        // }

        
        
        DeliveryManager.Instance.KillTracker.IncreaseKillCount(EnemyType.Normal);
        
        Dictionary<DropItemType, int> dropPlan = Owner.GetDrops();
        ItemDropManager.Instance.DropItems(dropPlan, transform.position, transform.forward);
    }

    public override void Update()
    {
        base.Update();
        _deadTimer += Time.deltaTime;
        if (_deadTimer >= 3f)
        {
            _deadTimer = 0f;
            DeliveryManager.Instance.KillCount++;
            Owner.Pool.ReturnObject(Owner.gameObject);
            Owner.SwitchToOriginModel();
            SuperMachine.ChangeState<IdleState>();
        }
    }

}
