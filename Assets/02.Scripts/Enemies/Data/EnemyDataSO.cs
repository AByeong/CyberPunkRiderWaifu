using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Scriptable Objects/EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    public EEnemyType EnemyType;
    public int HealthPoint;
    public int Damage;
    public float AttackCoolDown;
    public float AttackDistance;
    public float AttackForce;
    public float StaggerTime;
    public float DownTime;
    public int InAirThreshold;
}
