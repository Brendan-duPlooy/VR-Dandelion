using System.Collections.Generic;
using UnityEngine;

public class DandelionGenerator : MonoBehaviour
{
    [Header("Petal Prefabs")]
    public List<GameObject> petalPrefabs = new();

    [Header("Generation")]
    public int petalCount = 100;
    public float radius = 0.15f;

    [Header("References")]
    public Transform center;
    public Transform petalParent;

    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        // Delete old petals
        foreach (Transform child in petalParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < petalCount; i++)
        {
            float phi =
                Mathf.Acos(1 - 2 * (i + 0.5f) / petalCount);

            float theta =
                Mathf.PI * (1 + Mathf.Sqrt(5)) * (i + 0.5f);

            Vector3 direction = new Vector3(
                Mathf.Cos(theta) * Mathf.Sin(phi),
                Mathf.Sin(theta) * Mathf.Sin(phi),
                Mathf.Cos(phi)
            );

            Vector3 position =
                center.position +
                direction * radius;

            GameObject prefab =
                petalPrefabs[Random.Range(0, petalPrefabs.Count)];

            GameObject petal =
                Instantiate(prefab, position, Quaternion.identity, petalParent);

            petal.transform.up = direction;

            PetalSway sway = petal.GetComponent<PetalSway>();

            if (sway != null)
            {
                sway.Initialize(center);
            }

            // Randomize child Particle System simulation speed
            Transform childPS = petal.transform.Find("Particle System");

            if (childPS != null)
            {
                ParticleSystem ps = childPS.GetComponent<ParticleSystem>();

                if (ps != null)
                {
                    var main = ps.main;
                    main.simulationSpeed = Random.Range(0.02f, 0.15f);
                }
            }
        }
    }
}