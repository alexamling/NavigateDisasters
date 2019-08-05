using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour {
    [SerializeField] MeshCollider terrain;
    [SerializeField] Transform trackObject;
    [SerializeField] RectTransform map;
    Transform cursor;

    Vector3 adjustPosition;
    Vector3 mapExtents;
	// Use this for initialization
	void Start () {
        cursor = gameObject.transform;
        mapExtents = new Vector3(map.rect.width / 2, 0, map.rect.height / 2);
	}
	
	// Update is called once per frame
	void Update () {
        adjustPosition = trackObject.position * (mapExtents.magnitude / terrain.bounds.extents.magnitude); //Scales tracker's position from world to map
        adjustPosition = new Vector3(adjustPosition.x, adjustPosition.z, 0);
        cursor.localPosition = -adjustPosition;
        cursor.localRotation = Quaternion.Euler(0, 0, 180 - trackObject.rotation.eulerAngles.y);
	}
}
