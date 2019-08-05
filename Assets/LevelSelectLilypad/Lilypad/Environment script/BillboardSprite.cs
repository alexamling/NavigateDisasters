using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    public Camera cam;
    public float halfBounceHeight;

    private float initialHeight;

    private void Start()
    {
        initialHeight = transform.position.y;
    }

    private void Update ()
    {
        // billboard
        transform.LookAt(cam.transform, Vector3.up);
        //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        // bounce
        float heightOffset = Mathf.Sin(Time.time) * halfBounceHeight;
        transform.position = new Vector3(transform.position.x, initialHeight + heightOffset, transform.position.z);
    }
}