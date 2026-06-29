using UnityEngine;

public class ConnectLine : MonoBehaviour
{
    public Transform startObject; 
    public Transform vfxObject;   
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // Ensure the line only uses two points (start and end)
        lineRenderer.positionCount = 2; 
    }

    void Update()
    {
        if (startObject != null && vfxObject != null)
        {
            // Update the start position
            lineRenderer.SetPosition(0, startObject.position);
            // Update the end position
            lineRenderer.SetPosition(1, vfxObject.position);
        }
    }
}
