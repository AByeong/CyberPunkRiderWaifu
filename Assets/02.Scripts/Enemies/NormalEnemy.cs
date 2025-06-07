using UnityEngine;

public class NormalEnemy : Enemy
{
    public Collider WeaponCollider;
    public GameObject OriginModel;
    public GameObject RagdollModel;

    private float RagdollForceMultiply = 1000f;

    [SerializeField] private Rigidbody _rigidbody;

    protected override void Awake()
    {
        base.Awake();
        WeaponCollider.enabled = false;
        // _rigidbody = GetComponentInChildren<Rigidbody>();

    }

    void Update()
    {
        _animator.SetFloat("Velocity", _navMeshAgent.velocity.magnitude);
    }

    public void SwitchToRagdoll()
    {
        WeaponCollider.enabled = false;
        _collider.enabled = false;

        OriginModel.SetActive(false);
        RagdollModel.SetActive(true);

        Vector3 ragdollForce = (transform.position - TakedDamage.From.transform.position).normalized * TakedDamage.DamageForce * RagdollForceMultiply;
        Debug.LogError($"{gameObject.name} :: {ragdollForce}");
        _rigidbody.AddForce(ragdollForce, ForceMode.Impulse);
    }

    public void SwitchToOriginModel()
    {
        RagdollModel.SetActive(false);
        OriginModel.SetActive(true);

        _collider.enabled = true;
    }
}
