using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    public string MaterialName; // 이 변수는 이제 단일 머티리얼 이름이 아닌, targetMaterialNames에 추가될 기본값으로 활용
    [SerializeField]
    private EnemyDataSO _enemyData;
    public ObjectPool Pool;

    public const float GRAVITY = 9.8f;

    public bool IsHit { get; set; }
    public bool IsInAir { get; set; }
    public Vector3 VerticalVelocity = new Vector3();

    public Damage TakedDamage => _takedDamage;
    private Damage _takedDamage = new Damage();

    protected Animator _animator;
    protected NavMeshAgent _navMeshAgent;
    protected CharacterController _characterController;
    protected Collider _collider;

    private IStatsProvider _stat;
    // TODO
    // private DropTable _dropTable;

    public GameObject Target { get; set; }
    public EnemyDataSO EnemyData => _enemyData;
    public int CurrentHealthPoint { get; private set; }

    public Animator Animator => _animator;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;
    public Collider Collider => _collider;
    public Transform DamagePopupPosition;
    public GameObject DamagePopup;
    public GameObject WorldSpaceCanvas;

    protected virtual void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if (_navMeshAgent == null)
        {
            Debug.LogWarning($"{gameObject.name} NavMeshAgent가 없습니다");
        }

        _animator = GetComponentInChildren<Animator>();
        if (_animator == null)
        {
            Debug.LogWarning($"{gameObject.name} Animator가 없습니다");
        }

        _collider = GetComponent<Collider>();
        if (_collider == null)
        {
            Debug.LogWarning($"{gameObject.name} Collider가 없습니다");
        }
    }

    private async void Start()
    {
        _stat = await StatLoader.LoadFromCSVAsync("EnemyStat.csv");
        _stat = new StatModifierDecorator(_stat, StatType.AttackPower, 20);

        CurrentHealthPoint = _enemyData.HealthPoint;
        IsHit = false;
        IsInAir = false;

        // Start에서 렌더러와 초기 색상 캐싱
        CacheRenderersAndInitialColors();
    }

    public virtual void Initialize()
    {
        CurrentHealthPoint = EnemyData.HealthPoint;
        IsHit = false;
        IsInAir = false;
        // Reset hit flash if it was active
        if (_hitFlashCoroutine != null)
        {
            StopCoroutine(_hitFlashCoroutine);
            _hitFlashCoroutine = null;
            // 초기 색상으로 복구 (Initialize 시점에는 이미 복구되어 있어야 하나, 안전장치)
            ApplyOriginalColorsToAllRenderers(); 
        }
    }

    public void PlayHitParticle()
    {
        // CacheRenderersAndInitialColors는 Start 또는 Initialize에서 이미 호출되었다고 가정합니다.
        // 하지만 혹시 모를 경우를 대비하여 다시 호출할 수 있습니다. (성능에 영향)
        // CacheRenderersAndInitialColors(); 

        if (_targetedRenderersData.Count == 0) // 캐시된 렌더러 데이터가 없으면 경고 후 종료
        {
            Debug.LogWarning($"[HitState] PlayHitParticle: No renderers or valid materials found for {gameObject.name}. Hit flash may not work.");
            return;
        }

        if (_hitFlashCoroutine != null) StopCoroutine(_hitFlashCoroutine);

        float staggerDuration = (EnemyData != null && EnemyData.StaggerTime > 0) ? EnemyData.StaggerTime : hitFlashEffectDuration;
        float currentFlashDuration = Mathf.Min(staggerDuration, hitFlashEffectDuration);

        if (currentFlashDuration > 0)
        {
            _hitFlashCoroutine = StartCoroutine(HitFlashEffectCoroutine(currentFlashDuration));
        }
        else // 지속시간이 0이면 즉시 normalVValue 적용 (또는 원래 색상)
        {
            SetAllRenderersColorToVValue(normalVValue); // 모든 대상 렌더러에 적용
        }
    }


    public virtual void TakeDamage(Damage damage)
    {
        IsHit = true;

        CurrentHealthPoint -= damage.DamageValue;
        _takedDamage = damage;

        PlayHitParticle();
        // Vector3 damagedForceDir = (gameObject.transform.position - damage.From.transform.position).normalized;
        Vector3 worldPos = DamagePopupPosition.position;

        // Canvas 하위로 생성
        GameObject popup = Instantiate(DamagePopup, WorldSpaceCanvas.transform);
        popup.transform.position = worldPos;
        popup.GetComponentInChildren<TypingEffect>().Typing(damage.DamageValue.ToString());
        // 선택: 파괴 또는 애니메이션
        Destroy(popup, 1.5f);
    }

    public List<GameObject> GetDrops() // TODO: List<Item>으로 변경예정
    {
        List<GameObject> drops = new List<GameObject>();
        // TODO
        return drops;
    }



    #region 피격머테리얼 효과
    // --- 피격 머테리얼 효과 관련 변수 ---
    private Coroutine _hitFlashCoroutine;

    [Header("Hit Flash Material Effect")]
    [Tooltip("효과를 적용할 머티리얼의 이름 목록. 각 렌더러의 머티리얼 중 이 목록에 포함된 이름(contains)을 가진 머티리얼에 효과 적용.")]
    public List<string> targetMaterialNames = new List<string> { "KyleRobot" }; // 기본값 예시

    [Tooltip("머티리얼의 메인 색상 프로퍼티 이름 (예: _BaseColor, _Color)")]
    public string mainColorPropertyName = "_Color"; // 대부분 셰이더에서 _Color 또는 _BaseColor 사용

    [Tooltip("피격 시 밝기(V) 값. 높을수록 희게 보임 (HDR 고려 시 1.0 이상)")]
    [Range(0f, 5f)]
    public float highlightVValue = 2.0f;

    [Tooltip("피격 후 잠시 유지될 밝기(V) 값. 원래 머티리얼의 V값과 유사하게 설정")]
    [Range(0f, 5f)]
    public float normalVValue = 0.8f; // 예시 값, 실제 머티리얼 밝기에 맞춰 조정

    [Tooltip("피격 효과 지속 시간 (초). StaggerTime 보다 짧게 설정될 수 있음")]
    public float hitFlashEffectDuration = 0.3f;

    // 각 렌더러와 해당 렌더러의 머티리얼별 초기 색상을 저장할 구조체
    private struct RendererMaterialData
    {
        public Renderer Renderer;
        public int MaterialIndex; // 해당 렌더러의 materials 배열에서의 인덱스
        public Color InitialColor;
    }

    // 대상 렌더러와 그에 해당하는 머티리얼 데이터를 저장하는 리스트
    private List<RendererMaterialData> _targetedRenderersData = new List<RendererMaterialData>();
    private MaterialPropertyBlock _propertyBlock;

    // --- 피격 머테리얼 효과 관련 변수 끝 ---


    private void CacheRenderersAndInitialColors()
    {
        // MaterialName이 설정되어 있고 targetMaterialNames에 아직 없다면 추가
        if (!string.IsNullOrEmpty(MaterialName) && !targetMaterialNames.Contains(MaterialName))
        {
            targetMaterialNames.Add(MaterialName);
        }

        if (targetMaterialNames == null || targetMaterialNames.Count == 0)
        {
            Debug.LogWarning($"[HitState] CacheRenderersAndInitialColors: targetMaterialNames is not set for {gameObject.name}. Hit flash may not work.");
            return;
        }

        _targetedRenderersData.Clear(); // 이전 데이터 클리어

        Renderer[] allRenderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in allRenderers)
        {
            if (rend != null && rend.sharedMaterials != null)
            {
                for (int i = 0; i < rend.sharedMaterials.Length; i++)
                {
                    Material mat = rend.sharedMaterials[i];
                    if (mat != null && mat.HasProperty(mainColorPropertyName))
                    {
                        // 설정된 모든 targetMaterialNames 중 하나라도 포함하는지 확인
                        foreach (string targetName in targetMaterialNames)
                        {
                            if (mat.name.Contains(targetName))
                            {
                                // 해당 머티리얼의 초기 색상을 저장
                                _targetedRenderersData.Add(new RendererMaterialData
                                {
                                    Renderer = rend,
                                    MaterialIndex = i,
                                    InitialColor = mat.GetColor(mainColorPropertyName)
                                });
                                break; // 이 렌더러의 현재 머티리얼이 일치하는 이름을 찾았으니 다음 머티리얼로 이동
                            }
                        }
                    }
                }
            }
        }

        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        if (_targetedRenderersData.Count == 0)
        {
            Debug.LogWarning($"[HitState] CacheRenderersAndInitialColors: No relevant renderers/materials found for {gameObject.name} matching targetMaterialNames.");
        }
    }

    private IEnumerator HitFlashEffectCoroutine(float duration)
    {
        // 1. 즉시 highlightVValue로 변경
        SetAllRenderersColorToVValue(highlightVValue);

        // 2. 짧은 시간 동안 최대 밝기 유지 (예: 전체 duration의 20%)
        float peakDuration = duration * 0.2f;
        float elapsedTime = 0f;
        while (elapsedTime < peakDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // 3. 나머지 시간 동안 normalVValue로 점진적 변경
        float transitionDuration = duration - peakDuration;
        elapsedTime = 0f; // 타이머 리셋

        if (transitionDuration > 0)
        {
            while (elapsedTime < transitionDuration)
            {
                float t = elapsedTime / transitionDuration; // 0에서 1로
                foreach (var data in _targetedRenderersData)
                {
                    Renderer rend = data.Renderer;
                    Color initialColor = data.InitialColor;

                    Color.RGBToHSV(initialColor, out float H, out float S, out _);
                    float currentV = Mathf.Lerp(highlightVValue, normalVValue, t);
                    Color newColor = Color.HSVToRGB(H, S, currentV);

                    // MaterialPropertyBlock을 사용하여 특정 머티리얼 인덱스에 색상 적용
                    rend.GetPropertyBlock(_propertyBlock);
                    _propertyBlock.SetColor(mainColorPropertyName, newColor);
                    rend.SetPropertyBlock(_propertyBlock, data.MaterialIndex); // 특정 인덱스에 적용
                }
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        // 4. 최종적으로 normalVValue 적용 (또는 원래 색상으로 복구)
        SetAllRenderersColorToVValue(normalVValue); // 모든 대상 렌더러에 적용

        _hitFlashCoroutine = null;
    }

    // 모든 대상 렌더러의 해당 머티리얼에 특정 V 값을 적용하는 헬퍼 메서드
    private void SetAllRenderersColorToVValue(float vValue)
    {
        if (_targetedRenderersData.Count == 0 || _propertyBlock == null) return;

        foreach (var data in _targetedRenderersData)
        {
            Renderer rend = data.Renderer;
            Color initialColor = data.InitialColor;

            Color.RGBToHSV(initialColor, out float H, out float S, out _);
            Color targetColor = Color.HSVToRGB(H, S, vValue);

            rend.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(mainColorPropertyName, targetColor);
            rend.SetPropertyBlock(_propertyBlock, data.MaterialIndex); // 특정 인덱스에 적용
        }
    }

    // 초기 색상으로 모든 대상 렌더러의 머티리얼을 복구하는 헬퍼 메서드
    private void ApplyOriginalColorsToAllRenderers()
    {
        if (_targetedRenderersData.Count == 0 || _propertyBlock == null) return;

        foreach (var data in _targetedRenderersData)
        {
            Renderer rend = data.Renderer;
            Color initialColor = data.InitialColor;

            rend.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(mainColorPropertyName, initialColor);
            rend.SetPropertyBlock(_propertyBlock, data.MaterialIndex); // 특정 인덱스에 적용
        }
    }
    #endregion 피격 머테리얼 효과
}