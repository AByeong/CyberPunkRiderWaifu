using System.Collections.Generic;
using UnityEngine;

 
class BoidsController : MonoBehaviour
{
    public GameObject boidPrefab; // 인스턴싱할 프리팹
    public int numberOfBoids = 100;
    public float spawnRadius = 10f;
    public Vector3 bounds = new Vector3(20, 20, 20); // Boid들이 머무를 공간

    private List<Boid> boids;
    private Matrix4x4[] matrices;
    private Mesh boidMesh;
    private Material boidMaterial;
    private MaterialPropertyBlock propertyBlock; // 개별 속성용 (선택)

    void Start()
    {
        if (boidPrefab == null)
        {
            Debug.LogError("Boid Prefab이 할당되지 않았습니다.");
            this.enabled = false;
            return;
        }

        MeshFilter mf = boidPrefab.GetComponent<MeshFilter>();
        MeshRenderer mr = boidPrefab.GetComponent<MeshRenderer>();

        if (mf == null || mr == null || mf.sharedMesh == null || mr.sharedMaterial == null)
        {
            Debug.LogError("Boid Prefab에 MeshFilter, MeshRenderer, Mesh, Material이 모두 필요합니다.");
            this.enabled = false;
            return;
        }

        boidMesh = mf.sharedMesh;
        boidMaterial = mr.sharedMaterial; // GPU Instancing 활성화된 머티리얼 사용

        // GPU Instancing이 머티리얼에서 활성화되어 있는지 확인 (필수!)
        if (!boidMaterial.enableInstancing)
        {
            Debug.LogWarning("Boid 머티리얼에 GPU Instancing이 활성화되어 있지 않습니다. 성능 저하가 발생할 수 있습니다. 머티리얼에서 'Enable GPU Instancing'을 체크하세요.");
            // 필요하다면 코드로 활성화: boidMaterial.enableInstancing = true; (원본 머티리얼 변경 주의)
        }


        boids = new List<Boid>(numberOfBoids);
        matrices = new Matrix4x4[numberOfBoids];
        propertyBlock = new MaterialPropertyBlock(); // 선택적: 개별 색상 등을 위함

        for (int i = 0; i < numberOfBoids; i++)
        {
            Boid b = new Boid();
            b.position = Random.insideUnitSphere * spawnRadius;
            b.velocity = Random.insideUnitSphere * 2f; // 초기 속도
            b.rotation = Quaternion.LookRotation(b.velocity);
            boids.Add(b);
            matrices[i] = Matrix4x4.TRS(b.position, b.rotation, boidPrefab.transform.localScale);
        }
    }

    void Update()
    {
        // Boid 로직 업데이트
        for (int i = 0; i < boids.Count; i++)
        {
            boids[i].flock(boids); // 모든 Boid 리스트를 전달하여 이웃 계산
            boids[i].UpdatePhysics();
            ApplyBounds(boids[i]); // 경계 처리
            matrices[i] = Matrix4x4.TRS(boids[i].position, boids[i].rotation, boidPrefab.transform.localScale);

            // (선택) 인스턴스별 색상 변경 예시
            // propertyBlock.SetColor("_Color", Random.ColorHSV());
            // Graphics.DrawMeshInstanced(boidMesh, 0, boidMaterial, new Matrix4x4[] { matrices[i] }, 1, propertyBlock);
            // 위처럼 하면 개별 Draw Call이 되므로, 아래의 전체 Draw Call 전에 propertyBlock을 설정해야 합니다.
            // 이는 MaterialPropertyBlock을 배열로 만들어 DrawMeshInstanced에 전달하는 더 복잡한 방식으로 가능하나,
            // 여기서는 모든 Boid에 대해 한번에 그리는 것을 우선으로 합니다.
            // 만약 정말로 각 Boid마다 다른 속성이 필요하고 그 수가 많다면 Compute Shader 방식이 더 적합할 수 있습니다.
        }

        // GPU Instancing으로 렌더링
        // 최대 1023개까지의 인스턴스를 한 번의 호출로 그릴 수 있습니다 (플랫폼/API에 따라 다를 수 있음).
        // 그 이상은 여러 번 나눠서 호출해야 합니다.
        int instanceCountPerCall = 1023;
        int remainingInstances = numberOfBoids;
        int batchIndex = 0;

        while(remainingInstances > 0)
        {
            int countInThisBatch = Mathf.Min(remainingInstances, instanceCountPerCall);
            // 임시 배열을 만들어 현재 배치만큼의 매트릭스만 복사합니다.
            Matrix4x4[] batchMatrices = new Matrix4x4[countInThisBatch];
            System.Array.Copy(matrices, batchIndex * instanceCountPerCall, batchMatrices, 0, countInThisBatch);

            Graphics.DrawMeshInstanced(
                boidMesh,
                0, // submesh index
                boidMaterial,
                batchMatrices,
                countInThisBatch,
                propertyBlock // 모든 인스턴스에 동일한 프로퍼티 블록 적용 (비워두면 기본값)
            );

            remainingInstances -= countInThisBatch;
            batchIndex++;
        }
    }

    void ApplyBounds(Boid boid)
    {
        Vector3 pos = boid.position;
        if (pos.x > bounds.x) pos.x = -bounds.x;
        if (pos.x < -bounds.x) pos.x = bounds.x;
        if (pos.y > bounds.y) pos.y = -bounds.y;
        if (pos.y < -bounds.y) pos.y = bounds.y;
        if (pos.z > bounds.z) pos.z = -bounds.z;
        if (pos.z < -bounds.z) pos.z = bounds.z;
        boid.position = pos;
    }
}