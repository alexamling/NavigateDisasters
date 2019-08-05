using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class UnityEventIntArg : UnityEvent<int> { }
[System.Serializable]
public class UnityEventBoolArg : UnityEvent<bool> { }

struct Checkpoint
{
    public Vector3 m_PlayerPosition;
    public Quaternion m_PlayerRotation;
}

public class QuestManager : MonoBehaviour
{
    public GameObject player;
    public GameObject followVirtualCamera;
    public TextManager textManager;
    public Timer timer;
    public Text questTextUI;

    public List<Quest> m_Quests;
    private int m_ActiveQuestIndex;
    private Checkpoint m_Checkpoint;

    void Start ()
    {
        SetCheckpoint();

        // start with first quest
        BeginQuest(0);
    }
    
    public void BeginQuest(int questIndex)
    {
        // verify quest exists at index
        if (questIndex < m_Quests.Count)
        {
            //m_ActiveQuests.Add(questIndex);

            // start with first goal of quest
            m_Quests[questIndex].BeginGoal(0);

            // display on ui panel
            questTextUI.text = m_Quests[questIndex].m_QuestName;
        }
    }

    public void FinishQuest(int questIndex)
    {
        //m_ActiveQuests.Remove(questIndex);

        // verify quest exists at index
        if (questIndex < m_Quests.Count)
        {
            // begin next quests in chain
            foreach (int nextChainedQuestIndex in m_Quests[questIndex].m_NextChainedQuestIndices)
            {
                BeginQuest(nextChainedQuestIndex);
            }
        }
    }

    public void SetCheckpoint()
    {
        m_Checkpoint.m_PlayerPosition = player.transform.position;
        m_Checkpoint.m_PlayerRotation = player.transform.rotation;
    }

    public void LoadCheckpoint()
    {
        // teleport
        player.transform.position = m_Checkpoint.m_PlayerPosition;
        player.transform.rotation = m_Checkpoint.m_PlayerRotation;
        
        // physics
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // narrative
        textManager.SetText(m_Quests[m_ActiveQuestIndex].m_Goals[m_Quests[m_ActiveQuestIndex].m_ActiveGoalIndex].m_Subtitles);
        textManager.ReadText();

        // timer
        timer.ClearClock();
        textManager.m_SendEventOnFinishTextAsset.AddListener(m_Quests[m_ActiveQuestIndex].StartActiveGoalTimer);
        m_Quests[m_ActiveQuestIndex].FreezePlayer();

        // reset enabled
        foreach (GameObject gameObject in m_Quests[m_ActiveQuestIndex].m_Goals[m_Quests[m_ActiveQuestIndex].m_ActiveGoalIndex].m_ObjectsToEnableAfterReading)
        {
            gameObject.SetActive(false);
        }
        textManager.m_SendEventOnFinishTextAsset.AddListener(m_Quests[m_ActiveQuestIndex].ActiveGoalCallsAfterReading);
    }
}