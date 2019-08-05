using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScreen : MonoBehaviour
{
    bool isLoading = false;

    [SerializeField]
    private int scene;
    [SerializeField]
    private Text loadingText;
    
    void Update()
    {
        if (!isLoading)
        {
            isLoading = true;
            StartCoroutine(LoadNewScene());
        }
        else
        {
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.Sin(Time.time));
        }
    }

    IEnumerator LoadNewScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);

        while (!async.isDone)
        {
            yield return null;
        }
    }
}
