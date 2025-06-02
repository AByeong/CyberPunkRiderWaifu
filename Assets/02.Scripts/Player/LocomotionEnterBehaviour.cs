using JY;
using UnityEngine;
public class LocomotionEnterBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController controller = animator.GetComponent<PlayerController>();
        if (controller != null && controller.MoveDirection.sqrMagnitude > 0f)
        {
            controller.transform.rotation = Quaternion.LookRotation(controller.MoveDirection);
        }
    }
}
