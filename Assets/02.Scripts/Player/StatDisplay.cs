using System.Collections.Generic;
using System.Text;
using JY;
using TMPro;
using UnityEngine;
public class StatDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private PlayerController playerController;
    private Dictionary<StatType, float> _previousStats;
    private bool _isInitialized;
    private IStatsProvider _lastStat;

    // private void Start()
    // {
    //     _previousStats = new Dictionary<StatType, float>();
    //     _isInitialized = false;
    //     _lastStat = null;
    //
    //     if (playerController == null)
    //     {
    //         Debug.LogError("PlayerController가 할당되지 않았습니다!");
    //         return;
    //     }
    //
    //     // 초기 스탯 표시
    //     DisplayStats();
    // }
    //
    private bool HasStatsChanged()
    {
        if (!_isInitialized || playerController.Stat == null) return false;

        Stat currentStat = playerController.Stat as Stat;
        if (currentStat == null || currentStat.Stats == null) return false;

        Dictionary<StatType, float> currentStats = currentStat.Stats;

        if (_previousStats.Count != currentStats.Count) return true;

        foreach(KeyValuePair<StatType, float> stat in currentStats)
        {
            if (!_previousStats.ContainsKey(stat.Key) ||
                !Mathf.Approximately(_previousStats[stat.Key], stat.Value))
            {
                return true;
            }
        }

        return false;
    }

    public void DisplayStats(Stat stat)
    {
        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

        Stat currentStat = stat;
       
        StringBuilder sb = new StringBuilder();
        Dictionary<StatType, float> statDict = currentStat.Stats;

        foreach(KeyValuePair<StatType, float> entry in statDict)
        {
            sb.AppendLine($"{entry.Key}: {entry.Value}");
        }

        textUI.text = sb.ToString();

        // 현재 스탯을 이전 스탯으로 저장
        _previousStats = new Dictionary<StatType, float>(statDict);
    }
}
