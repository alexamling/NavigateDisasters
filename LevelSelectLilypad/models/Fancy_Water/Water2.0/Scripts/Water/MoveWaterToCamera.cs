using UnityEngine;
using System.Collections;

public class MoveWaterToCamera : MonoBehaviour {

    public GameObject CurrenCamera;

    void Update()
    {
        if (CurrenCamera==null)
            return;
        var pos = transform.position;
        pos.x = CurrenCamera.transform.position.x;
        pos.z = CurrenCamera.transform.position.z;
        transform.position = pos;
        var rotation = CurrenCamera.transform.rotation;
        rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, 0, rotation.eulerAngles.z);
    }
}
