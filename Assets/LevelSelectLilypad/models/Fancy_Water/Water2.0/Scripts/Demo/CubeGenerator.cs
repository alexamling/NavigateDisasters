using UnityEngine;
using System.Collections;

public class CubeGenerator : MonoBehaviour
{

    public GameObject cubes;
	// Use this for initialization
	void Start () {
        InvokeRepeating("UpdateCube", 1, 2);
	}
	
	// Update is called once per frame
	void UpdateCube ()
	{
	    var pos = transform.position;
	    pos.y += 10;
	    pos.z -= 4;
	    pos += Random.insideUnitSphere * 7;
        var go = Instantiate(cubes, pos, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))) as GameObject;
	    go.AddComponent<Buoyancy>().Density = Random.Range(700, 850);
	    go.AddComponent<Rigidbody>().mass = Random.Range(100, 150);
        Destroy(go, 30);
	}
}
