using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAttackManager : Singleton<PlayerAttackManager>
{
    [FormerlySerializedAs("attackColliders")]
    [Header("Attack Colliders")]
    [Tooltip("List of Colliders to toggle during attack animations.")]
    [SerializeField]
    private Collider[] _attackColliders;

    [FormerlySerializedAs("disableOnAwake")]
    [Header("Optional Settings")]
    [Tooltip("Automatically disable colliders on Awake if any are enabled.")]
    [SerializeField]
    private bool _disableOnAwake = true;

    public StretchColliderOnly StretchColliderOnly;
    private void Awake()
    {
        if (_disableOnAwake)
        {
            DisableAllColliders();
        }
    }

    private void Start()
    {
        StretchColliderOnly = GetComponentInChildren<StretchColliderOnly>();
    }

    public void EnableAttackColliders()
    {
        SetColliders(true);
    }

    public void DisableAttackColliders()
    {
        SetColliders(false);
        StretchColliderOnly.StretchDown(1);
    }
    private void SetColliders(bool enabled)
    {
        foreach(Collider _collider in _attackColliders)
        {
            if (_collider != null)
            {
                _collider.enabled = enabled;
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
