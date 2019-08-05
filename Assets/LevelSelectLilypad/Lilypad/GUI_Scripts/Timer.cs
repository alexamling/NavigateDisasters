using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public float timeAllowed;
    private float startTime;
    bool isTicking;

    [SerializeField] Text display;
    public UnityEvent sendEventOnTimeOut;

	// Use this for initialization
	void Start () {
        isTicking = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (isTicking)
        {
            if (startTime + timeAllowed < Time.time)
            {
                isTicking = false;
                sendEventOnTimeOut.Invoke();
            }
            if (display != null)
            {
                display.text = ConvertToTime(timeAllowed - (Time.time - startTime));
            }
        }
    }
    
    public void StartClock(float time)
    {
        timeAllowed = time;
        startTime = Time.time;
        isTicking = true;
    }

    public void ClearClock()
    {
        isTicking = false;
        display.text = "";
    }

    /// <summary>
    /// Converts a time into a formatted string
    /// </summary>
    /// <param name="time">The time, in seconds</param>
    /// <returns></returns>
    private string ConvertToTime(double time)
    {
        string result = "";
        if (time / 3600 >= 1)
        {
            result += ((int)time / 3600).ToString() + ":";
            time %= 3600;
        }
        if (time / 60 >= 1)
        {
            result += ((int)time / 60).ToString() + ":";
            time %= 60;
        }
        result += time.ToString("00.00");
        return result;
    }
}
