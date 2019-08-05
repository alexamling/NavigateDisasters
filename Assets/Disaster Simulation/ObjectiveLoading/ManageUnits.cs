using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles assinging units to objectives and updating the display of available and sent units
/// </summary>
public class ManageUnits : MonoBehaviour
{
    #region attributes

    public GameObject[] unitCounts = new GameObject[5]; //count for units assigned to an objective
    public int[] availibleUnits = new int[5]; //units available to be sent
    private bool isUnitsEmpty = true;
    public GameObject resourceBar; //UI panel containg available unit values
    public GameObject[] elementsUI = new GameObject[12]; //Stores UI references to be used in functions below
    public Text[] resourceValues = new Text[5]; //The text display values of available units in the resource bar

    public PlayerControls controller;
    private MapController mapController;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Set the available units equal to the vlaues displayed in the resource bar
        resourceValues = resourceBar.GetComponentsInChildren<Text>();
        GameObject[] children = new GameObject[5]; 
        for (int i = 0; i < availibleUnits.Length; i++)
        {
            resourceValues[i] = resourceBar.transform.GetChild(i).GetChild(0).GetComponent<Text>();
            resourceValues[i].text = "" + availibleUnits[i];
        }


        mapController = GameObject.Find("Main Camera").GetComponent<MapController>();
    }

    void FixedUpdate()
    {
        if (isUnitsEmpty)
        {
            //if no units are assigned, turn off interactability for the send button
            elementsUI[10].GetComponent<Button>().interactable = false;
        }
        else if (!isUnitsEmpty)
        {
            //if units are assigned, turn on interactability for the send button
            elementsUI[10].GetComponent<Button>().interactable = true;
        }

        //Loop through the unit counts (assigned untis) and set isUnitsEmpty to true if 
        //all assigned unit values are zero (nothing assigned). False if units are assigned
        for (int i = 0; i < unitCounts.Length; i++)
        {
            if (Int32.Parse(unitCounts[i].GetComponent<Text>().text, CultureInfo.InvariantCulture.NumberFormat) != 0)
            {
                isUnitsEmpty = false;
                break;
            }
            else if (Int32.Parse(unitCounts[i].GetComponent<Text>().text, CultureInfo.InvariantCulture.NumberFormat) == 0)
            {
                isUnitsEmpty = true;
            }
        }
    }

    /// <summary>
    /// Hooked up to the ignore button and the notification window 'x's. Closes the notification and removes the objective.
    /// </summary>
    public void ignoreResponse()
    {
        controller.selectedObjective.notification.Close();
    }

    /// <summary>
    /// Hooked up to the send button. Send units to an objective and take them away form the resource bar
    /// </summary>
    public void sendTeam()
    {
        int val;
        int totalUnits = 0;
        for (int i = 0; i < unitCounts.Length; i++)
        {
            //use the string values of assigned units and set the actual units of the selected objective to those
            val = Int32.Parse(unitCounts[i].GetComponent<Text>().text, CultureInfo.InvariantCulture.NumberFormat);
            controller.selectedObjective.units[i] = val;

            //take away the sent units form the resource bar
            availibleUnits[i] -= val;
            resourceValues[i].text = "" + availibleUnits[i];

            //add to temporary value total units
            totalUnits += val;
        }
        //add total unit count to the total sent unit count (used as a stat/metric at the gameover screen)
        controller.totalSentUnits += totalUnits;

        //change the selected objective state from requesting to responding and update the UI
        controller.selectedObjective.objectiveState = ObjectiveState.Responding;
        ToggleUI(controller.selectedObjective);
    }

    /// <summary>
    /// Probably should have been called "UpdateUI" instead. Updates relevant UI based on selected objective
    /// </summary>
    /// <param name="selectedObject"></param>
    public void ToggleUI(PlayerObjective selectedObject)
    {
        //If requesting, turn on the add/subtract and send/ignore buttons. Update title to reflect requesting
        if (selectedObject.objectiveState == ObjectiveState.Requesting)
        {
            for (int i = 0; i < elementsUI.Length - 1; i++)
            {
                elementsUI[i].SetActive(true);
            }
            //title update
            elementsUI[12].GetComponent<Text>().text = "Units Requested";
        }
        //If responding, turn off the interactables. Update title to reflect units sent
        else if (selectedObject.objectiveState == ObjectiveState.Responding)
        {
            for (int i = 0; i < elementsUI.Length - 1; i++)
            {
                elementsUI[i].SetActive(false);
            }
            //title update
            elementsUI[12].GetComponent<Text>().text = "Units Sent";
        }
        //Update the UI unit counts with the units that are at the objective (all zero if nothing has been sent yet)
        for (int i = 0; i < unitCounts.Length; i++)
        {
            unitCounts[i].GetComponent<Text>().text = selectedObject.units[i].ToString();

            //Remember to update with populated values as default
            unitCounts[i].GetComponent<Counter>().value = 0;
        }
    }

    /// <summary>
    /// Returns sent units to available units when an objective succeeeds or fails
    /// </summary>
    /// <param name="obj"></param>
    public void restoreUnits(PlayerObjective obj)
    {
        for (int i = 0; i < unitCounts.Length; i++)
        {
            availibleUnits[i] += obj.units[i];
            resourceValues[i].text = "" + availibleUnits[i];
        }

        //add objective score to toal game score (if objective score is less than 0, add 0 )
        if (obj.score > 0.0f)
        {
            mapController.score += obj.score;
        }
        else
        {
            mapController.score += 0;
        }
    }

}
