using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefabToPool;
    public int initialPoolSize = 20;
    public bool allowPoolToGrow = true;

    private List<GameObject> allPooledObjects = new List<GameObject>(); // 풀에 의해 생성된 모든 오브젝트 추적 (활성/비활성 포함)
    private Queue<GameObject> availableObjects = new Queue<GameObject>(); // 사용 가능한 (비활성) 오브젝트 큐
    private Transform poolContainer;

    void Awake()
    {
        poolContainer = new GameObject(prefabToPool.name + "_Pool").transform;
        poolContainer.SetParent(this.transform);

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateAndPoolObject();
        }
    }

    GameObject CreateAndPoolObject()
    {
        GameObject obj = Instantiate(prefabToPool, poolContainer);
        obj.SetActive(false);
        allPooledObjects.Add(obj); // 모든 생성된 오브젝트 리스트에 추가
        availableObjects.Enqueue(obj); // 사용 가능 큐에 추가
        return obj;
    }

    public GameObject GetObject()
    {
        if (availableObjects.Count > 0)
        {
            GameObject objToServe = availableObjects.Dequeue();
            objToServe.SetActive(true);
            return objToServe;
        }

        if (allowPoolToGrow)
        {
            Debug.LogWarning($"Pool for {prefabToPool.name} is growing. Current size: {allPooledObjects.Count}");
            GameObject newObj = Instantiate(prefabToPool, poolContainer); // 컨테이너는 지정, 바로 활성화해서 반환하므로 큐에 넣지 않음
            newObj.SetActive(true);
            allPooledObjects.Add(newObj); // 모든 생성된 오브젝트 리스트에는 추가
            return newObj;
        }

        Debug.LogWarning($"Pool for {prefabToPool.name} is empty and growth is not allowed.");
        return null;
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);
        obj.transform.SetParent(poolContainer); // 확실히 풀 컨테이너 하위로 이동

        if (!availableObjects.Contains(obj)) // 중복 반환 방지 (선택적, 상황에 따라 필요 없을 수 있음)
        {
            availableObjects.Enqueue(obj);
        }
        else
        {
            // Debug.LogWarning($"Object {obj.name} is already in the available pool.");
        }
    }
}