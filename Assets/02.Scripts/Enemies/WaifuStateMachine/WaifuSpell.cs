using UnityEngine;

public class WaifuSpell : StateMachineBehaviour
{

   private EliteEnemy Owner;
   private ElliteStateMachine _stateMachine;
   

   public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
      base.OnStateEnter(animator, stateInfo, layerIndex);
      Owner = animator.GetComponent<EliteEnemy>();
      _stateMachine = animator.GetComponent<ElliteStateMachine>();

      animator.applyRootMotion = true;
   }

   public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
      base.OnStateUpdate(animator, stateInfo, layerIndex);

      if (stateInfo.normalizedTime >= 1f)
      {
         animator.applyRootMotion = false;
         _stateMachine.ChangeState<EliteIdleState>();
      }
   }
}
