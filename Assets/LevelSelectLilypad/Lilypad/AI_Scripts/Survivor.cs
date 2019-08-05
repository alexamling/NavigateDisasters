using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Survivor : MonoBehaviour
{
    public GameObject player;
    private Animator animator;
    private bool isWaving;
    private bool isStanding;
    private float distanceFromPlayer;
    
	void Start ()
    {
        animator = GetComponent<Animator>();
        SetStanding(true);
        StartCoroutine("UpdateDistanceFromPlayer");
    }

    IEnumerator UpdateDistanceFromPlayer()
    {
        while (true)
        {
            distanceFromPlayer = (this.transform.position - player.transform.position).magnitude;
            animator.SetFloat("distanceFromPlayer", distanceFromPlayer);

            // update less often when further away
            yield return new WaitForSeconds(distanceFromPlayer / 25.0f);
        }
    }

    public void SetStanding(bool newState)
    {
        isStanding = newState;
        animator.SetBool("isStanding", isStanding);
    }
}
