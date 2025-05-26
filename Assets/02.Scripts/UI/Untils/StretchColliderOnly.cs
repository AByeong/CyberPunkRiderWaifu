using UnityEngine;
using UnityEngine.Serialization;

public class StretchColliderOnly : MonoBehaviour
{
    public GameObject LineEffect;
    public float TargetHeight = 1f;

    private BoxCollider _boxCollider;
    private float _originalHeight;
    private Vector3 _originalCenter;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();

        _originalHeight = _boxCollider.size.y;
        _originalCenter = _boxCollider.center;

        StretchDown(TargetHeight);
    }

    public void StretchDown(float height)
    {
        _boxCollider.size = new Vector3(
            _boxCollider.size.x,
            height,
            _boxCollider.size.z
        );

        float delta = (height - _originalHeight) / 2f;

        // 항상 originalCenter를 기준으로 보정
        _boxCollider.center = new Vector3(
            _originalCenter.x,
            _originalCenter.y + delta,
            _originalCenter.z
        );
        UpdateLineEffectPosition();
    }
    private void UpdateLineEffectPosition()
    {
        if (LineEffect == null) return;

        // 콜라이더의 월드 위치 + 회전 고려해서 끝점 계산
        Vector3 topOffset = Vector3.up * (_boxCollider.size.y / 2f) * transform.lossyScale.y;
        Vector3 worldTop = transform.TransformPoint(_boxCollider.center + topOffset);

        LineEffect.transform.position = worldTop;
    }
}
