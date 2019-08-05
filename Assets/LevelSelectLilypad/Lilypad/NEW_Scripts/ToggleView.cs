using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// NSF REU
/// Elliot Privateer
/// Replacement for original paid asset camera changing script
public class ToggleView : MonoBehaviour
{
    // Anchors for camera to lerp to
    public GameObject fpsAnchor;
    public GameObject tpsAnchor;

    // Canvas objects for both views
    public GameObject tpsCanvas;
    public GameObject fpsCanvas;

    // Used to switch camera view
    public int switchView;

    // Finds current start and end positions for camera lerp
    Vector3 startPos;
    Vector3 endPos;

    // Used to determine distance over time for camera position
    float startTime;
    float totalDistance;

    // Determines if camera is lerping or not
    bool lerping;

    // Third person controller; used to change inMap boolean
    GameObject tpController;

    // Start is called before the first frame update
    void Start()
    {
        // Setting up defaults
        lerping = false;
        tpController = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Press to activate view change
        if(Input.GetKeyDown(KeyCode.M))
        {
            // If view is greater than or equal to 4, change back to 0 to reset incrementation
            if (switchView >= 4)
                switchView = 0;

            // Increment view switching
            switchView++;

            // Set start time for use in lerping function
            startTime = Time.time;
        }

        // Sets initial values for lerping to first person view
        if(switchView == 1)
        {
            startPos = tpsAnchor.transform.position;
            endPos = fpsAnchor.transform.position;
            totalDistance = Vector3.Distance(startPos, endPos);

            // Boolean becomes true to start lerping process
            lerping = true;

            // Activates first person elements and deactivates third person ones
            tpsCanvas.SetActive(false);
            fpsCanvas.SetActive(true);

            // Changes boolean only if the parent has the proper object
            if(transform.parent == tpController)
                tpController.GetComponent<ThirdPersonUserControl>().inMap = true;
        }
        else if(switchView == 3)
        {
            startPos = fpsAnchor.transform.position;
            endPos = tpsAnchor.transform.position;
            totalDistance = Vector3.Distance(startPos, endPos);

            // Boolean becomes true to start lerping process
            lerping = true;

            // Activates third person elements and deactivates first person ones
            tpsCanvas.SetActive(true);
            fpsCanvas.SetActive(false);

            // Changes boolean only if the parent has the proper object
            if (transform.parent == tpController)
                transform.parent.GetComponent<ThirdPersonUserControl>().inMap = false;
        }

        // Starts lerping
        if(lerping)
            Lerping();
    }

    /// LERPING
    /// Description:
    /// Takes in set values and 
    void Lerping()
    {
        // Calculates distance covered based on current time and start time of lerp
        float distanceCovered = (Time.time - startTime) * 10f;

        // Determines fraction of journey covered based on total distance and distance already covered
        float fractionJourney = distanceCovered / totalDistance;

        // Lerps camera to new position
        Camera.main.transform.position = Vector3.Lerp(startPos, endPos, fractionJourney);

        // If camera reaches it's end destination, stop lerping and change view value
        if (Camera.main.transform.position == endPos)
        { lerping = false; switchView++; }
    }
}
