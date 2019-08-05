using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScroll : MonoBehaviour {

    public int speed = 1;
    public int waitTime;
    public string levelToLoad;

    private void Start()
    {
        StartCoroutine(Wait());
    }
    
    void Update ()
    {
        transform.Translate(Vector3.up * Time.deltaTime * speed);
	}

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(levelToLoad);
    }

    public void Skip()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
