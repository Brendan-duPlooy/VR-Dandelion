using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DandelionStrand : MonoBehaviour
{
    private Transform center;

    [SerializeField] private float curveAmount = 0.005f;

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();

        // Find the "Middle" object under the same parent
        if (transform.parent != null)
        {
            center = transform.parent.parent.Find("Middle");
        }

        if (center == null)
        {
            Debug.LogError($"Could not find a GameObject named 'Middle' under {transform.parent?.name}");
            enabled = false;
            return;
        }

        lr.positionCount = 7;
        lr.startWidth = 0.001f;
        lr.endWidth = 0.0001f;
    }

    void Update()
    {
        Vector3 start = center.position;
        Vector3 end = transform.position;

        Vector3 outward = (end - start).normalized;

        lr.positionCount = 7;

        for (int i = 0; i < 7; i++)
        {
            float t = i / 6f;

            Vector3 point =
                Vector3.Lerp(start, end, t);

            float curveA =
                Mathf.Sin(t * Mathf.PI) *
                curveAmount;

            float curveB =
                Mathf.Sin(t * Mathf.PI * 2f) *
                curveAmount *
                0.4f;

            point += outward * (curveA + curveB);

            lr.SetPosition(i, point);
        }
    }
}