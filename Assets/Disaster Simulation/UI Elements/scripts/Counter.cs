using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public int value;
    public int unitIndex = 0;

    private Text number;
    private ManageUnits unitManager;

    // Start is called before the first frame update
    void Start()
    {
        unitManager = FindObjectOfType<ManageUnits>();
        number = GetComponent<Text>();
    }

    public void Increase()
    {
        if (value < unitManager.availibleUnits[unitIndex])
        {
            value++;
            UpdateText();
        }
    }

    public void Decrease()
    {
        if (value > 0)
        {
            value--;
            UpdateText();
        }
    }

    void UpdateText()
    {
        number.text = "" + value;
    }
}
