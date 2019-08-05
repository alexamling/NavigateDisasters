using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Mission2End1 : MonoBehaviour
{
    public GameObject choiceOne;
    public GameObject showChoiceOne;
    public GameObject showChoiceTwo;
    public GameObject overlay;
    public GameObject backButton;
    //public List<TextMeshProUGUI> choiceReviews;
    // New list using normal text UI objects over outdated TMP objects
    public List<Text> newChoiceReviews;

   
    
    private string[] lilypadNames = { "Dickinson High School", "Barber Middle School", "High School Football Field", "West Residential", "South Residential" };
    public List<int> lilypadsUsed = new List<int>();
    private bool[,] lilypadGrades = new bool[5,2] { 
        { true, true },
        { true, true }, 
        { false, true }, 
        { false, false }, 
        { false, false }
    };

    void Start()
    {
        overlay.SetActive(false);
    }

    public void RecordLilypadUsed(int lilypadIndex)
    {
        Debug.Log(lilypadIndex);
        if (lilypadsUsed.Count < 2)
        {
            lilypadsUsed.Add(lilypadIndex);
            PopulateChoice(lilypadsUsed.Count - 1, lilypadIndex);
        }
        else
        {
            Debug.Log("Error: Mission2End.RecordLilyPadUsed(int) attempted to record more than intended (2) uses.");
        }
    }

    private void PopulateChoice(int textIndex, int lilypadIndex)
    {
        //choiceReviews[textIndex].text = "Choice " + (textIndex + 1) + ": Brought people to " + lilypadNames[lilypadIndex] + ".\n";
        //choiceReviews[textIndex].text += (((lilypadGrades[lilypadIndex, 0] == true) && (lilypadGrades[lilypadIndex, 1] == true)) ? "Good Job! " : "") + "The location had " + (lilypadGrades[lilypadIndex, 0] ? "" : "not ") + "enough resources " + 
        //    (lilypadGrades[lilypadIndex, 0] == lilypadGrades[lilypadIndex, 1] ? "and" : "but") + " was " + (lilypadGrades[lilypadIndex, 1] ? "" : "not ") + "recognizeable as a landmark.";

        // Rework of previous code to incorporate new List object
        // Creates string based on choices made when dropping people off
        // at a site.  Puts in List for later display.
        newChoiceReviews[textIndex].text = "Choice " + (textIndex + 1) + ": Brought people to " + lilypadNames[lilypadIndex] + ".\n";
        newChoiceReviews[textIndex].text += (((lilypadGrades[lilypadIndex, 0] == true) && (lilypadGrades[lilypadIndex, 1] == true)) ? "Good Job! " : "") + "The location had " + (lilypadGrades[lilypadIndex, 0] ? "" : "not ") + "enough resources " +
            (lilypadGrades[lilypadIndex, 0] == lilypadGrades[lilypadIndex, 1] ? "and" : "but") + " was " + (lilypadGrades[lilypadIndex, 1] ? "" : "not ") + "recognizeable as a landmark.";
    }

    public void Activate()
    {
        overlay.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Continue()
    {
        overlay.SetActive(false);
        Time.timeScale = 1;
    }

    /// NSF REU
    /// Elliot Privateer
    // Used to display the first choice after completing the first lilypad
    public void DisplayChoices()
    {
        overlay.SetActive(!overlay.activeSelf);
        showChoiceTwo.SetActive(!overlay.activeSelf);
        backButton.SetActive(!backButton.activeSelf);
    }
}
