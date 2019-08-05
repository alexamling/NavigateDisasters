using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { Paused, Completed, Running };

public class gameTimer : MonoBehaviour
{

    #region attributes

    public Text scoreText; //final score display

    //Attributes for final stats
    public Text successfulObj;
    public Text failedObj;
    public Text ignoredObj;
    public Text ignoredObJIdeal;
    public Text unitsSent;
    public Text unitsRequested;

    //Game timer stuff
    public float currentTime;
    public float timeLimit = 600;
    public Text sceneTimer; //game timer display

    private MapController mapControl;
    private GameObject gameOverPanel;
    public GameState gameState = GameState.Running;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        currentTime = timeLimit;
        mapControl = GameObject.Find("Main Camera").GetComponent<MapController>();

        gameOverPanel = GameObject.Find("gameOver");
        gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (gameState == GameState.Running)
        {
            //Display the current time at each fixed pdate
            if (currentTime > 0.1f)
            {
                currentTime -= Time.fixedDeltaTime;
                int minutes = Mathf.FloorToInt(currentTime / 60.0f);
                int seconds = Mathf.FloorToInt(currentTime - (minutes * 60));
                sceneTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            //If timer hits zero (or near it) complete the game and print final stats
            if (currentTime <= 0.1f)
            {
                gameState = GameState.Completed;
                gameOverPanel.SetActive(true);
                scoreText.text = "Final Score" + "\n" + Mathf.FloorToInt(mapControl.score);

                successfulObj.text = "Objectives Successfully Resolved: " + mapControl.playerControls.sucessfulObjectivesCount;
                failedObj.text = "Objectives Failed: " + mapControl.playerControls.failedObjectivesCount;
                ignoredObj.text = "Objectives Ignored/Passed On: " + mapControl.playerControls.ignoredObjectivesActual;
                ignoredObJIdeal.text = "Ideal Number of Objectives to Ignore: " + mapControl.playerControls.ignoredObjectivesIdeal;
                unitsSent.text = "Total Units Sent: " + mapControl.playerControls.totalSentUnits;
                unitsRequested.text = "Units Requested/Mutual Aid: " + mapControl.playerControls.totalRequestedUnits;
            }
        }
        
        else if (gameState == GameState.Paused)
        {
            //unused for now
        }

        else if (gameState == GameState.Completed)
        {
            //unused for now
        }
    }
}
