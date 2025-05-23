using UnityEngine;

public class StretchColliderOnly : MonoBehaviour
{
    public float targetHeight = 3f;

    private BoxCollider boxCollider;
    private float originalHeight;
    private Vector3 originalCenter;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        originalHeight = boxCollider.size.y;
        originalCenter = boxCollider.center;

        StretchDown(targetHeight);
    }

    public void StretchDown(float height)
    {
        boxCollider.size = new Vector3(
            boxCollider.size.x,
            height,
            boxCollider.size.z
        );

        float delta = (height - originalHeight) / 2f;

        // 항상 originalCenter를 기준으로 보정
        boxCollider.center = new Vector3(
            originalCenter.x,
            originalCenter.y + delta,
            originalCenter.z
        );
    }
}
