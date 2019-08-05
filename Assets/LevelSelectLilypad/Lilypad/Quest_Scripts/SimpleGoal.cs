using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGoal : Goal
{
	public void ReceiveEventFinishGoal()
    {
        // don't complete inactive goals
        if (!m_IsActiveGoal) { return; }

        // don't repeat on completed goals
        if (m_IsGoalComplete) { return; }

        m_IsGoalComplete = true;
        m_SendEventOnGoalComplete.Invoke();

        // placeholder parameters for values set in inspector
        m_SendEventIntArgOnGoalComplete.Invoke(0);
        m_SendEventBoolArgOnGoalComplete.Invoke(false);
    }
}
