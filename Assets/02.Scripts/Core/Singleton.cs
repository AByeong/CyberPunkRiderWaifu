using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    private bool _dontDestroy = true; // DontDestroy �Ӽ� �������� ����
    private static bool _isQuitting = false; // ������ ��� ����� �ν��Ͻ� ������ ���� ���� �÷��� ����
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_isQuitting)
            {
                return null;
            }

            if (_instance == null)
            {
                T[] instances = FindObjectsByType<T>(FindObjectsSortMode.None);

                if (1 < instances.Length)
                {
                    Debug.LogError($"�ߺ��� Singleton {typeof(T)} �ν��Ͻ��� �����Ǿ�, �ߺ� �ν��Ͻ� ���Ÿ� �����մϴ�.");
                    for (int i = 1; i < instances.Length; i++)
                    {
                        Destroy(instances[i].gameObject);
                    }
                }

                _instance = instances.Length > 0 ? instances[0] : null;
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name, typeof(T));
                    _instance = go.GetComponent<T>();
                }
            }
            return _instance;
        }
    }
    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Destroy(transform.root.gameObject);
            return;
        }
        else
        {
            _instance = this as T;
        }

        if (_dontDestroy && transform.parent != null && transform.root != null)
        {
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            GameObject rootGO = GameObject.FindGameObjectWithTag("Singleton");
            if (rootGO != null)
            {
                transform.SetParent(rootGO.transform);
            }
            else if (_dontDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        _instance = null;
    }
}

