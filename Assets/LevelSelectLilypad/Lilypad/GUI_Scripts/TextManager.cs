using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/* Jordan Machalek
* Attached to MAIN_GUI_Canvas > TextOverlay
* Handles display of instructional and narrative text prompts to the screen
* Some class functionality derived from: 
* http://answers.unity.com/answers/16229/view.html
* Ben-Foote at https://forum.unity.com/threads/whats-the-least-boring-way-to-do-subtitles.17753/
*/
public class TextManager : MonoBehaviour
{
    // Variables
    public GameObject prevLineButtonObject;
    public GameObject nextLineButtonObject;
    public UnityEvent m_SendEventOnFinishTextAsset;
    private bool shouldSendEventOntFinishTextAsset;
    private Text textScript; // Associated text script used for display
    private List<TextSet> currentText;
    private int currentTextLineIndex;

    // Use this for initialization
    void Awake ()
    {        
        textScript = GetComponent<Text>();
        currentTextLineIndex = 0;
        shouldSendEventOntFinishTextAsset = false;
    }

    /// <summary>
    /// Sets the current text file to display from
    /// </summary>
    public void SetText(TextAsset textAsset)
    {
        currentText = Interpret(textAsset);
    }

    /// <summary>
    /// Adds a list of strings to the current displayed collection
    /// </summary>
    public void AddText(List<string> newText)
    {
        if (newText.Count <= 0) { return; }

        bool wasAtEndOfText = currentTextLineIndex == currentText.Count;

        // add
        foreach (string line in newText)
        {
            currentText.Add(new TextSet(line, GetColor(line)));
        }

        if (wasAtEndOfText)
        {
            // update display
            textScript.text = currentText[currentTextLineIndex].text;

            // update next button
            nextLineButtonObject.SetActive(true);
        }
    }

    /// <summary>
    /// Takes a TextAsset and converts it to a List<string>
    /// DEV NOTE: I'm not gonna lie, what I'm doing in this method is probably pretty redundant but it makes sense to include at the current
    /// moment. Even if I or someone else is able to do the same thing in 1 or 2 lines later after looking at what I'm doing in it currently, 
    /// it will have served its purpose and I'll still be happy.
    /// </summary>
    /// <param name="file">TextAsset file that is to be converted to a Queue</param>
    List<TextSet> Interpret(TextAsset file)
    {
        // Convert lines from file to a string array.
        string[] dataLines = file.text.Split('\n');
        // Transfer the strings to a List for easier usage.
        List<TextSet> data = new List<TextSet>();

        foreach (string line in dataLines)
        {
            data.Add(new TextSet(line, GetColor(line)));
        }
        return data;
    }

    /// <summary>
    /// Display the first line from the current file
    /// </summary>
    public void ReadText()
    {
        shouldSendEventOntFinishTextAsset = true;
        currentTextLineIndex = -1;
        prevLineButtonObject.SetActive(false);
        StepForwardLine();
    }

    /// <summary>
    /// Sets next line of text from the current file
    /// </summary>
    public void StepForwardLine()
    {
        currentTextLineIndex += 1;

        // If there is text left, assign to current line
        if (currentTextLineIndex < currentText.Count) 
        {
            // update ui
            textScript.text = currentText[currentTextLineIndex].text;
            textScript.color = currentText[currentTextLineIndex].color;
            nextLineButtonObject.SetActive(true);
        }
        else // Clear the line once the current file is through
        {
            // update ui
            textScript.text = null;
            nextLineButtonObject.SetActive(false);

            // notify narrative goals that dialogue finished
            if (shouldSendEventOntFinishTextAsset)
            {
                m_SendEventOnFinishTextAsset.Invoke();

                // only notify on first finish per text asset
                shouldSendEventOntFinishTextAsset = false;
            }
        }

        // show the back button when not at beginning of file
        if ((prevLineButtonObject.activeInHierarchy == false) && (currentTextLineIndex > 0))
        {
            prevLineButtonObject.SetActive(true);
        }
    }

    /// <summary>
    /// Sets previous line of text from the current file
    /// </summary>
    public void StepBackLine()
    {
        // can't step back if there is no previous line
        if (currentTextLineIndex <= 0) { return; }

        // change subtitle to previous line in file
        currentTextLineIndex -= 1;
        textScript.text = currentText[currentTextLineIndex].text;
        textScript.color = currentText[currentTextLineIndex].color;

        // hide the back button when arriving at beginning of file
        if (currentTextLineIndex <= 0)
        {
            prevLineButtonObject.SetActive(false);
        }
        // show the forward button when not at end of file
        if ((nextLineButtonObject.activeInHierarchy == false) && (currentTextLineIndex + 1 >= currentText.Count))
        {
            nextLineButtonObject.SetActive(true);
        }
    }

    /// <summary>
    /// Returns a color based on the name of the speaker in a line of text.
    /// </summary>
    /// <param name="text">Line of text to find color for</param>
    /// <returns></returns>
    private Color GetColor(string text)
    {
        int speakEnd = text.IndexOf(':'); //Colon represents the end of the indicator of the speaker
        if (speakEnd > 0)
        {
            string speaker = text.Substring(0, speakEnd);
            if (speaker.Equals("Dispatcher")) return new Color(0, .75f, 0);
            if (speaker.Equals("Tent Volunteer")) return new Color(0, .75f, 1);
            if (speaker.Equals("House Resident")) return new Color(.625f, .25f, 0);
            if (speaker.Equals("Person") || speaker.Equals("Person 1") || speaker.Equals("Person 2")) return new Color(1, .625f, 0);
            //Put other speakers here
        }
        return Color.white;
    }
}

/// <summary>
/// Holds a line of text and the color of that text.
/// </summary>
class TextSet
{
    public string text;
    public Color color;

    public TextSet(string textLine = "")
    {
        text = textLine;
        color = Color.white;
    }
    public TextSet(string textLine, Color textColor)
    {
        text = textLine;
        color = textColor;
    }
}
