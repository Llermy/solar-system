using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OrbitDisplay : MonoBehaviour
{
    public int numSteps;
    public float timeStep;
    public bool hideAtPlay;
    public float lineThickness;

    public bool relativeToBody;
    public GravityBody centralBody;

    VirtualBody[] vrBodies;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying && hideAtPlay)
        {
            HideOrbits();
        }
    }

    private void OnValidate()
    {
        if(lineThickness > 0)
        {
            GravityBody[] bodies = FindObjectsOfType<GravityBody>();
            foreach (GravityBody body in bodies)
            {
                var lineRenderer = body.gameObject.GetComponentInChildren<LineRenderer>();
                if (lineRenderer != null)
                {
                    lineRenderer.widthMultiplier = lineThickness;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            DrawOrbits();
        }
    }

    void DrawOrbits()
    {
        GravityBody[] bodies = FindObjectsOfType<GravityBody>();
        vrBodies = new VirtualBody[bodies.Length];
        Vector3[][] drawPoints = new Vector3[bodies.Length][];
        Vector3 referenceBodyInitialPosition = Vector3.zero;
        int referenceBodyIndex = -1;

        for (int i = 0; i < bodies.Length; i++)
        {
            vrBodies[i] = new VirtualBody(bodies[i]);
            drawPoints[i] = new Vector3[numSteps];

            if (bodies[i] == centralBody && relativeToBody)
            {
                referenceBodyIndex = i;
                referenceBodyInitialPosition = vrBodies[referenceBodyIndex].position;
            }
        }

        // Simulate
        for (int step = 0; step < numSteps; step++)
        {
            Vector3 referenceBodyPosition = relativeToBody ? vrBodies[referenceBodyIndex].position : Vector3.zero;

            // Update velocities
            for (int i = 0; i < vrBodies.Length; i++)
            {
                vrBodies[i].velocity += CalcAccelerationAt(vrBodies[i].position, vrBodies[i]) * timeStep;
            }
            // Update positions
            for (int i = 0; i < vrBodies.Length; i++)
            {
                vrBodies[i].position = vrBodies[i].position + vrBodies[i].velocity * timeStep;
                var referenceFrameOffset = referenceBodyPosition - referenceBodyInitialPosition;
                drawPoints[i][step] = i == referenceBodyIndex ? referenceBodyInitialPosition : vrBodies[i].position - referenceFrameOffset;
            }
        }

        // Draw paths
        for (int i = 0; i < vrBodies.Length; i++)
        {
            var pathColour = bodies[i].gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;
            var lineRenderer = bodies[i].gameObject.GetComponentInChildren<LineRenderer>();
            if(lineRenderer != null)
            {
                lineRenderer.enabled = true;
                lineRenderer.positionCount = numSteps;
                lineRenderer.SetPositions(drawPoints[i]);
                lineRenderer.startColor = pathColour;
                lineRenderer.endColor = pathColour;
                lineRenderer.useWorldSpace = true;
            }
        }
    }

    void HideOrbits()
    {
        GravityBody[] bodies = FindObjectsOfType<GravityBody>();

        // Draw paths
        for (int bodyIndex = 0; bodyIndex < bodies.Length; bodyIndex++)
        {
            var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
            if(lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
            }
        }
    }

    public Vector3 CalcAccelerationAt(Vector3 point, VirtualBody bodyToIgnore = null)
    {
        Vector3 acceleration = Vector3.zero;
        foreach (VirtualBody body in vrBodies)
        {
            if (bodyToIgnore != body)
            {
                float sqrDst = (body.position - point).sqrMagnitude;
                Vector3 forceDir = (body.position - point).normalized;
                acceleration += forceDir * Universe.gravitationalConstant * body.mass / sqrDst;
            }
        }
        return acceleration;
    }


    public class VirtualBody
    {
        public float mass;
        public Vector3 velocity;
        public Vector3 position;

        public VirtualBody(GravityBody body)
        {
            this.mass = body.mass;
            this.velocity = body.velocity;
            this.position = body.transform.position;
        }
    }
}
