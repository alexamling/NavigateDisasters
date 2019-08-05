using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractThis : MonoBehaviour
{
    public GameObject interactPrompt;
    public UnityEvent m_SendEventOnInteracted;
    public UnityEventIntArg m_SendEventIntArgOnInteracted;
    public UnityEventBoolArg m_SendEventBoolArgOnInteracted;

    private bool m_IsPlayerWithinRange;
    private bool m_HasBeenInteracted = false;

    ToggleView changeSwitch;

    void Start()
    {
        changeSwitch = Camera.main.GetComponent<ToggleView>();
    }

    public void Update()
    {
        if (m_IsPlayerWithinRange && Input.GetKeyDown(KeyCode.C))
        {
            /// NSF REU SUMMER 2018
            /// Elliot Privateer
            // Returns user to third person view when interacting in order
            // to properly display text.
            changeSwitch.switchView = 3;
            ///
            m_SendEventOnInteracted.Invoke();

            // placeholder parameters for values set in inspector
            m_SendEventIntArgOnInteracted.Invoke(0);
            m_SendEventBoolArgOnInteracted.Invoke(false);

            m_HasBeenInteracted = true;
            interactPrompt.SetActive(false);
        }
    }

    void OnEnable()
    {
        m_HasBeenInteracted = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player") || (other.gameObject.tag == "Boat"))
        {
            m_IsPlayerWithinRange = true;

            if (!m_HasBeenInteracted)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.tag == "Player") || (other.gameObject.tag == "Boat"))
        {
            m_IsPlayerWithinRange = false;

            if (!m_HasBeenInteracted)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}
