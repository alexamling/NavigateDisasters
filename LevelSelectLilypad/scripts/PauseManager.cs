using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class PauseManager : MonoBehaviour
{

    public GameObject pauseCanvas;
    public GameObject playerController;
    public GameObject otherCamera;
    private bool curState;

    // Start is called before the first frame update
    void Start()
    {
        pauseCanvas = GameObject.Find("PauseCanvas");
        curState = false;
        pauseCanvas.SetActive(curState);

        playerController = GameObject.Find("FPSController");
        otherCamera = GameObject.Find("objCam");
    }

    // Update is called once per frame
    void Update()
    {
        if (!curState)
        {
            //FirstPersonController playerScript = playerController.GetComponent<FirstPersonController>();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                curState = !curState;
                pauseCanvas.SetActive(curState);

                otherCamera.GetComponent<Camera>().enabled = true;

                playerController.GetComponent<FirstPersonController>().lockState = 1;

                playerController.transform.GetChild(0).gameObject.GetComponent<Camera>().enabled = false;

                //otherCamera.GetComponent<movCam>().targetToLerp = otherCamera;
                otherCamera.GetComponent<movCam>().moveEnabled = false;
                //otherCamera.GetComponent<movCam>()._camState = movCam.CameraState.lerping;
                otherCamera.GetComponent<movCam>()._camState = movCam.CameraState.locked;
            }
            /*else if (!curState)
            {
                //otherCamera.GetComponent<movCam>().targetToLerp = otherCamera;
                //otherCamera.GetComponent<movCam>()._camState = movCam.CameraState.outlerping;
                otherCamera.GetComponent<movCam>()._camState = movCam.CameraState.follow;

                playerController.GetComponent<FirstPersonController>().lockState = 0;

                otherCamera.GetComponent<Camera>().enabled = false;
                playerController.transform.GetChild(0).gameObject.GetComponent<Camera>().enabled = true;
            }*/
        }
    }

    public void ExitGame()
    {

    }

    public void ReturnToGame()
    {
        otherCamera.GetComponent<movCam>()._camState = movCam.CameraState.follow;

        playerController.GetComponent<FirstPersonController>().lockState = 0;

        otherCamera.GetComponent<Camera>().enabled = false;
        playerController.transform.GetChild(0).gameObject.GetComponent<Camera>().enabled = true;

        curState = !curState;
        pauseCanvas.SetActive(curState);
    }
}
