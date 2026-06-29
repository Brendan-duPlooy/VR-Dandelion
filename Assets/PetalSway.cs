using UnityEngine;

public class PetalSway : MonoBehaviour
{
    private Transform center;

    [Header("Spring Settings")]
    [SerializeField] private float springStrength = 20f;
    [SerializeField] private float damping = 5f;
    [SerializeField] private float movementInfluence = 0.0005f;

    private Vector3 localDirection;
    private float radius;

    private Vector3 offset;
    private Vector3 velocity;

    private Vector3 lastCenterPosition;

    public void Initialize(Transform flowerCenter)
    {
        center = flowerCenter;

        Vector3 worldDirection =
            (transform.position - center.position).normalized;

        localDirection =
            center.InverseTransformDirection(worldDirection);

        radius =
            Vector3.Distance(transform.position, center.position);

        lastCenterPosition = center.position;
    }

    public void ResetSpring()
    {
        offset = Vector3.zero;
        velocity = Vector3.zero;

        lastCenterPosition = center.position;
    }

    void Update()
    {
        if (center == null)
            return;

        Vector3 centerVelocity =
            (center.position - lastCenterPosition) /
            Mathf.Max(Time.deltaTime, 0.0001f);

        lastCenterPosition = center.position;

        Vector3 desiredDirection =
            center.TransformDirection(localDirection);

        Vector3 desiredPosition =
            center.position +
            desiredDirection * radius;

        offset += -centerVelocity * movementInfluence;

        Vector3 springForce =
            (desiredPosition - (transform.position + offset))
            * springStrength;

        velocity += springForce * Time.deltaTime;

        velocity *= Mathf.Exp(-damping * Time.deltaTime);

        offset += velocity * Time.deltaTime;

        transform.position =
            desiredPosition + offset;
    }
}