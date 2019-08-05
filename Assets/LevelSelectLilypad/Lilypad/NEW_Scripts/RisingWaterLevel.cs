using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingWaterLevel : MonoBehaviour
{
    // Vector that determines rate of rising
    Vector3 risingRate;

    // Start is called before the first frame update
    void Start()
    {
        risingRate = new Vector3(0f, .002f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Moves attached object up at a defined rate based in time
        transform.position += risingRate * Time.deltaTime;
    }
}
