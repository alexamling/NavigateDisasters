using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatPassengerCapacity : MonoBehaviour
{
    public List<Transform> m_PassengerSeatedTransforms;
    public List<GameObject> m_Passengers;

    public void AttemptBoarding(GameObject passenger)
    {
        if (m_Passengers.Count < m_PassengerSeatedTransforms.Count)
        {
            m_Passengers.Add(passenger);
            passenger.GetComponent<Survivor>().SetStanding(false);
            passenger.transform.SetParent(this.transform);
            passenger.transform.localPosition = m_PassengerSeatedTransforms[m_Passengers.Count - 1].localPosition;
            passenger.transform.localRotation = m_PassengerSeatedTransforms[m_Passengers.Count - 1].localRotation;
        }
        else
        {
            passenger.SetActive(false);
        }
    }

    public GameObject Disembark()
    {
        if (m_Passengers.Count > 0)
        {
            GameObject lastPassenger = m_Passengers[m_Passengers.Count - 1];
            m_Passengers.RemoveAt(m_Passengers.Count - 1);
            return lastPassenger;
        }
        return null;
    }
}
