using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _mainCamera;

    private void LateUpdate()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                return;
            }
        }

        Transform billboardTarget = _mainCamera.transform;
        Vector3 targetPosition = new Vector3(
            billboardTarget.position.x,
            transform.position.y,
            billboardTarget.position.z);
        
        Vector3 lookDirection = transform.position - targetPosition;
        if (lookDirection.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
    }
}
