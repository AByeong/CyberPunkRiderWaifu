using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Skill : MonoBehaviour
{
    //[Header("Addressable Prefab Reference")]
    //public AssetReferenceGameObject prefabToPoolRef; // Inspector에서 어드레서블 프리팹 할당

    public AnimationClip Clip;
    public AnimatorOverrideController ao;
    public GameObject VFXPrefab;
    public GameObject Player;
    public int Index;
    public SkillData SkillData;

    private bool _isActive = false;
    private AsyncOperationHandle<AnimationClip> _clipHandle;
    private AsyncOperationHandle<GameObject> _vfxHandle;
    private void Awake()
    {
        // AddressableAssetLoad

        //_clipHandle = Addressables.LoadAssetAsync<AnimationClip>("");
        //_clipHandle.Completed += handle =>
        //{
        //    Clip = handle.Result;
        //};
        //_vfxHandle = Addressables.LoadAssetAsync<GameObject>("");
        //_vfxHandle.Completed += handle =>
        //{
        //    VFXPrefab = handle.Result;
        //};
    }
    private void OnDestroy()
    {
        Addressables.Release(_clipHandle);
        Addressables.Release(_vfxHandle);
    }
    public void Init()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public void UseSkill(Transform vfxTransform)
    {
        Debug.Log("Using Skill");
        Animator animator = Player.GetComponent<Animator>();
        animator.SetTrigger($"{SkillData.SkillName}");
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            ///SkillManager.Instance.EquipSkill(KeyCode.Q, 1);
        }
    }
}
