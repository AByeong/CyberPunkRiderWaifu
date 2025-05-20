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
    protected Damage __testDamage__;
    public GameObject Player;

    protected virtual void Awake()
    {
        _behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();

        // 디버깅
        __testDamage__ = new Damage() { From = Player, DamageType = EDamageType.TODO, DamageForce = 2f, DamageValue = 2 };
    }

    public void TakeDamage(Damage damage)
    {
        _blackboardRef = _behaviorGraphAgent.Graph.BlackboardReference;
        if (_blackboardRef == null)
        {
            Debug.LogError($"{gameObject.name} BlackboardRef가 없습니다!!");
            return;
        }

        Vector3 damagedForceDir = gameObject.transform.position - damage.From.transform.position;
        _blackboardRef.SetVariableValue("DamageForce", damagedForceDir.normalized * damage.DamageForce);
        _blackboardRef.SetVariableValue("DamageValue", damage.DamageValue);
        _blackboardRef.SetVariableValue("EEnemyState", EEnemyState.Hit);
        _blackboardRef.SetVariableValue("IsHit", true);
        Debug.Log("TakeDamage");
    }

    public List<GameObject> GetDrops() // TODO: List<Item>으로 변경예정
    {
        List<GameObject> drops = new List<GameObject>();
        // TODO
        return drops;
    }
}
