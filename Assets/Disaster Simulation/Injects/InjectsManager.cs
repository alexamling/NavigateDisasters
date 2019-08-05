using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * NSF REU Serious Geogame and Spatial Thinking Research (2nd Year)
 * Elliot Privateer
 * Class created to start and manage the injects that appear in the game
*/
public class InjectsManager : MonoBehaviour
{
    // Variables used throughout the class
    #region Variables
    // List of all the injects able to be used
    List<InjectNode> injects;

    // In scene objects to be modified
    public GameObject display;
    public Text mainText;
    public List<GameObject> buttons;

    // Node that manages the current Inject
    InjectNode currentNode;

    // Booleans used to work through inject logic
    bool started;
    bool selected;
    bool earlyEnd;

    // Local value that holds the chosen interval value
    int chosenValue;

    // Multiplier for calculating and adding to overall score
    int multiplier;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Sets up necessary defaults
        injects = GetComponent<ImportScript>().injects;
        started = false;
        selected = false;
        earlyEnd = false;
    }

    // Update function with a test debug that is currently inactive
    #region Update
    // Update is called once per frame
    void Update()
    {
        // Small chance to activate inject based on random number and if an inject is already started
        //if (Random.Range(0f, 1f) > .99f && started == false)
        //    StartInject(1, 0);
    }
    #endregion

    // Functions that handle moving through the Inject system node architecture and
    // handles any changes in inject state and score modifications
    #region InjectFunction
    /// STARTINJECT
    /// Description:
    /// Prepares the inject for user input.  Sets up buttons and text
    /// to use for traversing the injects.
    public void StartInject(float delay, float delayVariance)
    {
        // Changes started to true in order to prevent a new inject from overwriting the current one
        started = true;

        // Turns display on so user can see first phase of inject
        display.SetActive(true);

        // Sets current node to a random inject from the list
        currentNode = injects[Random.Range(0, injects.Count)];

        // Set thte main text of the UI to reflect that of the starting text of the inject
        mainText.text = currentNode.main;

        // Sets up choices for user based on the numnber of choices in the array for the Inject node
        for(int x = 0; x < currentNode.numChoices; x++)
        {
            buttons[x].GetComponentInChildren<Text>().text = currentNode.choices[x];
            buttons[x].SetActive(true);
        }

        // Start the inject
        StartCoroutine(ProcessInject(delay, delayVariance));
    }

    /// PROCESSINJECT
    /// COROUTINE
    /// Description:
    /// Goes through each of the sections of the inject and makes sure there is user input
    IEnumerator ProcessInject(float delay, float delayVariance)
    {
        // Wait until an option has been chosen.  It changes selected to true and allows the routine to continue
        // If a chosen option is connected to a negative interval before the final section, it changes earlyEnd to true.
        // When earlyEnd is true, display the result from the inject and provide a continue button which exits the inject early.
        yield return new WaitUntil(() => selected == true);
        yield return new WaitForSeconds(delay + Random.Range(-delayVariance, delayVariance));
        

        // Process next set of events
        ProcessChanges(chosenValue);

        // Checks if earlyEnd is true, begins the end of the inject
        selected = false;
        if(earlyEnd == true)
            EndInject();
        
        yield return new WaitUntil(() => selected == true);
        // When Early end is active, reset UI elements and scripts and break from the coroutine
        if (earlyEnd == true)
        {
            Camera.main.GetComponent<MapController>().score += SetScore(100);
            started = false;
            selected = false;
            earlyEnd = false;
            ResetButtons();
            yield break;
        }
        yield return new WaitForSeconds(delay + Random.Range(-delayVariance, delayVariance));
        
        // Process next set of events
        ProcessChanges(chosenValue);
        selected = false;

        // Checks if earlyEnd is true, begins the end of the inject
        if (earlyEnd == true)
            EndInject();

        yield return new WaitUntil(() => selected == true);
        // When Early end is active, reset UI elements and scripts and break from the coroutine
        if (earlyEnd == true)
        {
            Camera.main.GetComponent<MapController>().score += SetScore(100);
            started = false;
            selected = false;
            earlyEnd = false;
            ResetButtons();
            yield break;
        }
        yield return new WaitForSeconds(delay + Random.Range(-delayVariance, delayVariance));
        
        // Process next set of events
        ProcessChanges(chosenValue);
        selected = false;

        yield return new WaitUntil(() => selected == true);
        yield return new WaitForSeconds(delay + Random.Range(-delayVariance, delayVariance));
        Camera.main.GetComponent<MapController>().score += SetScore(100);
        // Reset buttons values
        ResetButtons();

        // Reset default values in preparation for a new inject
        selected = false;
        started = false;
        yield return null;
    }

    /// PROCESSCHANGES
    /// Description:
    /// Takes the user input and uses it to change the next steps for the inject
    void ProcessChanges(int value)
    {
        // Activates display for user to see the next options
        display.SetActive(true);

        // Sets the canvas text to the previously chosen result
        mainText.text = currentNode.results[value];

        //Debug.Log(currentNode.scoreMultiplier[value]);
        multiplier = currentNode.scoreMultiplier[value];

        // Checks if the inject is in it's final phase or not
        if (currentNode.localPart < currentNode.localMax)
        {
            // Changes current node to the next node in line
            currentNode = currentNode.nextNode;

            // Local array to hold new set of intervals
            string[] local = currentNode.intervals[value].Split('^');

            // Checks if the inject is ending early
            if (local[0].Contains("-1"))
            {
                ResetButtons();
                chosenValue = 0;
                earlyEnd = true;
                return;
            }

            // Local value for handling changing button data
            int index = 0;

            // Go through, change the text and value of each of the buttons and activate it for user to interact
            for (int x = int.Parse(local[0]); x < int.Parse(local[1]); x++)
            {
                buttons[index].GetComponentInChildren<Text>().text = currentNode.choices[x];
                buttons[index].GetComponent<ButtonValues>().value = x;
                buttons[index].SetActive(true);
                index++;
            }
        }
        else  // If the final section is finished, display final results and continue button
        {
            EndInject();
        }
    }

    /// ENDINJECT
    /// Description:
    /// Show a single button with the content being 'Continue' to end inject interaction
    void EndInject()
    {
        // Show canvas and button and change the text
        display.SetActive(true);
        buttons[0].SetActive(true);
        buttons[0].GetComponentInChildren<Text>().text = "Continue";
        chosenValue = 0;
    }
    #endregion

    // Buttons functionality that changes based on status of inject
    #region Buttons
    /// RESETBUTTONS
    /// Description:
    /// Changes the button values of each of the buttons back to their defaults
    /// by calling a function local to the ButtonValues script
    void ResetButtons()
    {
        for (int x = 0; x < buttons.Count; x++)
            buttons[x].GetComponent<ButtonValues>().Reset();
    }

    /// GETINPUT
    /// Description:
    /// When button is clicked, check its value, set it to chosenValue and
    /// turn display off in preparation for the next section of the inject
    public void GetInput(ButtonValues value)
    {
        // Changes selected to true in order to continue
        selected = true;

        // Sets chosenValue used to set up intervals for choices
        chosenValue = value.value;

        // Sets UI elements to be inactive
        for (int x = 0; x < buttons.Count; x++)
            buttons[x].SetActive(false);

        display.SetActive(false);
    }
    #endregion

    /// SETSCORE
    /// Description:
    /// Returns float of score increase based on multiplier
    float SetScore(float scoreNum)
    {
        float evaluatedScore = scoreNum * (10 - multiplier);
        return evaluatedScore;
    }
}