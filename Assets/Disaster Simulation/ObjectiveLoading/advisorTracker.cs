using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles Updates to the advisor tips shown for an objective
/// </summary>
public class advisorTracker : MonoBehaviour
{
    #region attributes

    public PlayerControls playerController;

    //messages to change
    public Text[] tipMessages = new Text[3];

    //currently selected objective
    private PlayerObjective curSelected;

    public float lowThreshhold = 100; //values below are low severity
    public float highThreshhold = 200; //values above are high severity, between thresholds is medium

    #endregion

    // Update is called once per frame
    void FixedUpdate()
    {
        //if an objective has been selected
        if (playerController.selectedObjective != null)
        {
            //if the selected objective has changed from what it was previously
            if (playerController.selectedObjective != curSelected)
            {
                UpdateNotes(4);
            }

            //update the reference to the selected objective
            curSelected = playerController.selectedObjective;
        }
    }

    /// <summary>
    /// Function to update the tip text strings, generally called from the advisor call buttons
    /// </summary>
    /// <param name="noteToUpdate"></param>
    public void UpdateNotes(int noteToUpdate)
    {
        switch (noteToUpdate)
        {

            #region Planning (0)

            case 0: //PLANNING hints to severity
                if (playerController.selectedObjective.originalScore < lowThreshhold)
                {
                    tipMessages[0].text = "The current incident is likely low in severity. We don't expect major reprucussions even if it is not swiftly dealt with.";
                }

                else if (playerController.selectedObjective.originalScore >= lowThreshhold && playerController.selectedObjective.score < highThreshhold)
                {
                    tipMessages[0].text = "The current incident is likely of moderate severity. Major reprucussions may occur.";
                }

                else if (playerController.selectedObjective.originalScore >= highThreshhold)
                {
                    tipMessages[0].text = "The current situation is severe. Proceed with caution.";
                }

                break;

            #endregion

            #region Logistics (1)

            case 1: //LOGISTICS hints to effective units
                int logRange = Random.Range(0, 99); //Generate a random int (essentially a percentage 1-100%)
                float[] bestMod = new float[] { 0, 0 }; //index, value: best modifier in list
                float[] backupMod = new float[] { 0, 0 }; //index, value: second best modifier in list

                //Copy of the modifier list for the current selected objective
                float[] backupModifiers = new float[playerController.selectedObjective.delayedResponseModifiers.Length];

                #region arrayParsing

                //Parse the modifiers and find the largest one, store it's index location and value in bestMod
                for (int i = 0; i < playerController.selectedObjective.delayedResponseModifiers.Length; i++)
                {
                    if (playerController.selectedObjective.delayedResponseModifiers[i] > bestMod[1])
                    {
                        bestMod[1] = playerController.selectedObjective.delayedResponseModifiers[i];
                        bestMod[0] = i;
                    }
                }

                //Parse again and create a copy of the modifier list in backupModifiers, replace bestMod's value with 0
                for (int i = 0; i < playerController.selectedObjective.delayedResponseModifiers.Length; i++)
                {
                    backupModifiers[i] = playerController.selectedObjective.delayedResponseModifiers[i];
                    if (i == bestMod[0])
                    {
                        backupModifiers[i] = 0;
                    }
                }

                //Parse the new array of modifiers (with the previous higest value now set to 0) to find the (2nd) highest value
                for (int i = 0; i < backupModifiers.Length; i++)
                {
                    if (backupModifiers[i] > backupMod[1])
                    {
                        backupMod[1] = backupModifiers[i];
                        backupMod[0] = i;
                    }
                }

                #endregion

                #region textUpdating

                switch (logRange)
                {
                    case int n when (n >= 0 && n < 10): //10% chance to give the 2nd best answer
                        switch (backupMod[0])
                        {
                            case 0:
                                tipMessages[1].text = "The unit type most effective for this incident is likely EMS";
                                break;
                            case 1:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Fire Dept.";
                                break;
                            case 2:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Military";
                                break;
                            case 3:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Police";
                                break;
                            case 4:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Volunteer Groups";
                                break;
                        }

                        break;

                    case int n when (n >= 10 && n < 100): //90% chance to correctly give the best option
                        switch (bestMod[0])
                        {
                            case 0:
                                tipMessages[1].text = "The unit type most effective for this incident is likely EMS";
                                break;
                            case 1:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Fire Dept.";
                                break;
                            case 2:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Military";
                                break;
                            case 3:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Police";
                                break;
                            case 4:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Volunteer Groups";
                                break;
                        }

                        break;
                }

                break;

                #endregion

            #endregion

            #region Operations (2)

            case 2: //OPERATIONS hints to needs response or not
                int range = Random.Range(0, 9);

                //if current objective actually should be responded to
                #region needsResponse
                if (playerController.selectedObjective.needsResponse)
                {
                    switch (range)
                    {
                        case int n when (n >= 0 && n < 1): //10% chance to be wrong
                            {
                                tipMessages[2].text = "Local units appear to have things under control, it may be better to ignore this incident and move on.";
                                break;
                            }

                        case int n when (n >= 1 && n < 6): //40% chance to be right, vague
                            {
                                tipMessages[2].text = "Local units are stetched thin. They may have things covered, but additional resources should be sent if available.";
                                break;
                            }

                        case int n when (n >= 6 && n < 10): //40% chance to be right, certain
                            {
                                tipMessages[2].text = "Local units are overwhelmed and unable to resolve the situation. Additinal aid is needed.";
                                break;
                            }
                    }
                }

                #endregion

                //if current objective should be passed on
                #region !needsResponse
                else if (!playerController.selectedObjective.needsResponse)
                {
                    switch (range)
                    {
                        case int n when (n >= 0 && n < 1): //10% chance to be wrong
                            {
                                tipMessages[2].text = "The incident seems to be too much for local units to handle, you are advised to send support";
                                break;
                            }

                        case int n when (n >= 1 && n < 6): //40% chance to be right, vague
                            {
                                tipMessages[2].text = "Local units appear to have things covered; it's likely best to ignore this and move on.";
                                break;
                            }

                        case int n when (n >= 6 && n < 10): //40% chance to be right, certain
                            {
                                tipMessages[2].text = "Local units have got things covered; it's best to ignore this and move on.";
                                break;
                            }
                    }
                }

                break;

            #endregion

            #endregion

            case 4: //A default case of sorts that resets the text strings when a new objective is accessed
                for (int i = 0; i < tipMessages.Length; i++)
                {
                    tipMessages[i].text = "Contact the associated advisor for more information.";
                }
                break;

            default:
                break;
        }
    }
}
