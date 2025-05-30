using UnityEngine;

public class OnAttackBehaviour : StateMachineBehaviour
{
    private EliteEnemy Owner;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Owner = animator.gameObject.GetComponent<EliteEnemy>();
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (stateInfo.normalizedTime >=  0.9f)
        {
            
        }
    }
}
