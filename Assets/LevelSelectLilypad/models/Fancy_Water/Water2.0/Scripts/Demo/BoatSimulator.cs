using UnityEngine;
using System.Collections;

public class BoatSimulator : MonoBehaviour
{

    private Rigidbody rigid;
    private bool keyPressedW, keyPressedA, keyPressedS, keyPressedD;
	// Use this for initialization
	void Start ()
	{
	    rigid = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.W))
	        keyPressedW = true;
        if (Input.GetKeyDown(KeyCode.A))
            keyPressedA = true;
        if (Input.GetKeyDown(KeyCode.S))
            keyPressedS = true;
        if (Input.GetKeyDown(KeyCode.D))
            keyPressedD = true;

        if (Input.GetKeyUp(KeyCode.W))
            keyPressedW = false;
        if (Input.GetKeyUp(KeyCode.A))
            keyPressedA = false;
        if (Input.GetKeyUp(KeyCode.S))
            keyPressedS = false;
        if (Input.GetKeyUp(KeyCode.D))
            keyPressedD = false;

        if (keyPressedW)
        {
            rigid.AddForce(transform.right * 1000 * Time.deltaTime);
	    }
        if (keyPressedS)
        {
            rigid.AddForce(-transform.right * 1000 * Time.deltaTime);
	    }
	    if (keyPressedD) {
            rigid.AddTorque(transform.up * 200 * Time.deltaTime);
	    }
        if (keyPressedA)
        {
            rigid.AddTorque(-transform.up * 200 * Time.deltaTime);
        }

       }

}
