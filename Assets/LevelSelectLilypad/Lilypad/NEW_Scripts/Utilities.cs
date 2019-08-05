using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    /// GETVALUEFROMRANGETRANSFER
    /// Description:
    /// Takes in a value based around a range of values and
    /// translates an equivalent value using a new range defined
    /// by user.
    public float GetValueFromRangeTransfer(float originalValue, float oldMax, float oldMin, float newMax, float newMin)
    {
        float oldRange = oldMax - oldMin;
        float newRange = newMax - newMin;

        float newValue = (((originalValue - oldMin) * newRange) / oldRange) + newMin;

        return newValue;
    }

    /// ROUNDVALUE
    /// Description:
    /// Takes in value and amount of decimals to round to.
    /// Rounds inputted value to a certain decimal place.
    public int RoundValue(float valueToRound, float zeroesToRound)
    {
        int roundedValue = 0;

        

        roundedValue = (int)(valueToRound / Mathf.Pow(10f, zeroesToRound));

        

        return roundedValue;
    }
}
