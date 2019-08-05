using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ImportScripting_Test : MonoBehaviour
{
    // Dictionary that holds the name of a file and an 
    // array of strings with it's lines
    public Dictionary<string, string[]> directionValues;

    // Start is called before the first frame update
    void Start()
    {
        // Instantiates dictionary
        directionValues = new Dictionary<string, string[]>();

        // Stores path to search for files and gets directory info from it
        string path = Application.dataPath + "/InGameText";
        DirectoryInfo textDirectory = new DirectoryInfo(path);

        // Checks if the directory exists or not
        if(textDirectory != null)
        {
            // Grabs all the files in the directory
            FileInfo[] data = textDirectory.GetFiles("*.*");

            // Goes through each of the files and skips the meta files 
            for (int x = 0; x < data.Length; x+=2)
            {
                // Takes file name and removes file extension for readability
                string name = data[x].Name.Replace(".txt", "");

                // Creates local array and list to parse through and remove unnecessary elements
                string[] localValues = File.ReadAllLines(path + "/" + data[x].Name);
                List<string> finalValues = new List<string>();
                for(int y = 0; y < localValues.Length; y++)
                {
                    // If a line is not empty, add it to the list
                    if (localValues[y] != string.Empty)
                        finalValues.Add(localValues[y]);
                }

                // Transform list into array and add it to the dictionary
                // with the name string as they key and array as the value
                directionValues.Add(name, finalValues.ToArray());
            }

            // Run through all elements added from test file for debugging
            // TEST
            for (int x = 0; x < data.Length; x += 2)
            {
                for (int y = 0; y < directionValues["Hello"].Length; y++)
                    Debug.Log(directionValues["Hello"][y] + " ");
            }


        }
        else // If directory can not be found, put error message in debug
        {
            Debug.Log("Error - No InGameText folder detected");
        }
       
    }
}
