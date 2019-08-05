using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/* Author: Jordan Machalek - jdm4344@rit.edu
 * Dev note: I feel like there's an easier way to process the logic here, but I'm just happy
 * it works so I'm not going to try and find a better way to do it. To the nex person working on this,
 * that's your job.
 * 
 * The name of this script is deceptive, it has nothing to do with loading anything;
 * it would be better named as "InstructionScreenManager"
 */

public class LoadingScreenText : MonoBehaviour {

    // Variables
    public Canvas[] instructions; // Array of canvases to switch between on the loading screen
    public string sceneToLoad = ""; // Name of the scene that is to be loaded
    public Button rightButton; // Moves to the next canvas - is kept track of here in order to swap out with the start button
    public Button leftButton; // Moves to the previous canvas - is kept track of here in order to show/hide
    public Button startButton; // Used to load the next scene when the player is done reading what is on the loading screen
    [SerializeField]
    private int textIndex = 0;

    private void Start()
    {
        // Initialize the first canvas to be active and disable all others
        // This isn't strictly necessary but its a good preventive measure because if any canvas other than
        // the first is active when run things will break
        instructions[0].gameObject.SetActive(true);

        for (int i = 1; i < instructions.Length; i++)
        {
            instructions[i].gameObject.SetActive(false);
        }
    }

    // Display next canvas
    // Called OnClick() by Right Button
    public void AdvanceCanvas()
    {
        textIndex++;

        if(textIndex >= instructions.Length - 1) // reached the last screen, change to the start button
        {
            ChangeButton();
        }

        instructions[textIndex].gameObject.SetActive(true);
        instructions[textIndex - 1].gameObject.SetActive(false);

        if (textIndex > 0) leftButton.gameObject.SetActive(true);
    }

    // Display previous canvas
    // Called OnClick() by Left Button
    public void RetreatCanvas()
    {
        if (textIndex == instructions.Length - 1) // moving back from last screen, change back to arrow
        {
            ChangeButton();
        }

        if (textIndex > 0) // only move back if you're not on the first screen
        {
            textIndex--;

            instructions[textIndex].gameObject.SetActive(true);
            if(textIndex != instructions.Length - 1) instructions[textIndex + 1].gameObject.SetActive(false);
        }

        if (textIndex == 0) leftButton.gameObject.SetActive(false);
    }

    // Enables/disables Right+Start buttons
    void ChangeButton()
    {
        if(rightButton.IsActive())
        {
            rightButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(true);
        }
        else
        {
            rightButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
        }
    }

    // Loads the specified mission
    // Called OnCLick() by Start Button
    public void StartMission()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
