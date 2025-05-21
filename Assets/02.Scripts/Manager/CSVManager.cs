using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
public class CSVManager : MonoBehaviour
{
    public static CSVManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void GetPlayerStatsAsync(Action<Dictionary<StatType, float>> onComplete)
    {
        Addressables.LoadAssetAsync<TextAsset>("PlayerStats").Completed += handle => {
            Dictionary<StatType, float> dict = new Dictionary<StatType, float>();
            string[] lines = handle.Result.text.Split('\n');

            foreach(string line in lines)
            {
                string[] parts = line.Split(',');

                if (parts.Length < 2) continue;

                if (Enum.TryParse(parts[0], out StatType statType) && float.TryParse(parts[1], out float value))
                {
                    dict[statType] = value;
                }
            }

            onComplete?.Invoke(dict);
        };
    }
}
