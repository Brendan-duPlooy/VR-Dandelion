using UnityEngine;

public class SphereSpawner : MonoBehaviour
{
    public GameObject objectToCopy; // Drag your object here in the Inspector
    public int totalCopies = 50;
    public float sphereRadius = 5f;

    void Start()
    {
        for (int i = 0; i < totalCopies; i++)
        {
            // 1. Get a random position on a sphere surface of radius 1
            Vector3 randomDirection = Random.onUnitSphere; 

            // 2. Multiply by the radius and center it around this GameObject
            Vector3 spawnPosition = transform.position + (randomDirection * sphereRadius);

            // 3. Create the copy
            GameObject newCopy = Instantiate(objectToCopy, spawnPosition, Quaternion.identity);

            // Optional: Make the copy face outward from the center
            newCopy.transform.LookAt(transform.position);
            newCopy.transform.Rotate(0, 180, 0); // Flip so its "front" faces out
        }
    }
}
