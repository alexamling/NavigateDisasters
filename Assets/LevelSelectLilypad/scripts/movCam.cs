//
//  Original Creator: Max Kaiser
//  Contributors:
//  Date Created: 5/30/19
//  Last Modified: 6/12/19
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// Script attached to the object camera (camera used for points of interest)
/// Defines what the camera does in various states
/// -Follows first person controller when in follow state
/// -Lerps to point of interest lock point when in the lerp state
/// -Lerps back to old position when outlerping
/// -Allows for camera movement in locked state (if movement is enabled)
/// </summary>
public class movCam : MonoBehaviour
{

    #region Attributes

    //for camera movement, this is the limit of movement from the cameras center position
    public float horizontalLimitFromOrigin;
    public float verticalLimitFromOrigin;

    //lock points for the camera to lerp to, overlooks points of interest
    private Transform warOriginPoint;
    private Transform monOriginPoint;

    //special object used to Control from the warTable camera point
    private GameObject backOut;
    private GameObject goTo;

    //bounds for object camera movement
    private float zUpBound;
    private float zLowBound;
    private float xUpBound;
    private float xLowBound;

    //enum for camera states
    public enum CameraState {follow, lerping, outlerping, locked};
    public CameraState _camState;

    //camera objects
    GameObject playerCam;

    //if the camera can move during lock state, set from rotCam.sc
    public bool moveEnabled;

    //lerping variables
    public float lerpSpeed = 4.0f;
    private float startTime;
    private float lerpLength;
    private bool isLerpStart = true;
    private Transform lerpStartPos;
    private Transform headPos;
    public GameObject targetToLerp;

    #endregion

    #region initialization

    void Awake()
    {
        //find the camera anchor points if they exist for the points of interest and reference them
        warOriginPoint = GameObject.Find("warCamPos").transform;
        monOriginPoint = GameObject.Find("monAnchor").transform;

        /*Set up bounds based on the chosen anchor, here we only use the warTable because it's currently
        he only one that allows movement. This will need to be changed to a function with a passed in transform if
        more points of interest with movement are added*/
            zUpBound = warOriginPoint.position.z + verticalLimitFromOrigin;
            zLowBound = warOriginPoint.position.z - verticalLimitFromOrigin;
            xUpBound = warOriginPoint.position.x + horizontalLimitFromOrigin;
            xLowBound = warOriginPoint.position.x - horizontalLimitFromOrigin;

        //default cam state and player camera
        _camState = CameraState.follow;
        playerCam = GameObject.FindGameObjectWithTag("playerCam").transform.GetChild(0).gameObject;
        

        //special object for controlling the warTable
            backOut = warOriginPoint.GetChild(0).gameObject;
            backOut.SetActive(false);

            goTo = warOriginPoint.GetChild(1).gameObject;
            goTo.SetActive(false);
    }

    #endregion


    void Update()
    {
        //The state in which the object camera is "locked" to a point of interest
        if (_camState == CameraState.locked) 
        {
            //this GameObject turns on when in lock state to be used as an exit 
            backOut.SetActive(true);
            goTo.SetActive(true);

            #region raycasting

            RaycastHit hit;
            Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                //If raycast hits the exit GameObject
                if (hit.transform == backOut.transform)
                {
                    backOut.GetComponent<Outline>().enabled = true;
                    //clicking the exit object will "exit" from the point of interest by lerping away, back to the player character
                    if (Input.GetMouseButtonDown(0))
                    {
                        _camState = CameraState.outlerping;
                        backOut.GetComponent<Outline>().enabled = false;
                    }
                }

                else if (hit.transform == goTo.transform)
                {
                    goTo.GetComponent<Outline>().enabled = true;
                    //clicking the go to object will load the next scene
                    if (Input.GetMouseButtonDown(0))
                    {
                        goTo.GetComponent<Outline>().enabled = false;
                        GameObject.Find("SceneManager").GetComponent<sceneManager>().LoadScene();
                    }
                }
                //if you look away from the exit object, stop highlighting it
                else
                {
                    backOut.GetComponent<Outline>().enabled = false;
                    goTo.GetComponent<Outline>().enabled = false;
                }
            }

            #endregion

            #region CameraMovement

            //if moving was allowed when the lockstate was called, alow the WASD keys to move the camera within set bounds
            if (moveEnabled)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    if (gameObject.transform.position.z <= zUpBound)
                    {
                        Vector3 tempPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 0.2f);
                        gameObject.transform.SetPositionAndRotation(tempPos, gameObject.transform.rotation);
                    }
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    if (gameObject.transform.position.x >= xLowBound)
                    {
                        Vector3 tempPos = new Vector3(gameObject.transform.position.x - 0.2f, gameObject.transform.position.y, gameObject.transform.position.z);
                        gameObject.transform.SetPositionAndRotation(tempPos, gameObject.transform.rotation);
                    }
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    if (gameObject.transform.position.z >= zLowBound)
                    {
                        Vector3 tempPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 0.2f);
                        gameObject.transform.SetPositionAndRotation(tempPos, gameObject.transform.rotation);
                    }
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    if (gameObject.transform.position.x <= xUpBound)
                    {
                        Vector3 tempPos = new Vector3(gameObject.transform.position.x + 0.2f, gameObject.transform.position.y, gameObject.transform.position.z);
                        gameObject.transform.SetPositionAndRotation(tempPos, gameObject.transform.rotation);
                    }
                }
            }

            #endregion
        }

        //State in which the object camera is disabled and follows the first person (player) camera
        else if (_camState == CameraState.follow)
        {
            //Debug.Log(playerCam.name);
            gameObject.transform.position = playerCam.transform.position;
            gameObject.transform.rotation = playerCam.transform.rotation;
        }

        //State in which the object camera switches on and lerps to the decided anchor point
        else if (_camState == CameraState.lerping)
        {
            if (targetToLerp.name == "warTable")
            {
                Lerping(warOriginPoint);
            }
            else if (targetToLerp.name == "MonMain")
            {
                Lerping(monOriginPoint);
            }
            else
            {
                Lerping(playerCam.transform);
            }
        }

        //State in which the object camera moves back to the first person controller and switches off
        else if (_camState == CameraState.outlerping)//go from lock point to first person head
        {
            outLerping();
        }
    }

    /// <summary>
    /// Custom lerp function that takes in an anchor transform. When called the first person camera is switched
    /// off and replaced with the object camera. The object camera will then lerp its position and rotation
    /// to the passed in anchor.
    /// </summary>
    /// <param name="lerpTarget"></param>
    void Lerping(Transform lerpTarget)
    {
        //if this is the first update frame since the function was called, initialize the lerp with the current state
        if (isLerpStart)
        {
            startTime = Time.time;

            //distance to lerp
            lerpLength = Vector3.Distance(gameObject.transform.position, lerpTarget.position);

            //point to begin lerping from, and a point to lerp back to later
            lerpStartPos = gameObject.transform;
            headPos = playerCam.transform;

            //don't initialize again
            isLerpStart = false;
        }

        //Standard lerping function here, update position along a continuum over time
        float lengthCovered = (Time.time - startTime) * lerpSpeed;
        float fracLength = lengthCovered / lerpLength;
        gameObject.transform.position = Vector3.Lerp(lerpStartPos.position, lerpTarget.position, fracLength);
        gameObject.transform.rotation = Quaternion.Lerp(lerpStartPos.rotation, lerpTarget.rotation, fracLength);

        //end case for the lerp
        if (Vector3.Distance(gameObject.transform.position, lerpTarget.position) < 0.05f)
        {
            /*if (lerpTarget.gameObject.name == "warCamPos")
            {
                //lockState controls cursor locking/invisibility
                playerCam.GetComponentInParent<FirstPersonController>().lockState = 1;
            }
            else if (lerpTarget.gameObject.name == "monAnchor")
            {
                //lockState controls cursor locking/invisibility
                playerCam.GetComponentInParent<FirstPersonController>().lockState = 1;
            }*/

            playerCam.GetComponentInParent<FirstPersonController>().lockState = 1;

            isLerpStart = true;
            _camState = CameraState.locked;
        }
    }

    /// <summary>
    /// Called when exiting the locked state, leads to the follow state. Is essentially the same as the lerp
    /// function, but in reverse. In this case no parameter is needed as the first person controller camera/head
    /// will always serve as the anchor point to lerp to.
    /// </summary>
    public void outLerping()
    {
        //if this is the first update frame since the function was called, initialize the lerp with the current state
        if (isLerpStart)
        {
            //relock the camera
            playerCam.GetComponentInParent<FirstPersonController>().lockState = 0;

            //initialize the de-lerp
            startTime = Time.time;
            lerpLength = Vector3.Distance(gameObject.transform.position, headPos.position);
            lerpStartPos = gameObject.transform;

            //skip this on the next frame
            isLerpStart = false;

            //turn off the exit point
            backOut.SetActive(false);
            goTo.SetActive(false);
        }

        //standard lerping, but reversed in direction
        float lengthCovered = (Time.time - startTime) * lerpSpeed;
        float fracLength = lengthCovered / lerpLength;
        gameObject.transform.position = Vector3.Lerp(lerpStartPos.position, headPos.position, fracLength);
        gameObject.transform.rotation = Quaternion.Lerp(lerpStartPos.rotation, headPos.rotation, fracLength);

        //exit case for the lerp
        if (Vector3.Distance(gameObject.transform.position, headPos.position) < 0.05f)
        {
            isLerpStart = true;

            playerCam.GetComponent<Camera>().enabled = true;
            gameObject.GetComponent<Camera>().enabled = false;

            _camState = CameraState.follow;
        }
    }
}
