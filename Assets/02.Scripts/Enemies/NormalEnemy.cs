using UnityEngine;
using UnityEngine.AI;

public class NormalEnemy : Enemy
{
    
    
    protected override void Awake()
    {
        base.Awake();

       
    }

    
    

    void Update()
    {
        _animator.SetFloat("Velocity", _navMeshAgent.velocity.magnitude);
    }

}
