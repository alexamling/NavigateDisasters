using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    public GameObject[] debrisObjects;
    public Transform[] spawnPoints;
    private int objectIndex;


    private void Start()
    {
        

        foreach (Transform i in spawnPoints)
        {
            objectIndex = Random.Range(0, debrisObjects.Length);
            Instantiate(debrisObjects[objectIndex], i.position, i.rotation);
        }
        

    }


}
