using System.Collections;
using UnityEngine;

public class Summoner : MonoBehaviour
{
    public Transform PlayerTransform;
    public GameObject BombEffect;
    [SerializeField] private float _bombArea = 3f;
    [SerializeField] private float _bombCounter = 10f;
    [SerializeField] private float _moveSpeed = 2f; // 🔸 이동 속도 추가

    private void Awake()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(Count());
    }

    IEnumerator Count()
    {
        yield return new WaitForSeconds(_bombCounter);
        Boom();
    }

    private void Update()
    {
        MoveTowardPlayer(); // 🔸 이동 함수 호출
    }

    private void MoveTowardPlayer()
    {
        if (PlayerTransform == null) return;

        // 방향 벡터 계산
        Vector3 direction = (PlayerTransform.position - transform.position).normalized;

        // 이동 (프레임 보정)
        transform.position += direction * _moveSpeed * Time.deltaTime;
        
         transform.LookAt(PlayerTransform.position);
    }

    private void Boom()
    {
        if (Vector3.Distance(PlayerTransform.position, this.transform.position) < _bombArea)
        {
            Damage damage = new Damage
            {
                DamageValue = 20,
                From = this.gameObject,
                DamageForce = 5,
                DamageType = EDamageType.Normal
            };
            
            Instantiate(BombEffect,this.transform.position,this.transform.rotation);
            PlayerTransform.gameObject.GetComponent<PlayerHit>().TakeDamage(damage);
        }

        Destroy(this.gameObject);
    }
}