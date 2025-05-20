using UnityEngine;

public class PlayerAttackManager : Singleton<PlayerAttackManager>
{
    [Header("Attack Colliders")]
    [Tooltip("List of Colliders to toggle during attack animations.")]
    [SerializeField]
    private Collider[] attackColliders;

    [Header("Optional Settings")]
    [Tooltip("Automatically disable colliders on Awake if any are enabled.")]
    [SerializeField]
    private bool disableOnAwake = true;

    private void Awake()
    {
        if (disableOnAwake)
        {
            DisableAllColliders();
        }
    }

    public void EnableAttackColliders()
    {
        SetColliders(true);
    }

    public void DisableAttackColliders()
    {
        SetColliders(false);
    }
    private void SetColliders(bool enabled)
    {
        foreach (var col in attackColliders)
        {
            if (col != null)
            {
                col.enabled = enabled;
            }
        }
    }


    private void DisableAllColliders()
    {
        SetColliders(false);
    }

    private void EnableAllColliders()
    {
        SetColliders(true);
    }
}
