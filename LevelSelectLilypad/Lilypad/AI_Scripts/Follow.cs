using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Follow : MonoBehaviour
{
    public bool FollowVolunteer = false;
    NavMeshAgent agent;
    public GameObject Player;
    public float offset = 5;
    public float followDist = 10;
    private float dist;
    

	// Use this for initialization
	void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
       
	}
	
	// Update is called once per frame
	void Update ()
    {
        dist = Vector3.Distance(transform.position, Player.transform.position);
       
        if (FollowVolunteer == true)
        {
            if(followDist < dist)
                agent.SetDestination(Player.transform.position);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                FollowVolunteer = true;
            }
        }

    }
}
