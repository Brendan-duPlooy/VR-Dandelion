using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DandelionBloom : MonoBehaviour
{
    [Header("Activation")]
    public KeyCode bloomKey = KeyCode.Space;
    public KeyCode blowKey = KeyCode.Return;
    public KeyCode resetKey = KeyCode.Backspace;

    [Header("Bloom")]
    public float growDuration = 0.6f;
    public float maxRandomDelay = 1.0f;

    [Header("Blow Away")]
    public float flightSpeed = 0.4f;
    public float spiralRadius = 0.01f;
    public float spiralSpeed = 3f;
    public float floatAmount = 0.003f;
    public float floatSpeed = 1.5f;
    public float shrinkTime = 5f;

    [Header("Cone Shape")]
    public float maxFlightDelay = 1.5f;

    [Header("Wind")]
    public Transform windDirection;

    public float resistanceStrength = 0.015f;
    public float resistanceBobAmount = 0.002f;
    public float resistanceBobSpeed = 8f;

    private bool hasBloomed = false;
    private bool hasBlownAway = false;

    private Transform center;

    private class PetalData
    {
        public Transform transform;

        public Vector3 originalScale;
        public Vector3 localPosition;
        public Vector3 flightDirection;

        public Quaternion originalRotation;

        public LineRenderer strand;
        public PetalSway sway;

        public float spiralOffset;
        public float floatOffset;

        public float detachDelay;
    }

    private readonly List<PetalData> petals = new();

    IEnumerator Start()
    {
        yield return null;
        CachePetals();
    }

    void Update()
    {
        //Removed Update() Listening functionality, to new files which reference functions in this file now
        // if (
        //     !hasBloomed &&
        //     Keyboard.current != null &&
        //     Keyboard.current.spaceKey.wasPressedThisFrame
        // )
        // {
        //     hasBloomed = true;
        //     StartCoroutine(BloomRoutine());
        // }

        // if (
        //     hasBloomed &&
        //     !hasBlownAway &&
        //     Keyboard.current != null &&
        //     Keyboard.current.enterKey.wasPressedThisFrame
        // )
        // {
        //     hasBlownAway = true;
        //     BlowAway();
        // }

        // if (
        //     Keyboard.current != null &&
        //     Keyboard.current.backspaceKey.wasPressedThisFrame
        // )
        // {
        //     ResetDandelion();
        // }
    }

    public void BloomFlower()
    {
        if (hasBloomed)
            return;

        hasBloomed = true;
        StartCoroutine(BloomRoutine());
    }

    public void BlowFlower()
    {
        if (!hasBloomed)
            return;

        if (hasBlownAway)
            return;

        hasBlownAway = true;
        BlowAway();
    }

    public void ResetFlower()
    {
        ResetDandelion();
    }

    void CachePetals()
    {
        petals.Clear();

        center = GetComponent<DandelionGenerator>().center;

        Transform petalParent =
            GetComponent<DandelionGenerator>().petalParent;

        foreach (Transform petal in petalParent)
        {
            PetalData data = new();

            data.transform = petal;

            data.originalScale = petal.localScale;

            data.localPosition =
                center.InverseTransformPoint(
                    petal.position
                );

            data.originalRotation =
                petal.rotation;

            data.strand =
                petal.GetComponent<LineRenderer>();

            data.sway =
                petal.GetComponent<PetalSway>();

            if (data.sway != null)
            {
                data.sway.enabled = false;
            }

            data.spiralOffset =
                Random.Range(0f, 100f);

            data.floatOffset =
                Random.Range(0f, 100f);

            data.flightDirection =
            (
                center.TransformPoint(
                    data.localPosition
                ) -
                center.position
            ).normalized;

            petal.position =
                center.position;

            petal.localScale =
                Vector3.zero;

            if (data.strand != null)
            {
                data.strand.enabled = false;
            }

            petals.Add(data);
        }
    }

    IEnumerator BloomRoutine()
    {
        List<PetalData> shuffled =
            new List<PetalData>(petals);

        for (int i = 0; i < shuffled.Count; i++)
        {
            int randomIndex =
                Random.Range(
                    i,
                    shuffled.Count
                );

            (shuffled[i], shuffled[randomIndex]) =
                (
                    shuffled[randomIndex],
                    shuffled[i]
                );
        }

        foreach (PetalData petal in shuffled)
        {
            StartCoroutine(
                GrowPetal(
                    petal,
                    Random.Range(
                        0f,
                        maxRandomDelay
                    )
                )
            );
        }

        yield return null;
    }

    IEnumerator GrowPetal(
        PetalData petal,
        float delay
    )
    {
        yield return new WaitForSeconds(delay);

        if (petal.strand != null)
        {
            petal.strand.enabled = true;
        }

        float timer = 0f;
        bool swayActivated = false;

        while (timer < growDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    timer / growDuration
                );

            Vector3 targetPosition =
                center.TransformPoint(
                    petal.localPosition
                );

            petal.transform.localScale =
                Vector3.Lerp(
                    Vector3.zero,
                    petal.originalScale,
                    t
                );

            petal.transform.position =
                Vector3.Lerp(
                    center.position,
                    targetPosition,
                    t
                );

            petal.transform.rotation =
                Quaternion.Slerp(
                    Quaternion.identity,
                    petal.originalRotation,
                    t
                );

            if (
                !swayActivated &&
                t > 0.95f &&
                petal.sway != null
            )
            {
                petal.sway.ResetSpring();
                petal.sway.enabled = true;
                swayActivated = true;
            }

            yield return null;
        }

        petal.transform.localScale =
            petal.originalScale;

        petal.transform.position =
            center.TransformPoint(
                petal.localPosition
            );

        petal.transform.rotation =
            petal.originalRotation;
    }

    void BlowAway()
    {
        if (windDirection == null)
        {
            Debug.LogWarning("No WindDirection assigned!");
            return;
        }

        Vector3 windDir =
            (windDirection.position - center.position).normalized;

        float minDot = float.MaxValue;
        float maxDot = float.MinValue;

        foreach (PetalData petal in petals)
        {
            Vector3 dir =
            (
                center.TransformPoint(
                    petal.localPosition
                ) -
                center.position
            ).normalized;

            // Store spread direction perpendicular to wind
            petal.flightDirection =
                Vector3.ProjectOnPlane(
                    dir,
                    windDir
                );

            if (petal.flightDirection.sqrMagnitude < 0.0001f)
            {
                petal.flightDirection =
                    Vector3.Cross(
                        windDir,
                        Vector3.up
                    );

                if (petal.flightDirection.sqrMagnitude < 0.0001f)
                {
                    petal.flightDirection =
                        Vector3.right;
                }
            }

            petal.flightDirection.Normalize();

            float dot =
                Vector3.Dot(dir, windDir);

            petal.detachDelay = dot;

            minDot = Mathf.Min(minDot, dot);
            maxDot = Mathf.Max(maxDot, dot);
        }

        foreach (PetalData petal in petals)
        {
            petal.detachDelay =
                Mathf.InverseLerp(
                    minDot,
                    maxDot,
                    petal.detachDelay
                ) * maxFlightDelay;

            StartCoroutine(
                FlyAway(petal)
            );
        }
    }

    IEnumerator FlyAway(
        PetalData petal
    )
    {
        if (windDirection == null)
            yield break;

        Vector3 windDir =
            (windDirection.position - center.position).normalized;

        Vector3 attachedPosition =
            petal.transform.position;

        float timer = 0f;

        while (timer < petal.detachDelay)
        {
            timer += Time.deltaTime;

            float t =
                timer /
                Mathf.Max(
                    petal.detachDelay,
                    0.0001f
                );

            float bob =
                Mathf.Sin(
                    Time.time *
                    resistanceBobSpeed +
                    petal.floatOffset
                ) *
                resistanceBobAmount;

            Vector3 resistanceOffset =
                windDir *
                resistanceStrength *
                t;

            Vector3 currentAttachedPosition =
                center.TransformPoint(
                    petal.localPosition
                );

            petal.transform.position =
                currentAttachedPosition +
                resistanceOffset +
                Vector3.up * bob;

            yield return null;
        }

        if (petal.sway != null)
        {
            petal.sway.enabled = false;
        }

        if (petal.strand != null)
        {
            petal.strand.enabled = false;
        }

        Vector3 startPos =
            petal.transform.position;

        Vector3 startScale =
            petal.originalScale;

        timer = 0f;

        while (timer < shrinkTime)
        {
            timer += Time.deltaTime;

            float t =
                timer / shrinkTime;

            Vector3 forwardMove =
                windDir *
                flightSpeed *
                timer;

            // Cone starts tiny and widens over time
            float coneRadius =
                Mathf.Lerp(
                    0f,
                    0.25f,
                    t * t
                );

            // Ensure spread direction is perpendicular to wind
            Vector3 spreadDir =
                Vector3.ProjectOnPlane(
                    petal.flightDirection,
                    windDir
                );

            if (spreadDir.sqrMagnitude < 0.0001f)
            {
                spreadDir =
                    Vector3.Cross(
                        windDir,
                        Vector3.up
                    );

                if (spreadDir.sqrMagnitude < 0.0001f)
                {
                    spreadDir = Vector3.right;
                }
            }

            spreadDir.Normalize();

            Vector3 coneOffset =
                spreadDir *
                coneRadius;

            // Build a stable basis around wind direction
            Vector3 side =
                Vector3.Cross(
                    windDir,
                    Vector3.up
                );

            if (side.sqrMagnitude < 0.001f)
            {
                side = Vector3.right;
            }

            side.Normalize();

            Vector3 up =
                Vector3.Cross(
                    side,
                    windDir
                ).normalized;

            // Spiral gets larger as petals travel
            float angle =
                timer *
                spiralSpeed +
                petal.spiralOffset;

            float growingSpiralRadius =
                spiralRadius *
                Mathf.Lerp(
                    0.1f,
                    6f,
                    t
                );

            Vector3 spiral =
            (
                side *
                Mathf.Cos(angle)
                +
                up *
                Mathf.Sin(angle)
            )
            * growingSpiralRadius;

            float floatY =
                Mathf.Sin(
                    timer *
                    floatSpeed +
                    petal.floatOffset
                )
                * floatAmount;

            petal.transform.position =
                startPos +
                forwardMove +
                coneOffset +
                spiral +
                up * floatY;

            petal.transform.localScale =
                Vector3.Lerp(
                    startScale,
                    Vector3.zero,
                    t
                );

            yield return null;
        }

        petal.transform.localScale =
            Vector3.zero;
    }

    void ResetDandelion()
    {
        StopAllCoroutines();

        hasBloomed = false;
        hasBlownAway = false;

        foreach (PetalData petal in petals)
        {
            if (petal.sway != null)
            {
                petal.sway.enabled = false;
            }

            if (petal.strand != null)
            {
                petal.strand.enabled = false;
            }

            petal.transform.position =
                center.position;

            petal.transform.rotation =
                petal.originalRotation;

            petal.transform.localScale =
                Vector3.zero;
        }
    }
}