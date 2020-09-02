using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour
{
    public float mass;
    public float radius;
    public Vector3 velocity;

    // Start is called before the first frame update
    void OnValidate()
    {
        Transform meshHolder = transform.GetChild(0);
        meshHolder.localScale = Vector3.one * radius;
    }

    public void UpdateVelocity(Vector3 acceleration, float timeStep)
    {
        velocity += acceleration * timeStep;
    }

    public void UpdatePosition(float timeStep)
    {
        transform.position += velocity * timeStep;
    }
}
