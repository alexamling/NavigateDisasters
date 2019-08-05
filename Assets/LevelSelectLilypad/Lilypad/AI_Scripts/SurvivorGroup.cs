using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorGroup : MonoBehaviour
{
    public BoatPassengerCapacity boat;
    public List<GameObject> m_Survivors;

    public void GetOnBoat()
    {
        foreach (GameObject survivor in m_Survivors)
        {
            boat.AttemptBoarding(survivor);
        }
    }
}