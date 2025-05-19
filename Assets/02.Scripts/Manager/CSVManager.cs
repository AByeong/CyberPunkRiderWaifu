using UnityEngine;
public class CSVManager : MonoBehaviour
{
    public static CSVManager instance;

    public void Awake()
    {
        instance = this;
    }

    public string GetPlayerStats()
    {
        return null;
    }
    
}