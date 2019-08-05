using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerHidden : MonoBehaviour
{
    public float m_TimeLimit;
    public UnityEvent m_SendEventOnTimeOut;

    private float m_StartTime;

    void Start ()
    {
        m_StartTime = Time.time;
    }
	
	void Update ()
    {
        if (m_StartTime + m_TimeLimit < Time.time)
        {
            m_SendEventOnTimeOut.Invoke();
            gameObject.SetActive(false);
        }
    }
}
