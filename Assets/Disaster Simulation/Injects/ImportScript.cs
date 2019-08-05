using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
/*
 * NSF REU Serious Geogame and Spatial Thinking Research (2nd Year)
 * Elliot Privateer
 * Class designed to grab external text files from a specified location and parses the data to be used in injects game mechanic
*/
public class ImportScript : MonoBehaviour
{
    // Variables used to parse, sort and hold data
    #region Variables
    // List of data structures used to parse and manage inject data
    List<string> choices;
    List<string> results;
    List<string> endScores;
    List<string> intervals;
    List<List<string[]>> injectFormat;
    public List<InjectNode> injects;
    public List<TextAsset> injectFiles;
    #endregion

    // Creates list of injects to be used in game
    void Awake()
    {
        #region Instantiation
        // Instantiate values
        injects = new List<InjectNode>();
        injectFormat = new List<List<string[]>>();
        choices = new List<string>();
        results = new List<string>();
        intervals = new List<string>();
        endScores = new List<string>();
        #endregion

        // Goes through each of the files and skips the meta files 
        for (int x = 0; x < injectFiles.Count; x++)
        {
            // Transform list into array and add it to the dictionary
            // with the name string as they key and array as the value
            string[] tempArray = injectFiles[x].ToString().Split('\n');

            // Variables to hold parsed strings and arrays for separating
            // the different aspects of the inject sections
            string holdCurrent = "";
            string holdMain = "";
            string[] temp;

            // Work through and parse the text data
            for(int y = 0; y < tempArray.Length; y++)
            {
                // If dipsplay is found, new section must be created
                if (tempArray[y].Contains("Display"))
                {
                    // Another local array for parsing strings even further
                    string[] local;

                    // Preps values to parse
                    temp = holdCurrent.Split('_');

                    // Parses strings based on special titles from text file
                    for(int z = 0; z < temp.Length; z++)
                    {
                        local = temp[z].Split(':');
                        if (local[0].Contains("Option"))
                        {
                            choices.Add(local[1]);
                            endScores.Add(local[0]);
                        }
                        else if (local[0].Contains("Result"))
                            results.Add(local[1]);
                        else if (local[0].Contains("Intervals"))
                            intervals.Add(local[1]);
                        else if (local[0].Contains("Inject"))
                            holdMain = local[1];
                    }

                    // After values are parsed, add each parsed array to a local list of arrays
                    // and then clear the holder variables
                    if(!holdCurrent.Contains("Inject"))
                    {
                        List<string[]> array = new List<string[]>();
                        array.Add(intervals.ToArray());
                        array.Add(choices.ToArray());
                        array.Add(results.ToArray());
                        injectFormat.Add(array);
                        intervals.Clear();
                        choices.Clear();
                        results.Clear();
                    }
                        
                    // Resets the test string that 
                    holdCurrent = "";

                }
                else
                {
                    // Add string to collective elements of an inject section
                    holdCurrent += tempArray[y] + "_";
                }
                        
            }

            // Create new inject and add it to list
            InjectNode newNode = new InjectNode(injectFormat, 0, endScores, 0);
            injectFormat.Clear();
            newNode.main = holdMain;
            injects.Add(newNode);
            endScores.Clear();
        }
    }
}
