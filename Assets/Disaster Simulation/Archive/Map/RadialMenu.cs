using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is used to display an arbitrary number of buttons in a half circle around the location of the menu
/// The player controls class holds references to the different lists of buttons
/// Written by Alexander Amling
/// </summary>

public class RadialMenu : MonoBehaviour
{
    public List<Button> buttons;

    float radius = 100;
    
    void Start()
    {
        buttons = new List<Button>();
    }

    /// <summary>
    /// This method updates the radial menu and displays the passed list of buttons
    /// </summary>
    /// <param name="btns">the buttons to be displayed</param>
    public void Display(List<Button> btns)
    {
        // send old buttons off screen
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].transform.localPosition = new Vector3(-10000, 0, 0);
        }

        float angleDivision = Mathf.PI / (btns.Count - 1);

        // display new buttons
        for (int i = 0; i < btns.Count; i++)
        {
            float angle = -angleDivision * i + Mathf.PI;
            btns[i].transform.SetParent(this.transform);
            btns[i].transform.localPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }
        buttons = btns;
    }

    public void SetPosition(Vector3 newPos)
    {
        this.GetComponent<RectTransform>().position = newPos;
    }
}
