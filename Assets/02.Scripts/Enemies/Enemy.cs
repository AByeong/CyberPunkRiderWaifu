using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public enum EDamageType
{
    // TODO
    TODO
}

public enum EEnemyType
{
    Normal,
    Elite,
    Boss,

    Count
}

public struct Damage
{
    public GameObject From;
    public EDamageType DamageType;
    public float DamageForce;
    public int DamageValue;
}

public abstract class Enemy : MonoBehaviour
{
    private BehaviorGraph _behaviorGraph;
    private BehaviorGraphAgent _behaviorGraphAgent;
    private BlackboardReference _blackboardRef;

    // TODO
    // private EnemyStats _stats;
    // private DropTable _dropTable;

    // 디버깅
    Damage __testDamage__;

    private void Awake()
    {
        _behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();

        _blackboardRef = _behaviorGraphAgent.Graph.BlackboardReference;
        
        // 디버깅
        __testDamage__ = new Damage() { From = gameObject, DamageType = EDamageType.TODO, DamageForce = 2f, DamageValue = 2 };
    }

    // 디버깅
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(__testDamage__);
        }
    }

    public void TakeDamage(Damage damage)
    {
        if (_behaviorGraphAgent == null)
        {
            Debug.LogError($"{gameObject.name} BehaviorAgent가 없습니다!!");
            return;
        }

        Vector3 damagedForceDir = gameObject.transform.position - damage.From.transform.position;
        _blackboardRef.SetVariableValue("DamageForce", damagedForceDir.normalized * damage.DamageForce);
        _blackboardRef.SetVariableValue("DamageValue", damage.DamageValue);
        _blackboardRef.SetVariableValue("IsHit", true);
    }

    private void Attack(GameObject target) // TODO: 매개변수 타입을 Player 클래스로 변경 예정
    {
        // TODO
    }

    private void Die()
    {
        // TODO
    }

    public List<GameObject> GetDrops() // TODO: List<Item>으로 변경예정
    {
        List<GameObject> drops = new List<GameObject>();
        // TODO
        return drops;
    }
}
