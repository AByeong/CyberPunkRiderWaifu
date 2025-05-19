using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    private string _data;
    public void Start()
    {
        _data = CSVManager.instance.GetPlayerStats();
    }
}