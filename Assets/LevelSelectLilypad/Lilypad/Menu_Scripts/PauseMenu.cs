using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject PauseMenuUI;

    public GameObject resumeButtonObject;
    public GameObject timeoutMessage1Object;
    public GameObject timeoutMessage2Object;
    public StickerManager stickerManager;
    public Canvas fpCavas;

    /// NSF REU
    /// Elliot Privateer
    // Elements used to handle checkpoints overlay when
    // activated from the pause menu
    public GameObject overlay;
    public GameObject overlayContinue;
    public GameObject coordinateInfoCanvas;
    bool coordinateCanvasActivation;
    ///

    private bool isTimedOut;

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isTimedOut)
        {
            if (isPaused)
            {
                Resume();
                fpCavas.GetComponent<MapZoom>().paused = false;
            }
            else
            {
                Pause();
                fpCavas.GetComponent<MapZoom>().paused = true;
            }
        }
	}

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        stickerManager.m_IsInteractionDisabled = false;

        

        CleanupTimeout();
    }

    private void CleanupTimeout()
    {
        isTimedOut = false;
        resumeButtonObject.SetActive(true);
        timeoutMessage1Object.SetActive(false);
        timeoutMessage2Object.SetActive(false);
    }

    private void Timeout()
    {
        isTimedOut = true;
        Pause();
        resumeButtonObject.SetActive(false);
    }

    public void TimeoutMission1()
    {
        Timeout();
        timeoutMessage1Object.SetActive(true);
    }

    public void TimeoutMission2()
    {
        Timeout();
        timeoutMessage2Object.SetActive(true);
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        stickerManager.m_IsInteractionDisabled = true;

        /// NSF REU SUMMER 2018
        /// Elliot Privateer
        // For button in checkpoints overlay that returns to pause menu 
        // if the checkpoints overlay is up from the pause menu.
        if (overlay.activeSelf)
        { overlay.SetActive(false); overlayContinue.SetActive(false); }
        ///
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    /// NSF REU SUMMER 2018
    /// Elliot Privateer
    // Brings up checkpoints overlay for user to see their first choice
    // made in bringing people to a site.
    public void ShowOverlay()
    {
        overlay.SetActive(true);
        overlayContinue.SetActive(true);
    }

    // Shows canvas for info on determining coordinates
    public void ShowCoordinateCanvas()
    {
        coordinateCanvasActivation = !coordinateCanvasActivation;
        coordinateInfoCanvas.SetActive(coordinateCanvasActivation);

    }
    ///
}
