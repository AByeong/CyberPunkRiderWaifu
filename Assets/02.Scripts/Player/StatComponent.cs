using System.Collections.Generic;
using JY;
using UnityEngine;
public class StatComponent : MonoBehaviour
{
    public PlayerController Player;
    public Stat Stat;

    private void Reset()
    {
        Player = GetComponent<PlayerController>();
    }
    private void Start()
    {
        Stat = Player.Stat as Stat;
    }

    // Stat에 접근할 수 있는 메서드 추가
    public Dictionary<StatType, float> GetStatDictionary()
    {
        return Stat.Stats;
    }
}
