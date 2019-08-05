using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointsEOC : MonoBehaviour
{
    private int gameScore =  0;

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Add specified number of points to overall private variable
    /// </summary>
    /// <param name="points"></param>
    public void addPoints(int points)
    {
        gameScore += points;
    }

    /// <summary>
    /// Subtract specified number of points from overall private variable
    /// </summary>
    /// <param name="points"></param>
    public void subtractPoints(int points)
    {
        gameScore -= points;
    }

    /// <summary>
    /// grade the players score and return an int to be interpreted by another script that calls this funtion at the end of the game
    /// [CASE RANGES, INTERPRETATION, AND NUMBER OF CASES SUBJECT TO CHANGE]
    /// </summary>
    /// <returns></returns>
    public int gradeScore()
    {
        switch (gameScore)
        {
            case int n when (n <= 0):
                return 1; //fail case 1
            case int n when (n > 0 && n <= 10):
                return 2; //fail case 2
            case int n when (n > 10 && n <= 20):
                return 3; //success case 1
            case int n when (n > 20):
                return 4; //success case 2
            default:
                return 0; //default case, shouldn't be used
        }
    }
}
