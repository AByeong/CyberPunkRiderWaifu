using UnityEngine;
using UnityEngine.Animations;

public class WaifuSpell : StateMachineBehaviour
{
   
   private EliteEnemy Owner;
   public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
      base.OnStateEnter(animator, stateInfo, layerIndex);
      Owner = animator.gameObject.GetComponent<EliteEnemy>();
      
      animator.applyRootMotion = true;
   }

   public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
      AnimatorControllerPlayable controller)
   {
      base.OnStateUpdate(animator, stateInfo, layerIndex, controller);

      if (stateInfo.normalizedTime >= 1f)
      {
         animator.applyRootMotion =false;
         Owner.IsAttacking = false;
         Owner.AttackTimer = 0;
      }
      
   }
}
