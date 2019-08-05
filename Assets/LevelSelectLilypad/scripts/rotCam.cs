//
//  Original Creator: Max Kaiser
//  Contributors:
//  Date Created: 6/03/19
//  Last Modified: 6/12/19
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script attached to the firstperson camera
/// -Raycasting to points of interest and highlighting them
/// -Switching Cameras
/// Triggering the camera lerp state (see movCam.cs for details)
/// </summary>
public class rotCam : MonoBehaviour
{
    #region Attributes
    //this camera component
    Camera cam;

    //Gameobjects for areas of interest
    GameObject mainScreen;
    GameObject warTable;
    GameObject warPos;
    GameObject door;
    GameObject EOCTable;

    GameObject highlightedObject;

    //other camera that follows the first person, we'll switch to this one when interacting with areas of interest
    GameObject otherCamera;

    #endregion

    #region intialization

    void Start()
    {
        cam = GetComponent<Camera>();
        mainScreen = GameObject.Find("MonMain");
        warTable = GameObject.Find("warTable");
        warPos = GameObject.Find("warCamPos");
        otherCamera = GameObject.Find("objCam");

        door = GameObject.Find("exitDoor");
        EOCTable = GameObject.Find("EOCTable");
    }

    #endregion

    void Update()
    {
        ///Code below executes when the object camera (the other camera that isn't this one) is in a follow state
        if (otherCamera.GetComponent<movCam>()._camState == movCam.CameraState.follow)
        {
            //Set up the raycast, and ignore casting to objects on the 9th layer (like the FPSController)
            int layerMask = 1 << 9;
            layerMask = ~layerMask;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;

            //if an object is successfully hit
            if (Physics.Raycast(ray, out hit, 20.0f, layerMask))
            {
                Debug.Log(hit.transform.gameObject);
                //if (highlightedObject != null)
                    //highlightedObject.GetComponent<Outline>().enabled = true;

                #region computerScreen

                if (hit.transform.gameObject == mainScreen) 
                {
                    //turn on outline
                    //mainScreen.transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = true;
                    highlightedObject = mainScreen.transform.GetChild(0).gameObject;
                    highlightedObject.GetComponent<Outline>().enabled = true;

                    //on mouse click while outlined, switch cameras and change to a lerping camera state
                    if (Input.GetMouseButtonDown(0))
                    {
                        otherCamera.GetComponent<Camera>().enabled = true;
                        gameObject.GetComponent<Camera>().enabled = false;
                        //mainScreen.transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = false;
                        highlightedObject.GetComponent<Outline>().enabled = false;
                        //highlightedObject = null;
                        otherCamera.GetComponent<movCam>().targetToLerp = mainScreen;
                        otherCamera.GetComponent<movCam>().moveEnabled = false;
                        otherCamera.GetComponent<movCam>()._camState = movCam.CameraState.lerping;
                    }
                }

                #endregion

                #region warTable

                else if (hit.transform.gameObject == warTable) 
                {
                    //turn on outline
                    //warTable.GetComponent<Outline>().enabled = true;
                    highlightedObject = warTable;
                    highlightedObject.GetComponent<Outline>().enabled = true;

                    //on mouse click while outlined, switch cameras and change to a lerping camera state
                    if (Input.GetMouseButtonDown(0))
                    {
                        otherCamera.GetComponent<Camera>().enabled = true;
                        gameObject.GetComponent<Camera>().enabled = false;
                        //warTable.GetComponent<Outline>().enabled = false;
                        highlightedObject.GetComponent<Outline>().enabled = false;
                        //highlightedObject = null;
                        otherCamera.GetComponent<movCam>().targetToLerp = warTable;
                        otherCamera.GetComponent<movCam>().moveEnabled = true;
                        otherCamera.GetComponent<movCam>()._camState = movCam.CameraState.lerping;
                    }
                }

                #endregion

                #region exitDoor

                else if (hit.transform.gameObject == door)
                {
                    //turn on outline
                    //warTable.GetComponent<Outline>().enabled = true;
                    highlightedObject = door.transform.GetChild(0).gameObject;
                    highlightedObject.GetComponent<Outline>().enabled = true;

                    //on mouse click while outlined, switch cameras and change to a lerping camera state
                    if (Input.GetMouseButtonDown(0))
                    {
                        //warTable.GetComponent<Outline>().enabled = false;
                        highlightedObject.GetComponent<Outline>().enabled = false;
                        //highlightedObject = null;
                        sceneManager manager = GameObject.Find("SceneManager").GetComponent<sceneManager>();
                        manager.nextSceneName = "levelSelect";
                        manager.LoadScene();
                    }
                }

                #endregion

                #region EOCTable

                else if (hit.transform.gameObject == EOCTable)
                {
                    //turn on outline
                    //warTable.GetComponent<Outline>().enabled = true;
                    //highlightedObject = EOCTable.transform.GetChild(0).gameObject;
                    highlightedObject = EOCTable;
                    highlightedObject.GetComponent<Outline>().enabled = true;

                    //on mouse click while outlined, switch cameras and change to a lerping camera state
                    if (Input.GetMouseButtonDown(0))
                    {
                        //warTable.GetComponent<Outline>().enabled = false;
                        highlightedObject.GetComponent<Outline>().enabled = false;
                        //highlightedObject = null;
                        sceneManager manager = GameObject.Find("SceneManager").GetComponent<sceneManager>();
                        manager.nextSceneName = "MapTesting";
                        manager.LoadScene();
                    }
                }

                #endregion

            }

            //if you don't see anything, stop highlighting anything
            else
            {
                //mainScreen.transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = false;
                //warTable.GetComponent<Outline>().enabled = false;
                if (highlightedObject != null)
                    highlightedObject.GetComponent<Outline>().enabled = false;
            }
        }
    }
}
