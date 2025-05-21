using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
public static class StatLoader
{
    public static async Task<Stat> LoadFromCSVAsync(string relativePath)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
        UnityWebRequest request = UnityWebRequest.Get(fullPath);

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield(); // 프레임 양보

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"CSV 로딩 실패: {request.error}");
            return null;
        }

        string[] lines = request.downloadHandler.text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
        {
            Debug.LogError("CSV 데이터 부족");
            return null;
        }

        string[] headers = lines[0].Split(',');
        string[] values = lines[1].Split(',');

        Dictionary<StatType, float> statDict = new Dictionary<StatType, float>();

        for (int i = 0; i < headers.Length; i++)
        {
            if (Enum.TryParse(headers[i], out StatType statType) &&
                float.TryParse(values[i], out float value))
            {
                statDict[statType] = value;
            }
        }

        return new Stat(statDict);
    }
}
