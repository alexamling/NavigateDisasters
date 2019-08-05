using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePerspective : MonoBehaviour
{
 //   // UI
 //   public GameObject thirdPersonCanvas;
 //   public GameObject firstPersonCanvas;

 //   // Cinemachine
 //   public Cinemachine.CinemachineVirtualCamera m_FollowVirtualCamera;
 //   public Cinemachine.CinemachineVirtualCamera m_DollyVirtualCamera;
 //   private Cinemachine.CinemachineTrackedDolly m_CameraDolly;

 //   // Player
 //   public GameObject playerObject;
    
 //   private bool m_IsCameraFocusMap = false;
 //   private bool m_IsCameraInTransitionBetweenFollowAndMap = false;
 //   private float m_CameraTransitionSpeed = 1.0f;

 //   void Start ()
 //   {
 //       m_CameraDolly = m_DollyVirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();

 //       // Start with third person perspective
 //       m_FollowVirtualCamera.m_Priority = 1;
 //       m_DollyVirtualCamera.m_Priority = 0;
 //       m_CameraDolly.m_PathPosition = 0;
 //       thirdPersonCanvas.SetActive(true);
 //       firstPersonCanvas.SetActive(false);
 //   }
	
	//void Update ()
 //   {
 //       // Switch Perspective and Canvas
 //       if (Input.GetKeyDown(KeyCode.M) && !m_IsCameraInTransitionBetweenFollowAndMap)
 //       {
 //           m_IsCameraInTransitionBetweenFollowAndMap = true;
            
 //           // Swap priority to start transition between virtual cameras
 //           m_FollowVirtualCamera.m_Priority = System.Convert.ToInt32(m_IsCameraFocusMap);
 //           m_DollyVirtualCamera.m_Priority = System.Convert.ToInt32(!m_IsCameraFocusMap);

 //           // todo: pause subtitles
 //       }
 //       if (m_IsCameraInTransitionBetweenFollowAndMap)
 //       {
 //           float step;
 //           float limit;
 //           float thresholdToToggleUI;
 //           bool isCameraAtEndOfDolly;

 //           // Prepare direction of transition
 //           if (m_IsCameraFocusMap)
 //           {
 //               // From map focus to follow camera
 //               limit = 0.0f;
 //               isCameraAtEndOfDolly = m_CameraDolly.m_PathPosition <= limit;
 //               step = -m_CameraTransitionSpeed;
 //               thresholdToToggleUI = 0.5f;
 //           }
 //           else
 //           {
 //               // From follow camera to map focus
 //               limit = 1.0f;
 //               isCameraAtEndOfDolly = m_CameraDolly.m_PathPosition >= limit;
 //               step = m_CameraTransitionSpeed;
 //               thresholdToToggleUI = 0.9f;
 //           }

 //           if (!isCameraAtEndOfDolly)
 //           {
 //               // Step through dolly
 //               m_CameraDolly.m_PathPosition += step * Time.deltaTime;

 //               // Switch UI
 //               if (Mathf.Abs(m_CameraDolly.m_PathPosition - thresholdToToggleUI) < .1)
 //               {
 //                   firstPersonCanvas.SetActive(!m_IsCameraFocusMap);
 //                   thirdPersonCanvas.SetActive(m_IsCameraFocusMap);
 //               }
 //           }
 //           else
 //           {
 //               // End transition
 //               m_CameraDolly.m_PathPosition = limit;
 //               m_IsCameraFocusMap = !m_IsCameraFocusMap;
 //               m_IsCameraInTransitionBetweenFollowAndMap = false;

 //               // Disable movement in first person perspective
 //               if (playerObject.tag == "Player")
 //               {
 //                   if (m_IsCameraFocusMap)
 //                   {
 //                       playerObject.GetComponent<ThirdPersonUserControl>().m_IsMovementDisabled = true;
 //                       playerObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
 //                       playerObject.GetComponent<Animator>().SetFloat("Forward", 0, 0.0f, Time.deltaTime);
 //                       playerObject.GetComponent<Animator>().SetFloat("Turn", 0, 0.0f, Time.deltaTime);
 //                   }
 //                   else
 //                   {
 //                       playerObject.GetComponent<ThirdPersonUserControl>().m_IsMovementDisabled = false;
 //                       playerObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
 //                   }
 //               }
 //               else if (playerObject.tag == "Boat")
 //               {
 //                   if (m_IsCameraFocusMap)
 //                   {
 //                       playerObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
 //                   }
 //                   else
 //                   {
 //                       playerObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
 //                   }
 //               }
 //           }
 //       }
 //   }
}
