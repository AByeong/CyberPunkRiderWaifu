using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LODMaterialController : MonoBehaviour
{
    [Tooltip("변경할 머티리얼의 Shader 프로퍼티 이름 (예: _Value, _Intensity, _HighlightAmount)")]
    public string propertyName = "_YourShaderValue"; // 실제 사용하는 쉐이더의 프로퍼티 이름으로 변경하세요.

    [Tooltip("일시적으로 변경될 값")]
    public float temporaryValue = 100f;

    [Tooltip("원래 값으로 돌아갈 값")]
    public float originalValue = 50f;

    [Tooltip("temporaryValue로 유지될 시간 (초)")]
    public float duration = 2.0f;

    private LODGroup lodGroup;
    private MaterialPropertyBlock propertyBlock;
    private Coroutine resetCoroutine;

    // 모든 LOD 레벨의 모든 렌더러를 캐싱하기 위한 리스트
    private List<Renderer> allManagedRenderers = new List<Renderer>();

    void Awake()
    {
        lodGroup = GetComponentInChildren<LODGroup>(); // 자식 오브젝트에서 LODGroup 탐색
        if (lodGroup == null)
        {
            lodGroup = GetComponent<LODGroup>(); // 현재 오브젝트에서 LODGroup 탐색
        }

        if (lodGroup == null)
        {
            Debug.LogError("LODGroup을 찾을 수 없습니다!", this);
            enabled = false; // 스크립트 비활성화
            return;
        }

        propertyBlock = new MaterialPropertyBlock();

        // LOD Group이 관리하는 모든 렌더러를 수집하고 초기값 설정
        CacheAndInitializeRenderers();
    }

    void CacheAndInitializeRenderers()
    {
        allManagedRenderers.Clear();
        LOD[] lods = lodGroup.GetLODs();
        for (int i = 0; i < lods.Length; i++)
        {
            foreach (Renderer renderer in lods[i].renderers)
            {
                if (renderer != null)
                {
                    allManagedRenderers.Add(renderer);
                    // 초기값 설정
                    renderer.GetPropertyBlock(propertyBlock);
                    propertyBlock.SetFloat(propertyName, originalValue);
                    renderer.SetPropertyBlock(propertyBlock);
                }
            }
        }
    }


    /// <summary>
    /// 현재 활성화된 LOD의 머티리얼 값을 변경하고 일정 시간 후 원래 값으로 되돌립니다.
    /// 이 메서드가 다시 호출되면 타이머가 초기화됩니다.
    /// </summary>
    public void TriggerValueChange()
    {
        if (lodGroup == null) return;

        // 현재 활성화된 (보이는) LOD의 렌더러들에 temporaryValue 적용
        bool foundVisibleRenderer = false;
        foreach (Renderer rend in allManagedRenderers)
        {
            if (rend != null && rend.isVisible) // 현재 보이는 렌더러 (활성 LOD)
            {
                rend.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat(propertyName, temporaryValue);
                rend.SetPropertyBlock(propertyBlock);
                foundVisibleRenderer = true;
            }
        }

        if (!foundVisibleRenderer)
        {
            // Debug.LogWarning("현재 활성화된 (보이는) LOD 렌더러가 없습니다.");
            // 보이지 않더라도 코루틴 로직은 진행하여, 나중에 보이게 될 때를 대비할 수 있습니다.
            // 혹은 여기서 중단할 수도 있습니다. 여기서는 일단 진행합니다.
        }

        // 기존 코루틴이 있다면 중지
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }

        // 새 코루틴 시작
        resetCoroutine = StartCoroutine(ResetValueAfterDelayCoroutine());
    }

    private IEnumerator ResetValueAfterDelayCoroutine()
    {
        yield return new WaitForSeconds(duration);

        // 시간이 지난 후, 관리되는 모든 렌더러의 값을 originalValue로 복원
        // LOD가 그 사이에 변경되었을 수도 있으므로, 모든 렌더러에 적용하는 것이 안전합니다.
        // 이렇게 하면 현재 활성화된 LOD가 무엇이든 원래 값으로 설정됩니다.
        foreach (Renderer rend in allManagedRenderers)
        {
            if (rend != null)
            {
                rend.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat(propertyName, originalValue);
                rend.SetPropertyBlock(propertyBlock);
            }
        }
        resetCoroutine = null; // 코루틴 완료 표시
    }

    // 오브젝트가 비활성화되거나 파괴될 때 정리
    void OnDisable()
    {
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;

            // 비활성화 시 즉시 원래 값으로 되돌립니다 (선택 사항).
            // 이렇게 하면 다음에 활성화될 때 temporaryValue로 남아있는 것을 방지합니다.
            if (lodGroup != null && propertyBlock != null && allManagedRenderers.Count > 0)
            {
                foreach (Renderer rend in allManagedRenderers)
                {
                    if (rend != null)
                    {
                        rend.GetPropertyBlock(propertyBlock);
                        propertyBlock.SetFloat(propertyName, originalValue);
                        rend.SetPropertyBlock(propertyBlock);
                    }
                }
            }
        }
    }
}