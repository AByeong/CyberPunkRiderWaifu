using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Skill
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
    }
    public void Init()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public void UseSkill(Transform vfxTransform)
    {
        Debug.Log($"Using {SkillData.SkillName}");
        Animator animator = Player.GetComponent<Animator>();
        animator.SetTrigger($"{SkillData.TriggerName}");
    }
    void Update()
    {
        
    }
}
