using UnityEngine;

[ExecuteAlways]
public class SnapTransform : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _posOffset = Vector3.zero;
    [SerializeField] private Vector3 _rotOffset = Vector3.zero;

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        Quaternion targetRotation = _target.rotation;
        Quaternion offsetRotation = Quaternion.Euler(_rotOffset);
        //transform.rotation = targetRotation * offsetRotation;
        
        transform.position = _target.position + targetRotation * _posOffset;
    }
}