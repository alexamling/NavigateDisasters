using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Car_AI : MonoBehaviour
{
  
    private NavMeshAgent carAgent;

    //for car navigation
    public GameObject[] wayPoints;
    private int currentWaypointIndex = 0;
    public float carSpeed = 5.0f;

    // Use this for initialization
    void Start ()
    {
        carAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Navigate();
	}

    void Navigate()
    {
        if (Vector3.Distance(this.transform.position, wayPoints[currentWaypointIndex].transform.position) >= 2)
        {
            carAgent.SetDestination(wayPoints[currentWaypointIndex].transform.position);
        }
    }
}
