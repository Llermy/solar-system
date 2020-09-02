using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySimulation : MonoBehaviour
{
    GravityBody[] bodies;

    // Start is called before the first frame update
    void Awake()
    {
        bodies = FindObjectsOfType<GravityBody>();
        Time.fixedDeltaTime = Universe.physicsDeltaTime;
    }


    void FixedUpdate()
    {
        foreach (GravityBody body in bodies)
        {
            Vector3 acceleration = CalcAccelerationAt(body.transform.position, body);
            body.UpdateVelocity(acceleration, Time.fixedDeltaTime);
        }

        foreach (GravityBody body in bodies)
        {
            body.UpdatePosition(Time.fixedDeltaTime);
        }
    }

    public Vector3 CalcAccelerationAt(Vector3 point, GravityBody bodyToIgnore = null)
    {
        Vector3 acceleration = Vector3.zero;
        foreach (GravityBody body in bodies)
        {
            if(bodyToIgnore != body)
            {
                float sqrDst = (body.transform.position - point).sqrMagnitude;
                Vector3 forceDir = (body.transform.position - point).normalized;
                acceleration += forceDir * Universe.gravitationalConstant * body.mass / sqrDst;
            }
        }
        return acceleration;
    }
}
