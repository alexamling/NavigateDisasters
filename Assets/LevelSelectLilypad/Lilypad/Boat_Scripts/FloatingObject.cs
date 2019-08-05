using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class FloatingObject : MonoBehaviour
{
    public float waterLevel = 0.0f;
    public float floatThreshold = 2.0f;
    public float waterDensity = 0.125f;
    public float downForce = 4.0f;
    private float forceFactor;
    private Vector3 floatForce;
    private Rigidbody RB;

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        forceFactor = 1.0f - ((transform.position.y - waterLevel) / floatThreshold);
        if (forceFactor > 0.0f)
        {
            floatForce = -Physics.gravity * RB.mass * (forceFactor - RB.velocity.y * waterDensity);
            floatForce += new Vector3(0.0f, -downForce * RB.mass, 0.0f);
            RB.AddForceAtPosition(floatForce, transform.position);
        }
    }
}
