using Unity.VisualScripting;
using UnityEngine;

public class OnAttackBehaviour : StateMachineBehaviour
{

    private EliteEnemy Owner;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Owner = animator.gameObject.GetComponent<EliteEnemy>();

        Owner.IsAttacking = true;
        
        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        
        Owner.IsAttacking = false;
        Owner.AttackTimer = 0;

        
        
    }
}
