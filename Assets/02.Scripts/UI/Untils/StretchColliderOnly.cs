using UnityEngine;
public class StretchColliderOnly : MonoBehaviour
{
    public float targetHeight = 3f;
    private BoxCollider boxCollider;
    private float originalHeight;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        originalHeight = boxCollider.size.y;

        StretchDown(targetHeight);
    }

    public void StretchDown(float height)
    {
        // 새 높이 적용
        boxCollider.size = new Vector3(boxCollider.size.x, height, boxCollider.size.z);

        // 아래 방향(-Y)으로만 늘리기 위해 center를 아래로 이동
        float delta = (height - originalHeight) / 2f;
        boxCollider.center = new Vector3(
            boxCollider.center.x,
            boxCollider.center.y + delta,
            boxCollider.center.z
        );
    }
}
