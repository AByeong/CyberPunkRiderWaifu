using UnityEngine;

public class FollowYRotationOnly : MonoBehaviour
{
    public Transform target;
    public Vector3 fixedPosition;
    public float rotationSpeed = 5f;

    void Start()
    {
        transform.position = fixedPosition;
    }

    void LateUpdate()
    {
        transform.position = fixedPosition;

        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.001f)
            return;

        // LookRotation 계산
        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
        Vector3 targetEuler = lookRotation.eulerAngles;

        // X축 (상하 시선) 회전 제한
        float rawX = targetEuler.x;
        float angleX = Mathf.DeltaAngle(0f, rawX);
        float clampedX = Mathf.Clamp(angleX, 15f, 45f); // 예: 상하 시야 제한
        float currentX = transform.eulerAngles.x;
        float newX = Mathf.LerpAngle(currentX, clampedX, Time.deltaTime * rotationSpeed);

        // Y축 회전 제한
        float rawY = targetEuler.y;
        float angleY = Mathf.DeltaAngle(0f, rawY);
        float clampedY = Mathf.Clamp(angleY, 4f, 24f);
        float currentY = transform.eulerAngles.y;
        float newY = Mathf.LerpAngle(currentY, clampedY, Time.deltaTime * rotationSpeed);

        // 최종 회전 적용
        transform.rotation = Quaternion.Euler(newX, newY, 0f);
    }
}
