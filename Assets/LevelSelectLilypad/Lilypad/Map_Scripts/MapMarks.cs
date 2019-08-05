using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapMarks : MonoBehaviour
{
    ///Variables
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            RaycastHit location;
            Ray MouseLocation = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(MouseLocation, out location, maxDistance: 5, layerMask: 9))
            {
                if (Input.GetMouseButtonDown(0))
                { //Places marker on map at location with M1
                    GameObject marker = Instantiate(Resources.Load("Prefabs/Marker"), location.point, this.gameObject.transform.rotation, this.gameObject.transform.parent) as GameObject;
                    //Parenting doesn't project to map
                    marker.transform.localScale = Vector3.one;
                    Vector3 adjustPosition = marker.transform.localPosition;
                    adjustPosition.z = 0;
                    marker.transform.localPosition = adjustPosition;
                }
                /*if(Input.GetMouseButtonDown(1) && location.collider.gameObject is ) //Removes existing marker at location with M2
                {

                }*/
            }
        }
    }
}
