using UnityEngine;
using UnityEngine.AddressableAssets; // 어드레서블 사용을 위해 추가
using UnityEngine.ResourceManagement.AsyncOperations; // AsyncOperationHandle 사용을 위해 추가
using System.Collections.Generic;
using System.Threading.Tasks; // Task 사용을 위해 추가

public class ObjectPool : MonoBehaviour
{
    [Header("Addressable Prefab Reference")]
    public List<AssetReferenceGameObject> prefabToPoolRef; // Inspector에서 어드레서블 프리팹 할당

    [Header("Pool Settings")]
    public int initialPoolSize = 20;
    public bool allowPoolToGrow = true;

    private List<GameObject> loadedPrefab = new List<GameObject>(); // 로드된 원본 프리팹을 캐싱할 변수

    private List<GameObject> allPooledObjects = new List<GameObject>();
    private Queue<GameObject> availableObjects = new Queue<GameObject>();
    private Transform poolContainer;

    private bool isPoolReady = false;
    private bool isLoadingPrefab = false;

    async void Start() // 비동기 로드를 위해 async void 사용
    {
        string containerName = "AddressablePool_UnknownKey";
        foreach (var prefab in prefabToPoolRef)
        {
            if (prefab != null && prefab.RuntimeKeyIsValid())
            {
                // RuntimeKey는 object 타입일 수 있으므로, ToString()으로 문자열화 합니다.
                // 파일 이름으로 사용하기 위해 일부 문자 제거 또는 해시값 사용 등을 고려할 수 있으나, 여기선 단순화합니다.
                string keyString = prefab.RuntimeKey.ToString();
                // 파일 경로에 부적합한 문자들을 제거하거나 대체하는 것이 좋습니다.
                // 여기서는 간단히 System.IO.Path.GetFileNameWithoutExtension을 사용합니다.
                // 실제 키가 URL이나 복잡한 문자열일 경우 추가 처리가 필요할 수 있습니다.
                containerName = $"AddressablePool_{System.IO.Path.GetFileNameWithoutExtension(keyString)}";
            }
        }
        

        poolContainer = new GameObject(containerName).transform;
        poolContainer.SetParent(this.transform);

        foreach (var prefab in prefabToPoolRef)
        {
            if (prefab == null || !prefab.RuntimeKeyIsValid())
            {
                Debug.LogError("ObjectPool: Prefab Addressable Reference is not set or is invalid!", this);
                isPoolReady = false;
                return;
            }
        }
        

        isLoadingPrefab = true;
        // ***** 수정된 부분: AssetAddress 대신 RuntimeKey 사용 *****
//        Debug.Log($"ObjectPool: Loading prefab from Addressable RuntimeKey: '{prefabToPoolRef.RuntimeKey}'", this);

        foreach (var prefab in prefabToPoolRef)
        {
            AsyncOperationHandle<GameObject> handle = prefab.LoadAssetAsync<GameObject>();
        
            await handle.Task;
        
            isLoadingPrefab = false;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedPrefab.Add(handle.Result);
                // ***** 수정된 부분: AssetAddress 대신 RuntimeKey 사용 *****
//            Debug.Log($"ObjectPool: Prefab '{loadedPrefab.name}' loaded successfully from Addressable RuntimeKey: '{prefabToPoolRef.RuntimeKey}'", this);
                InitializePool();
                isPoolReady = true;
            }
            else
            {
                // ***** 수정된 부분: AssetAddress 대신 RuntimeKey 사용 *****
                //   Debug.LogError($"ObjectPool: Failed to load prefab from Addressable RuntimeKey: '{prefabToPoolRef.RuntimeKey}' - Exception: {handle.OperationException}", this);
                isPoolReady = false;
                // AssetReference 자체를 통해 로드된 경우, OnDestroy에서 ReleaseAsset()으로 처리됩니다.
                // 개별 핸들을 명시적으로 해제할 필요는 일반적으로 없습니다.
            }
        }
    }

    void InitializePool()
    {
        if (loadedPrefab == null)
        {
            Debug.LogError("ObjectPool: Cannot initialize pool, prefab not loaded!", this);
            return;
        }
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateAndPoolObject();
        }
//        Debug.Log($"ObjectPool: Initialized with {initialPoolSize} objects for '{loadedPrefab.name}'.", this);
    }

    void CreateAndPoolObject()
    {
        if (loadedPrefab == null)
        {
     //     Debug.LogError("ObjectPool: Attempted to create object, but loadedPrefab is null!", this);
            return;
        }

        foreach (var prefab in loadedPrefab)
        {
            GameObject obj = Instantiate(prefab, poolContainer);
            obj.SetActive(false);
            allPooledObjects.Add(obj);
            availableObjects.Enqueue(obj);
        }
    }

    public GameObject GetObject(int index = 0)
    {
        if (!isPoolReady)
        {
            if (isLoadingPrefab) Debug.LogWarning("ObjectPool: GetObject called while prefab is still loading. Returning null.", this);
            else Debug.LogError("ObjectPool: GetObject called but pool is not ready (prefab failed to load or not set). Returning null.", this);
            return null;
        }
        if (index < 0 || index >= loadedPrefab.Count)
        {
            Debug.LogError($"ObjectPool: Invalid prefabIndex {index}.", this);
            return null;
        }

        if (availableObjects.Count > 0)
        {
            GameObject objToServe = availableObjects.Dequeue();
            objToServe.SetActive(true);
            return objToServe;
        }

        if (allowPoolToGrow)
        {
            GameObject prefabToUse = loadedPrefab[index];
            GameObject newObj = Instantiate(prefabToUse, poolContainer);
            newObj.SetActive(true);
            allPooledObjects.Add(newObj);
            return newObj;
        }

        //Debug.LogWarning($"ObjectPool: Pool for {(loadedPrefab != null ? loadedPrefab.name : "N/A")} is empty and growth is not allowed. Returning null.", this);
        return null;
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);
        obj.transform.SetParent(poolContainer);

        if (!availableObjects.Contains(obj))
        {
            availableObjects.Enqueue(obj);
        }
    }

    void OnDestroy()
    {
        foreach (var prefab in prefabToPoolRef)
        {
            if (prefab.IsValid() && prefab.Asset != null) // AssetReference에 의해 에셋이 로드되었는지 확인
            {
                prefab.ReleaseAsset(); // AssetReference를 통해 로드된 에셋 해제
//            Debug.Log($"ObjectPool: Released Addressable asset for {loadedPrefab?.name ?? "prefabRef"}.", this);
            }
        }
        
        loadedPrefab = null;

        if (availableObjects != null) availableObjects.Clear();
        if (allPooledObjects != null) allPooledObjects.Clear();
    }
}