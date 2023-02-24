using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    private Transform startTransform;
    private Vector3 endPosition;
    private float tracerSpawnDistance;
    private Vector3[] pointArray = new Vector3[2];
    [SerializeField] private LineRenderer thisRenderer;

    public void PassConstraints(Transform start, float tracerSpawnDist, Vector3 end)
    {
        startTransform = start;
        tracerSpawnDistance = tracerSpawnDist;
        endPosition = end;
    }

    private void Update()
    {
        pointArray[0] = startTransform.position + tracerSpawnDistance * startTransform.forward;
        pointArray[1] = endPosition;
    }

    private void LateUpdate()
    {
        thisRenderer.SetPositions(pointArray);
    }
}
