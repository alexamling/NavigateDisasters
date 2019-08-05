using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectGoal : Goal
{
    [SerializeField]
    private int m_AmountRequired;
    private int m_AmountCollected;

    private bool CheckIsGoalComplete()
    {
        if (m_AmountCollected >= m_AmountRequired)
        {
            m_IsGoalComplete = true;
        }
        else
        {
            m_IsGoalComplete = false;
        }
        return m_IsGoalComplete;
    }

    public void ReceiveEventOnCollectThing()
    {
        // only update uncompleted goals
        if (!m_IsGoalComplete)
        {
            // count collected
            m_AmountCollected++;

            // notify owning quest on completion
            if (CheckIsGoalComplete())
            {
                m_SendEventOnGoalComplete.Invoke();
            }
        }
    }
}
