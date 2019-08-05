using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleElement : MonoBehaviour {
    public void Toggle()
    {
        MaskableGraphic element = GetComponent<MaskableGraphic>();
        element.enabled = !(element.enabled);
    }
}
