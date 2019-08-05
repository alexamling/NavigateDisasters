using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * NSF REU Serious Geogame and Spatial Thinking Research (2nd Year)
 * Elliot Privateer
 * Class used to hold data for determining next set of choices and results
*/
public class ButtonValues : MonoBehaviour
{
    // Values used to determine indeces to serch in inject script
    public int value;
    [SerializeField]
    int originalValue;

    // Resets current value to originalValue that is the starting point
    public void Reset()
    {
        value = originalValue;
    }
}
