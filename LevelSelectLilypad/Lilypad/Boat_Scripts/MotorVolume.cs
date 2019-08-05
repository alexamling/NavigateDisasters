using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class MotorVolume : MonoBehaviour {
    Rigidbody RB;
    AudioSource audio;

    public float maxVolume;
	// Use this for initialization
	void Start () {
        RB = gameObject.GetComponent<Rigidbody>();
        audio = gameObject.GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        //audio.volume = RB.GetPointVelocity().magnitude / maxVolume;
        //Debug.Log(RB.GetPointVelocity(gameObject.transform.position));
	}
}
