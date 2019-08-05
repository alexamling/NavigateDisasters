using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles requesting and adding new units during gameplay, as well as much of the UI for doing so
/// </summary>
public class mutualAidManager : MonoBehaviour
{

    #region attributes

    public enum AidState { Standby, Arrived, Sending, Returning, Inactive };
    public AidState aidState = AidState.Standby;

    private int[] unitRangesA = new int[10]; //units to send ranges for county A
    private int[] unitRangesB = new int[10]; //units to send ranges for county B
    private int[] unitRangesC = new int[10]; //units to send ranges for county C
    public int[] selectedRange; //Currently selected range, based on dropdown input
    public int[] unitsToAdd = new int[] { 0, 0, 0, 0, 0 }; //Aid units to add to overall unit count

    //list of places already requested from
    public List<int[]> blackList = new List<int[]>();

    public Text description;
    public Text rangesText;
    public Dropdown dropdown;
    public Button requestButton;

    public int minUnitRange = 0; //minimum possible available units to recieve
    public int maxUnitRange = 3; //maximum possible available units to recieve
    public int unitRangeVariance = 1; //amount the individual range minimums may be grater than the global minimum

    //transport car stuff
    public Image aidCar;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public float step = 0;
    public float travelTimeRoundTrip = 30; //travel time of the car in seconds

    public ManageUnits unitManager;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //generate intial ranges of the number of units a county can provide
        GenerateRanges(unitRangesA);
        GenerateRanges(unitRangesB);
        GenerateRanges(unitRangesC);

        //Display initial range (default of dropdown)
        selectedRange = unitRangesA; 
        DisplayUnits();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (unitManager.controller.manager.gameTimer.gameState == GameState.Running)
        {
            if (aidState != AidState.Inactive)
            {
                #region standby

                //if the game is running and the aidCar is on stnadby, update display text based on whether or not
                //aid has already been requested from the slected county
                if (aidState == AidState.Standby)
                {
                    for (int i = 0; i < blackList.Count; i++)
                    {
                        if (selectedRange == blackList[i])
                        {
                            requestButton.interactable = false;
                            description.text = "You've Already Requested Units from this County\nPlease Select Another";
                        }
                    }
                }

                #endregion

                #region returning

                //If aid is on it's way back, lerp until it reaches the start point, turn around, and enter standby
                else if (aidState == AidState.Returning)
                {
                    Vector3 newPosition = new Vector3(Vector3.Lerp(endPoint, startPoint, step).x, aidCar.transform.localPosition.y, aidCar.transform.localPosition.z);
                    aidCar.transform.localPosition = newPosition;
                    step += 1 / ((1 / Time.fixedDeltaTime) * (travelTimeRoundTrip / 2));

                    if (aidCar.transform.localPosition.x == startPoint.x)
                    {
                        //turn off the interactables while in transit
                        dropdown.interactable = true;
                        requestButton.interactable = true;
                        description.text = "Potential Aid Available for Request:" + "\n\n" + "Standing By";
                        step = 0;
                        aidCar.transform.Rotate(new Vector3(0, 1, 0), 180); //flip sprite for standby
                        DisplayUnits();
                        aidState = AidState.Standby;
                    }
                }

                #endregion

                #region sending

                //if on the way: lerp to endpoint, set to arrived, display what units were gained
                else if (aidState == AidState.Sending)
                {
                    Vector3 newPosition = new Vector3(Vector3.Lerp(startPoint, endPoint, step).x, aidCar.transform.localPosition.y, aidCar.transform.localPosition.z);
                    aidCar.transform.localPosition = newPosition;
                    step += 1 / ((1 / Time.fixedDeltaTime) * (travelTimeRoundTrip / 2));

                    if (aidCar.transform.localPosition.x == endPoint.x)
                    {
                        description.text = "Aid has Arrived:\n +" + unitsToAdd[0] + " EMS, +" + unitsToAdd[1] + " FireDept, +" + unitsToAdd[2] + " Military,\n +" + unitsToAdd[3] + " Police, +" + unitsToAdd[4] + " Volunteer Groups\nReturning...";
                        aidState = AidState.Arrived;
                    }
                }

                #endregion

                #region arrived

                //if arrived: add requested units to unit total, turn around and begin returning
                else if (aidState == AidState.Arrived)
                {
                    for (int i = 0; i < unitsToAdd.Length; i++)
                    {
                        unitManager.availibleUnits[i] += unitsToAdd[i];
                        unitManager.resourceValues[i].text = "" + unitManager.availibleUnits[i];
                    }

                    aidCar.transform.Rotate(new Vector3(0, 1, 0), 180); //flip sprite for retrun trip
                    step = 0;
                    aidState = AidState.Returning;
                }

                #endregion
            }
        }
    }

    /// <summary>
    /// Generate the initial ranges that the counties can have
    /// </summary>
    /// <param name="range"></param>
    void GenerateRanges(int[] range)
    {
        for (int i = 0; i < range.Length; i++)
        {
            if (i % 2 == 0) //if even index (low range)
            {
                range[i] = minUnitRange + Random.Range(0, unitRangeVariance + 1); //rnage is minimum with variance
            }

            else if (i % 2 != 0) //if odd index (high range)
            {
                range[i] = Random.Range(range[i - 1], maxUnitRange + 1);
            }
        }
    }

    /// <summary>
    /// Generate what units will be sent given the range of the current county
    /// </summary>
    void PopulateUnits()
    {
        for (int i = 0; i < unitsToAdd.Length; i++)
        {
            unitsToAdd[i] = Random.Range(selectedRange[i * 2], selectedRange[i * 2 + 1]);

            unitManager.controller.totalRequestedUnits += unitsToAdd[i];
        }
    }

    /// <summary>
    /// Display the current ranges for the currently selected county
    /// </summary>
    public void DisplayUnits()
    {
        int[] range = selectedRange;
        rangesText.text = range[0] + " - " + range[1] + "       " + range[2] + " - " + range[3] + "    " + range[4] + " - " + range[5] + "      " + range[6] + " - " + range[7] + "     " + range[8] + " - " + range[9];
    }

    /// <summary>
    /// Connected to request button, populates units and sends them
    /// </summary>
    public void Request()
    {
        blackList.Add(selectedRange); //add the currently slected range to the blacklist (not to be used again)
        PopulateUnits();
        for (int i = 0; i < selectedRange.Length; i++)
        {
            selectedRange[i] = 0; //set the used range's new values all to 0
        }
        description.text = "Mutual Aid En Route, Please Wait for Resources to Arrive.";
        aidState = AidState.Sending; //send the car
        dropdown.interactable = false;
        requestButton.interactable = false;
    }

    /// <summary>
    /// Change the current selected range as an onChange event of the dropdown
    /// </summary>
    public void UpdateSlectedRange()
    {
        if (dropdown.value == 0)
        {
            selectedRange = unitRangesA;
        }
        else if (dropdown.value == 1)
        {
            selectedRange = unitRangesB;
        }
        else if (dropdown.value == 2)
        {
            selectedRange = unitRangesC;
        }

        requestButton.interactable = true;
        description.text = "Potential Aid Available for Request:" + "\n\n" + "Standing By";
    }
}
