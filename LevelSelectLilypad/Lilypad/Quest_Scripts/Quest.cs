using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Text goalTextUI;
    [SerializeField]
    private UnityEngine.UI.Text goalDirectionsTextUI;
    [SerializeField]
    private TextManager textManager;
    public Timer timer;
    [SerializeField]
    private int m_QuestIndex;
    public string m_QuestName;
    public List<Goal> m_Goals;
    public UnityEventIntArg m_SendEventOnQuestComplete;
    public List<int> m_NextChainedQuestIndices;
    public GameObject playerObject;

    private bool m_IsQuestComplete;
    public int m_ActiveGoalIndex;

    public void BeginGoal(int goalIndex)
    {
        // verify goal exist at index
        if (goalIndex >= m_Goals.Count) { return; }

        // display on ui panel
        goalTextUI.text = m_Goals[goalIndex].m_GoalName;
        // cleanup old description before setting new
        goalDirectionsTextUI.text = "";
        for (int directionIndex = 0; directionIndex < m_Goals[goalIndex].m_Directions.Count; directionIndex++)
        {
            if (directionIndex > 0)
            {
                goalDirectionsTextUI.text += "\n\n";
            }
            goalDirectionsTextUI.text += m_Goals[goalIndex].m_Directions[directionIndex];
        }

        // enable required gameobjects
        foreach (GameObject gameObject in m_Goals[goalIndex].m_ObjectsToEnable)
        {
            gameObject.SetActive(true);
        }

        // display narrative
        textManager.SetText(m_Goals[goalIndex].m_Subtitles);
        textManager.ReadText();

        // start timer after reading subtitles
        timer.ClearClock();
        textManager.m_SendEventOnFinishTextAsset.AddListener(StartActiveGoalTimer);
        FreezePlayer();

        // events after reading subtitles
        textManager.m_SendEventOnFinishTextAsset.AddListener(ActiveGoalCallsAfterReading);

        // set active
        m_Goals[goalIndex].m_IsActiveGoal = true;
    }

    public void ActiveGoalCallsAfterReading()
    {
        foreach (GameObject gameObject in m_Goals[m_ActiveGoalIndex].m_ObjectsToEnableAfterReading)
        {
            gameObject.SetActive(true);
        }
        textManager.m_SendEventOnFinishTextAsset.RemoveListener(ActiveGoalCallsAfterReading);
    }

    public void StartActiveGoalTimer()
    {
        if (m_Goals[m_ActiveGoalIndex].m_TimeLimit != 0)
        {
            timer.StartClock(m_Goals[m_ActiveGoalIndex].m_TimeLimit);
        }
        textManager.m_SendEventOnFinishTextAsset.RemoveListener(StartActiveGoalTimer);
        UnfreezePlayer();
    }

    public void FreezePlayer()
    {
        if (playerObject.tag == "Player")
        {
            playerObject.GetComponent<ThirdPersonUserControl>().m_IsMovementDisabled = true;
            playerObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            playerObject.GetComponent<Animator>().SetFloat("Forward", 0, 0.0f, Time.deltaTime);
            playerObject.GetComponent<Animator>().SetFloat("Turn", 0, 0.0f, Time.deltaTime);
        }
        else if (playerObject.tag == "Boat")
        {
            playerObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void UnfreezePlayer()
    {
        if (playerObject.tag == "Player")
        {
            playerObject.GetComponent<ThirdPersonUserControl>().m_IsMovementDisabled = false;
            playerObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        else if (playerObject.tag == "Boat")
        {
            playerObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    private bool CheckIsQuestComplete()
    {
        // don't check again if already completed
        if (m_IsQuestComplete) { return true; }

        // quest is complete if all goals are complete
        bool areAllGoalsComplete = true;
        foreach (Goal goal in m_Goals)
        {
            if (!goal.m_IsGoalComplete)
            {
                areAllGoalsComplete = false;
                break;
            }
        }
        if (areAllGoalsComplete)
        {
            m_IsQuestComplete = true;
        }
        return m_IsQuestComplete;
    }

    public void ReceiveEventOnGoalComplete()
    {
        if (CheckIsQuestComplete())
        {
            // notify quest manager on quest completion
            m_SendEventOnQuestComplete.Invoke(m_QuestIndex);
        }
        else
        {
            // set previous goal inactive and current active
            m_Goals[m_ActiveGoalIndex].m_IsActiveGoal = false;
            m_ActiveGoalIndex += 1;
            BeginGoal(m_ActiveGoalIndex);
        }
    }
}