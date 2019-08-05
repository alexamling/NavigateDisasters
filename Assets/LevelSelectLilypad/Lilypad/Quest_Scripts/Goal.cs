using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    public string               m_GoalName;
    public List<GameObject>     m_ObjectsToEnable;
    public List<GameObject>     m_ObjectsToEnableAfterReading;
    public TextAsset            m_Subtitles;
    public List<string>         m_Directions;
    public bool                 m_IsTimeLimited;
    public float                m_TimeLimit;
    public bool                 m_IsGoalComplete;
    public UnityEvent           m_SendEventOnGoalComplete;
    public UnityEventIntArg     m_SendEventIntArgOnGoalComplete;
    public UnityEventBoolArg    m_SendEventBoolArgOnGoalComplete;
    public bool                 m_IsActiveGoal;
}