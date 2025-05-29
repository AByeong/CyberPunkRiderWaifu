using UnityEngine;
public class VFXAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            Debug.Log(other.gameObject.name);
        }
    }
}
