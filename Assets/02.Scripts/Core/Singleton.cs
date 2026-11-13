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
    /// <summary>
    /// 이미 존재하는 인스턴스만 반환하고 없으면 null (Instance와 달리 새로 만들지 않음).
    /// "있으면 정리" 같은 존재 여부 검사에 사용한다.
    /// </summary>
    public static T ExistingInstance
    {
        get
        {
            if (_isQuitting) return null;
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // 다른 스크립트가 Awake 전에 Instance로 이 오브젝트를 먼저 찾아둔 경우(_instance == this)는 중복이 아니다
        if (_instance != null && _instance != this)
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
        // 중복 인스턴스가 파괴될 때 원본 참조까지 지우지 않도록
        if (_instance == this)
        {
            _instance = null;
        }
    }
}

