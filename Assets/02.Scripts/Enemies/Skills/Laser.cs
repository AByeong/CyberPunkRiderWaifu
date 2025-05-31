using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _preDelay;
    private Vector3 center;
    private Vector3 halfExtents;
    private Quaternion rot;

    private Damage _playerDamage = new Damage()
        {
            DamageType = EDamageType.Normal,
            DamageValue = 10,
            DamageForce = 1f,
            AirRiseAmount = 0f
        };

    private Damage _enemyDamage =  new Damage()
        {
            DamageType = EDamageType.Airborne,
            DamageValue = 0,
            DamageForce = 1f,
            AirRiseAmount = 0f
        };

    private float timer;
    
    public void SetDamage(Damage damage)
    {
        _playerDamage = damage;
        _playerDamage.From = gameObject;

        _enemyDamage = damage;
        _enemyDamage.From = gameObject;
        _enemyDamage.DamageValue = 0;
    }

    private void OnEnable()
    {
        timer = 0;
    }

    private void Update()
    {
        Vector3 origin = transform.position;
        Vector3 dir = transform.forward;

        center = origin + dir * (100f / 2f);
        halfExtents = new Vector3(5 / 2f, 5 / 2f, 100 / 2f);
        rot = Quaternion.LookRotation(dir);
            
        timer += Time.deltaTime;
        if(timer >_preDelay)
        {    
            Collider[] hits = Physics.OverlapBox(center, halfExtents, rot);
            foreach (var col in hits)
            {
                if (col.tag == "Player")
                {
                    col.GetComponent<PlayerHit>()?.TakeDamage(_playerDamage);
                }

                if(col.tag == "NormalEnemy")
                {
                    col.GetComponent<PlayerHit>()?.TakeDamage(_enemyDamage);
                }
            }
        }
    }
}
