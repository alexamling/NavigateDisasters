using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour {
    Slider slider;
	// Use this for initialization
	void Awake () {
        slider = gameObject.GetComponent<Slider>();
        slider.value = AudioListener.volume;
        slider.onValueChanged.AddListener(SetVolume);
	}
	
	void SetVolume(float value)
    {
        AudioListener.volume = value;
    }
}
