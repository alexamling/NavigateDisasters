using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    /// <summary>
    /// Scene to switch to
    /// nullScene used as a default
    /// </summary>
    public string nextSceneName;

    // Start is called before the first frame update
    void Start()
    {
        nextSceneName = "nullScene";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene()
    {
        if (nextSceneName != "nullScene")
        {
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
            nextSceneName = "nullScene";
        }
    }

}
