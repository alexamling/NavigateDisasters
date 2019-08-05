using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * NSF REU Serious Geogame and Spatial Thinking Research (2nd Year)
 * Elliot Privateer
 * Class designed to be the basis for the Inject game mechanic that holds the necessary data 
*/
public class InjectNode : MonoBehaviour
{
    // Parameters used to set up nodes for the inject
    #region varibales
    // Values used to determine displayed text and check levels of inject
    public string main;
    string endChoice;
    public List<int> scoreMultiplier;

    // Arrays for holding the data for results, choices and intervals used to traverse them
    public string[] intervals;
    public string[] choices;
    public string[] results;
    
    // Node used to hold info for next section of inject
    public InjectNode nextNode;

    // Values used for setup and external files in order to help manage the inject system
    public int localPart;
    public int localMax;
    public int numChoices;
    #endregion

    // Set up Inject and prepare next one
    public InjectNode(List<List<string[]>> injectSections, int part, List<string> endScores, int index)
    {
        // If it's not the beginning, find the intervals for the 
        if(part != 0)
            intervals = injectSections[part][0][0].Split('/');

        // Sets choices and results for this section of the inject
        choices = injectSections[part][1];
        results = injectSections[part][2];

        // Instantiates list
        scoreMultiplier = new List<int>();
       

        // Goes through, checks each of the options and assigns a value based on contents of the string
        for (int x = 0; x < choices.Length; x++)
        {
            // If the string contains 'End', parse the value and split the number from the string
            if (endScores[index].Contains("End"))
                scoreMultiplier.Add(int.Parse(endScores[index].Split('^')[1]));
            else // Add a '-1' as a placeholder
                scoreMultiplier.Add(-1);

            // increase index separately from loop so future iterations don't go over the same elements
            index++;
        }

        // Sets local values and creates new Node with next part, if necessary.
        numChoices = choices.Length;
        localPart = part + 1;
        localMax = injectSections.Count;
        if (localPart < injectSections.Count)
            nextNode = new InjectNode(injectSections, localPart, endScores, index);
    }
}
