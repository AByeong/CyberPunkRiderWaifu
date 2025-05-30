using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public abstract class Enemy : MonoBehaviour, IDamageable
{
    public string MaterialName;
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
    }

    public virtual void Initialize()
    {
        CurrentHealthPoint = EnemyData.HealthPoint;
        IsHit = false;
        IsInAir = false;
    }

    public void PlayHitParticle()
    {
        CacheRenderersAndInitialColors();
        if (_targetedRenderers != null && _targetedRenderers.Count > 0 && _initialRendererColors.Count > 0)
        {
            if (_hitFlashCoroutine != null) StopCoroutine(_hitFlashCoroutine);

            float staggerDuration = (EnemyData != null && EnemyData.StaggerTime > 0) ? EnemyData.StaggerTime : hitFlashEffectDuration;
            float currentFlashDuration = Mathf.Min(staggerDuration, hitFlashEffectDuration);

            if (currentFlashDuration > 0)
            {
                _hitFlashCoroutine = StartCoroutine(HitFlashEffectCoroutine(currentFlashDuration));
            }
            else // 지속시간이 0이면 즉시 normalVValue 적용 (또는 원래 색상)
            {
                SetRenderersColorToVValue(normalVValue);
            }
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
    [Tooltip("효과를 적용할 머티리얼의 이름 (예: Boid). Owner의 MaterialName 사용 시 비워둘 수 있음")]
    public string targetMaterialName = "KyleRobot"; // 기본값 예시

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

    private List<Renderer> _targetedRenderers;
    private MaterialPropertyBlock _propertyBlock;
    private Dictionary<Renderer, Color> _initialRendererColors = new Dictionary<Renderer, Color>();
    // --- 피격 머테리얼 효과 관련 변수 끝 ---



    private void CacheRenderersAndInitialColors()
    {
        if (!string.IsNullOrEmpty(MaterialName))
        {
            targetMaterialName = MaterialName;
        }

        if (string.IsNullOrEmpty(targetMaterialName))
        {
            Debug.LogWarning($"[HitState] CacheRenderersAndInitialColors: targetMaterialName is not set for {gameObject.name}. Hit flash may not work.");
            return;
        }

        if (_targetedRenderers == null) _targetedRenderers = new List<Renderer>();
        _targetedRenderers.Clear();
        _initialRendererColors.Clear();

        Renderer[] allRenderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in allRenderers)
        {
            if (rend != null && rend.sharedMaterials != null)
            {
                foreach (Material mat in rend.sharedMaterials)
                {
                    // 머티리얼 이름에 targetMaterialName이 포함되어 있고, (Instance)가 붙는 경우도 처리
                    if (mat != null && mat.name.Contains(targetMaterialName))
                    {
                        if (mat.HasProperty(mainColorPropertyName))
                        {
                            if (!_targetedRenderers.Contains(rend)) _targetedRenderers.Add(rend);
                            if (!_initialRendererColors.ContainsKey(rend)) // 첫 번째 일치하는 머티리얼의 색상 저장
                            {
                                _initialRendererColors[rend] = mat.GetColor(mainColorPropertyName);
                            }
                        }
                        break; // 해당 렌더러에서 일치하는 머티리얼 하나 찾으면 다음 렌더러로
                    }
                }
            }
        }

        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }
    }

    private IEnumerator HitFlashEffectCoroutine(float duration)
    {
        // 1. 즉시 highlightVValue로 변경
        SetRenderersColorToVValue(highlightVValue);

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
                foreach (Renderer rend in _targetedRenderers)
                {
                    if (_initialRendererColors.TryGetValue(rend, out Color initialColor))
                    {
                        Color.RGBToHSV(initialColor, out float H, out float S, out _);
                        float currentV = Mathf.Lerp(highlightVValue, normalVValue, t);
                        Color newColor = Color.HSVToRGB(H, S, currentV);

                        rend.GetPropertyBlock(_propertyBlock);
                        _propertyBlock.SetColor(mainColorPropertyName, newColor);
                        rend.SetPropertyBlock(_propertyBlock);
                    }
                }
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        // 4. 최종적으로 normalVValue 적용 (또는 원래 색상으로 복구하려면 ApplyOriginalColors())
        SetRenderersColorToVValue(normalVValue);

        _hitFlashCoroutine = null;
    }

    private void SetRenderersColorToVValue(float vValue)
    {
        if (_targetedRenderers == null || _propertyBlock == null) return;

        foreach (Renderer rend in _targetedRenderers)
        {
            if (_initialRendererColors.TryGetValue(rend, out Color initialColor))
            {
                Color.RGBToHSV(initialColor, out float H, out float S, out _);
                Color targetColor = Color.HSVToRGB(H, S, vValue);

                rend.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor(mainColorPropertyName, targetColor);
                rend.SetPropertyBlock(_propertyBlock);
            }
        }
    }
    #endregion 피격 머테리얼 효과
}
