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


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        
        if (stateInfo.normalizedTime >= 1f)
        {
            // Owner 스크립트의 속성을 변경합니다.
            Owner.IsAttacking = false;
            Owner.AttackTimer = 0;
        }
        
        
    }

    
}
