using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectThis : MonoBehaviour
{
    public UnityEvent m_SendEventOnCollected;
    
    private bool m_IsPlayerWithinRange;

    private void ToggleCollectPromptUI(bool displayState)
    {

    }

    public void Update()
    {
        if (m_IsPlayerWithinRange && Input.GetKeyDown(KeyCode.F))
        {
            m_SendEventOnCollected.Invoke();
            this.gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            ToggleCollectPromptUI(true);
            m_IsPlayerWithinRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            ToggleCollectPromptUI(false);
            m_IsPlayerWithinRange = false;
        }
    }
}
