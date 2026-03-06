using UnityEngine;

public class ConstantRotate : MonoBehaviour
{
    [Header("Enable")]
    [SerializeField]
    private bool isRotating = true;

    [Header("Speed (Degrees Per Second)")]
    [SerializeField]
    private float rotationSpeed = 90f;

    [Header("Rotation Axis (X, Y, Z)")]
    [SerializeField]
    private float rotationX = 0f;

    [SerializeField]
    private float rotationY = 1f;

    [SerializeField]
    private float rotationZ = 0f;

    [Header("Options")]
    [SerializeField]
    private bool normalizeAxis = true;

    [SerializeField]
    private Space rotationSpace = Space.Self;

    [Header("Rigidbody Support")]
    [SerializeField]
    private bool useRigidbodyIfAvailable = true;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    private void Update()
    {
        if (ShouldUseRigidbody())
        {
            return;
        }

        RotateTransform(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!ShouldUseRigidbody())
        {
            return;
        }

        RotateRigidbody(Time.fixedDeltaTime);
    }

    private bool ShouldUseRigidbody()
    {
        if (!useRigidbodyIfAvailable)
        {
            return false;
        }

        if (rb == null)
        {
            return false;
        }

        return true;
    }

    private Vector3 GetAxis()
    {
        Vector3 axis = new Vector3(rotationX, rotationY, rotationZ);

        if (normalizeAxis)
        {
            if (axis.sqrMagnitude > 0.000001f)
            {
                axis = axis.normalized;
            }
        }

        return axis;
    }

    private Quaternion GetDeltaRotation(float deltaTime)
    {
        Vector3 axis = GetAxis();
        if (axis.sqrMagnitude <= 0.000001f)
        {
            return Quaternion.identity;
        }

        float angle = rotationSpeed * deltaTime;
        return Quaternion.AngleAxis(angle, axis);
    }

    private void RotateTransform(float deltaTime)
    {
        if (!isRotating)
        {
            return;
        }

        Quaternion delta = GetDeltaRotation(deltaTime);
        if (delta == Quaternion.identity)
        {
            return;
        }

        if (rotationSpace == Space.Self)
        {
            transform.localRotation = transform.localRotation * delta;
        }
        else
        {
            transform.rotation = transform.rotation * delta;
        }
    }

    private void RotateRigidbody(float deltaTime)
    {
        if (!isRotating)
        {
            return;
        }

        Quaternion delta = GetDeltaRotation(deltaTime);
        if (delta == Quaternion.identity)
        {
            return;
        }

        if (rotationSpace == Space.Self)
        {
            Quaternion target = rb.rotation * delta;
            rb.MoveRotation(target);
        }
        else
        {
            Quaternion target = delta * rb.rotation;
            rb.MoveRotation(target);
        }
    }

    private void OnValidate()
    {
        if (float.IsNaN(rotationSpeed) || float.IsInfinity(rotationSpeed))
        {
            rotationSpeed = 0f;
        }

        if (float.IsNaN(rotationX) || float.IsInfinity(rotationX))
        {
            rotationX = 0f;
        }

        if (float.IsNaN(rotationY) || float.IsInfinity(rotationY))
        {
            rotationY = 0f;
        }

        if (float.IsNaN(rotationZ) || float.IsInfinity(rotationZ))
        {
            rotationZ = 0f;
        }
    }
}