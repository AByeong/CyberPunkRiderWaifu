using System;
using UnityEngine;
#region 컴포넌트

[Serializable]
public class GroundCheckComponent
{
    public CharacterController CharacterController;
}

#endregion
public class GroundCheck : MonoBehaviour // 플레이어 땅 체크
{
    [SerializeField] private GroundCheckComponent _component;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckOffsetY;

    private bool _isGroundedLast;
    public bool IsGrounded { get; private set; }
    private void Reset()
    {
        _component.CharacterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Check();
        // SetAnimatorValue();
    }
    private void OnDrawGizmos()
    {
        if (_component.CharacterController == null)
        {
            Reset();
        }

        float sphereRadius = _groundCheckRadius;
        Vector3 sphereOrigin = transform.position
            + new Vector3(0, _component.CharacterController.center.y - _component.CharacterController.height * 0.5f + sphereRadius + _groundCheckOffsetY, 0)
            + transform.forward * _component.CharacterController.center.z;

        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(sphereOrigin, sphereRadius);
    }
    private void Check()
    {
        float sphereRadius = _groundCheckRadius;
        Vector3 sphereOrigin = transform.position
            + new Vector3(0, _component.CharacterController.center.y - _component.CharacterController.height * 0.5f + sphereRadius + _groundCheckOffsetY, 0)
            + transform.forward * _component.CharacterController.center.z;
        IsGrounded = Physics.CheckSphere(
            sphereOrigin,
            sphereRadius,
            _groundLayer
        );
    }
    private void SetAnimatorValue()
    {
        if (_isGroundedLast != IsGrounded)
        {
            _isGroundedLast = IsGrounded;
            // _component.Animator.SetBool("IsGrounded", IsGrounded);
        }
    }
}
