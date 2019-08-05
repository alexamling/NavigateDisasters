using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationTracker : MonoBehaviour
{
    // Creates Utilities class for using special helper functions
    Utilities helper;

    // Values used to find and display coordinates
    float xDistance;
    float zDistance;
    int xBoxIdentifier;
    int zBoxIdentifier;
    string gpsDisplay;

    public GameObject coordinates;
    Text coordinatesText;

    // Start is called before the first frame update
    void Start()
    {
        helper = new Utilities();
        coordinatesText = coordinates.GetComponent<Text>();
    }

    //// Update is called once per frame
    void Update()
    {
        // Calls function to set coordinate values
        DeterminePosition();

        // creates string using the values for display in the GUI
        gpsDisplay = "0" + xBoxIdentifier;

        if (xDistance < 100f)
            gpsDisplay += "0";

        gpsDisplay += helper.RoundValue(xDistance, 1) + " " + zBoxIdentifier;

        if (zDistance < 100f)
            gpsDisplay += "0";

        gpsDisplay += helper.RoundValue(zDistance, 1);

        // Apply to text on UI
        coordinatesText.text = gpsDisplay;
    }

    /// DETERMINE POSITION
    /// Description:
    /// Determines the position of the player on the in-game map
    /// based on their location in world coordinates.
    void DeterminePosition()
    {
        // Statements to determine how far RIGHT on the map you must go
        if(transform.position.x > 510f)
        {
            xBoxIdentifier = 2;
            xDistance = helper.GetValueFromRangeTransfer(transform.position.x, 510f, 1190f, 1000f, 300f);
        }
        else if(transform.position.x < -465f)
        {
            xBoxIdentifier = 4;
            xDistance = helper.GetValueFromRangeTransfer(transform.position.x, -1190f, -465f, 700f, 0f);
        }
        else
        {
            xBoxIdentifier = 3;
            xDistance = helper.GetValueFromRangeTransfer(transform.position.x, -465f, 510f, 1000f, 0f);
        }

        // Statements to determine how far UP on the map you must go
        if (transform.position.z < -670f)
        {
            zBoxIdentifier = 62;
            zDistance = helper.GetValueFromRangeTransfer(transform.position.z, -1140f, -670f, 400f, 0f);
        }
        else if (transform.position.z > 300f)
        {
            zBoxIdentifier = 60;
            zDistance = helper.GetValueFromRangeTransfer(transform.position.z, 300f, 1140f, 1000f, 300f);
        }
        else
        {
            zBoxIdentifier = 61;
            zDistance = helper.GetValueFromRangeTransfer(transform.position.z, -670f, 300f, 1000f, 0f);
        }
    }
}
