using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("[Parameters]")]
    public BossPhase1 Owner;
    [SerializeField] private float _preDelay = 1f;
    [SerializeField] private float _postDelay = 0.5f;

    private Vector3 center;
    private Vector3 halfExtents;
    private Quaternion rot;

    private const float DURATION = 3f;

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
        AirRiseAmount = 2f
    };

    private float timer;

    // 매 프레임 판정하면 프레임레이트에 비례해 데미지가 들어가 사실상 즉사하므로 틱 간격을 둔다
    [SerializeField] private float _damageTickInterval = 0.25f;
    private float _tickTimer;

    public void SetDamage(Damage damage)
    {
        _playerDamage = damage;
        _playerDamage.From = gameObject;

        _enemyDamage = damage;
        _enemyDamage.DamageValue = 0;
        _enemyDamage.DamageType = EDamageType.Airborne;
        _enemyDamage.AirRiseAmount = 2f;
        _enemyDamage.From = gameObject;
    }

    private void OnEnable()
    {
        timer = 0;
        _tickTimer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= DURATION)
        {
            Destroy(gameObject);
            return;
        }

        if(timer >_preDelay && timer <= DURATION - _postDelay)
        {
            _tickTimer -= Time.deltaTime;
            if (_tickTimer > 0f) return;
            _tickTimer = _damageTickInterval;

            Vector3 origin = transform.position;
            Vector3 dir = transform.forward;

            center = origin + dir * (100f / 2f);
            halfExtents = new Vector3(5 / 2f, 5 / 2f, 100 / 2f);
            rot = Quaternion.LookRotation(dir);

            Collider[] hits = Physics.OverlapBox(center, halfExtents, rot);
            foreach (var col in hits)
            {
                if (!col.TryGetComponent(out IDamageable damageable)) continue;
                if (col.tag == "Player")
                {
                    damageable.TakeDamage(_playerDamage);
                }

                if(col.tag == "NormalEnemy")
                {
                    damageable.TakeDamage(_enemyDamage);
                }
            }
        }
    }
}
