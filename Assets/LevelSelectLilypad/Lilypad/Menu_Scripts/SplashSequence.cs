using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashSequence : MonoBehaviour {

    public static int sceneNumber = 0;
    public int waitTime;

	void Start ()
    {
        if(waitTime < 5)
        {
            waitTime = 5;
        }

        StartCoroutine(AdvanceSplash());
	}
  
    IEnumerator AdvanceSplash()
    {
        yield return new WaitForSeconds(waitTime);
        sceneNumber++;
        SceneManager.LoadScene(sceneNumber);
    }

}
