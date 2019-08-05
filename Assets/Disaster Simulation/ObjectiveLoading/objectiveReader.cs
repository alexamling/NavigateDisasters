using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;

/// <summary>
/// Class for reading in files of objectives, adding them to a list of objectives, 
/// and instantiating them under a parnet object in the scene
/// </summary>
public class objectiveReader : MonoBehaviour
{

    #region attributes

    //Files to be read in
    public TextAsset ObjectivesFire;
    public TextAsset ObjectivesFlood;
    public TextAsset ObjectivesAccident;
    public TextAsset ObjectivesPersonal;

    //lists to store objective classes created from read-in information
    public List<PlayerObjective> fireList;
    public List<PlayerObjective> floodList;
    public List<PlayerObjective> accidentList;
    public List<PlayerObjective> personalList;

    //object for cloning using read-in information
    public GameObject objectivePrefab;

    //object to parent instantiated objectives under
    public GameObject parent;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Read in each file to the associated list
        readFile(ObjectivesFire, fireList);
        readFile(ObjectivesFlood, floodList);
        readFile(ObjectivesAccident, accidentList);
        readFile(ObjectivesPersonal, personalList);
    }

    /// <summary>
    /// Function that reads in a passed in file, generates player objectives based on the info,
    /// and adds the new objectives to the passed in list
    /// </summary>
    /// <param name="textFile"></param>
    /// <param name="list"></param>
    public void readFile(TextAsset textFile, List<PlayerObjective> list)
    {
        //Split out all the unecessary info (it's only there for human readability), the join the substrings into a
        //new, cohesive text string
        string[] newStrings = textFile.text.Split(new String[] { "Score: ", "Location: ", "ImmediateResponseModifiers: ", "DelayedResponseModifiers: ", "NotificationTitle: ", "FullMessage: ", "TimeLimit: ", "NeedsResponse: ", "\n", "  " }, StringSplitOptions.RemoveEmptyEntries);
        string newString = String.Join("", newStrings);

       //Split each objective into separate substrings. Each substring now contains information for one objective each
        string[] mainObjectives = newString.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string mainObjective in mainObjectives)
        {
            //Instantiate a clone of the objective prefab and set it as a child of the set parent object
            PlayerObjective objective = Instantiate(objectivePrefab).GetComponent<PlayerObjective>();
            objective.transform.SetParent(parent.transform);

            //Split each substring into a series of substrings containing data for 1 class attribute each
            string[] subObjectives = mainObjective.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            //Assign the read-in info to the instantiated clone
            for (int i = 0; i < subObjectives.Length; i++)
            {
                objective.score = float.Parse(subObjectives[0], CultureInfo.InvariantCulture.NumberFormat);
                objective.location = new Vector2(float.Parse(subObjectives[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[2], CultureInfo.InvariantCulture.NumberFormat));
                objective.immediateResponseModifiers = new float[] { float.Parse(subObjectives[3], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[4], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[5], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[6], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[7], CultureInfo.InvariantCulture.NumberFormat) };
                objective.delayedResponseModifiers = new float[] { float.Parse(subObjectives[8], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[9], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[10], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[11], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[12], CultureInfo.InvariantCulture.NumberFormat) };
                objective.notificationTitle = subObjectives[13];
                objective.fullMessage = subObjectives[14];
                objective.timeLimit = float.Parse(subObjectives[15], CultureInfo.InvariantCulture.NumberFormat);
                objective.units = new int[] { 0, 0, 0, 0, 0 };
                if (subObjectives[16].ToUpper().Contains("TRUE"))
                {
                    objective.needsResponse = true;
                }
                else if (subObjectives[16].ToUpper().Contains("FALSE"))
                {
                    objective.needsResponse = false;
                }
            }
            list.Add(objective);
        }
    }
}
