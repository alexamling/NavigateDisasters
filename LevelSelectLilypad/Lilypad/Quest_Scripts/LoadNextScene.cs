using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadNextScene : MonoBehaviour
{
    public void DoThing()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading_Day 2");
    }
}