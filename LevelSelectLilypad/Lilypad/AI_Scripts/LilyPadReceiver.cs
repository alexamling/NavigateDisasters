using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyPadReceiver : MonoBehaviour
{
    public BoatPassengerCapacity boat;
    public List<Transform> m_SurvivorLandingTransforms;
    private List<GameObject> m_Survivors;

    private void Start()
    {
        m_Survivors = new List<GameObject>();
    }

    public void ReceiveSurvivors()
    {
        while (boat.m_Passengers.Count > 0)
        {
            GameObject passenger = boat.Disembark();
            m_Survivors.Add(passenger);

            //passenger.GetComponent<Survivor>().SetStanding(true);
            passenger.transform.SetParent(this.transform);
            passenger.transform.localPosition = m_SurvivorLandingTransforms[m_Survivors.Count - 1].localPosition;
            passenger.transform.localRotation = m_SurvivorLandingTransforms[m_Survivors.Count - 1].localRotation;
        }
        StartCoroutine("TransportSurvivors");
    }

    IEnumerator TransportSurvivors()
    {
        yield return new WaitForSeconds(20);
        for (int survivorIndex = m_Survivors.Count - 1; survivorIndex >= 0; survivorIndex--)
        {
            GameObject tempSurvivor = m_Survivors[survivorIndex];
            m_Survivors.RemoveAt(survivorIndex);
            Destroy(tempSurvivor);
        }
        yield return null;
    }
}
