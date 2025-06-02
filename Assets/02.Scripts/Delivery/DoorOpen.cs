using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public Animator DoorAnimator;

    public void OpenDoor()
    {
        DoorAnimator.SetTrigger("Open");
    }
    
    
}
