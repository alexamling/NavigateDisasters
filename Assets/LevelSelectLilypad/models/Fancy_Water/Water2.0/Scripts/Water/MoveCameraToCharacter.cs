using UnityEngine;
using System.Collections;

public class MoveCameraToCharacter : MonoBehaviour {

    public GameObject Target;
	
	// Update is called once per frame
	void Update () {
        transform.position = Target.transform.position;
	}
}
