using System;
using UnityEngine;
using JY;

public class PlayerHit : MonoBehaviour, IDamageable
{
    
    

    [SerializeField] private float _hitAnimationCoolTime = 1f;
    [SerializeField]private bool _isKnockedBackable = true;
    [SerializeField]private float _knockbackTimer = 0;
    private PlayerController _playerController;


    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (!_isKnockedBackable)
        {
            _knockbackTimer -= Time.deltaTime;
            if (_knockbackTimer <= 0)
            {
                _isKnockedBackable = true;
                Debug.Log("이제 넉백임");
            }
        }

       
    }
    
    public void TakeDamage(Damage damage)
    {
        if (_isKnockedBackable)
        {
            _isKnockedBackable = false;
            _knockbackTimer = _hitAnimationCoolTime;
  //          Debug.Log("넉백 있게 맞음");
            _playerController.TakeDamage(damage, true);
            
        }
        else
        {
//            Debug.Log("넉백 없게 맞음");
            _playerController.TakeDamage(damage, false);
            
        }
        
        
    }
}
