using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Jordan Machalek
 * Attached to MAIN_GUI_Canvas > Phone
 */
public class NotificationAnimation : MonoBehaviour {

    // Variables
    public bool notification;
    private RectTransform phoneTransform;
    private Vector3 phonePosition;
    private int notificationTime = 0;

    // Use this for initialization
    void Start()
    {
        phoneTransform = GetComponent<RectTransform>();
        phonePosition = phoneTransform.anchoredPosition3D;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            notification = true;
        }

        // Move up
        if (notification && phonePosition.y == -460)
        {
            Move(1);

            notification = false;
        }
        else if(notification && phonePosition.y == -250) // Move down
        {
            Move(-1);

            notification = false;
        }
	}
    
    // Moves the phone up or down depending on current position
    void Move(int direction)
    {
        phonePosition.y += direction * 210;

        phoneTransform.anchoredPosition3D = phonePosition;
    }

}
