using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogBounds : MonoBehaviour {
    public Transform playerTransform;
    public Transform cameraTransform;

    public float fogMin;
    public float fogMax;
    public int xBound;
    public int zBound;
    public int padding;

    private void Start()
    {
        RenderSettings.fogDensity = fogMin;
    }
    // Update is called once per frame
    void Update () {
        Vector3 cameraOffset = cameraTransform.position - playerTransform.position;
        //Calculates distance from edge from the position on the opposite side of the player from the camera so fog is relative to direction
        float xPadding = xBound - Mathf.Abs((playerTransform.position - cameraOffset).x);
        float zPadding = zBound - Mathf.Abs((playerTransform.position - cameraOffset).z);
        if (xPadding < padding || zPadding < padding) //Player is within distance from edge of map
        {
            //Increase fog as transform nears edge
            RenderSettings.fogDensity = fogMax * (1 - (1 - fogMin / fogMax) * Mathf.Min(xPadding,zPadding) / padding);
            //Checks for player position curbing
            if (Mathf.Abs(playerTransform.position.x) >= xBound)
                playerTransform.position = new Vector3(xBound * Mathf.Sign(playerTransform.position.x), playerTransform.position.y, playerTransform.position.z);
            if (Mathf.Abs(playerTransform.position.z) >= zBound)
                playerTransform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, zBound * Mathf.Sign(playerTransform.position.z));
        }
	}
}
