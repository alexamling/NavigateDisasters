using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void LoadDay1()
    {
        SceneManager.LoadScene("Loading_Day 1");
    }

    public void LoadDay2()
    {
        SceneManager.LoadScene("Loading_Day 2");
    }
    public void QuitGame()
    {
        SceneManager.LoadScene("levelSelect");
    }

    public void PlayCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
