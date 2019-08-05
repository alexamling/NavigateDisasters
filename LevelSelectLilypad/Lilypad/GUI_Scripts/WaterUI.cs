using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterUI : MonoBehaviour {
    RawImage image;
    Rect translate;
	// Use this for initialization
	void Start () {
        image = gameObject.GetComponent<RawImage>();
        translate = new Rect(0, 0, 1, 1);
	}
	
	// Update is called once per frame
	void Update () {
        translate.x += .01f;
        image.uvRect = translate;
	}
}
