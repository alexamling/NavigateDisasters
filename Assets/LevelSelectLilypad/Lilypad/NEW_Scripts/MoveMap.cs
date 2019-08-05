using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMap : MonoBehaviour
{
    // Gameobject anchors used to lerp map to
    public GameObject upAnchor;
    public GameObject downAnchor;

    // Map object
    public GameObject moveable;

    // Amount per update that the map moves when active
    Vector3 increment;

    // Booleans that determine which direction the map
    // goes and whether or not the map stays up
    bool moveUp;
    bool keepUp;

    // Start is called before the first frame update
    void Start()
    {
        // Sets values for necessary variables
        increment = new Vector3(0f, .075f, 0f);
        moveUp = false;
        keepUp = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Determines whether to move the map up or down
        if(moveUp || keepUp)
        {
            if (moveable.transform.position.y < upAnchor.transform.position.y)
                moveable.transform.position += increment;
        }
        else
        {
            if (moveable.transform.position.y > downAnchor.transform.position.y)
                moveable.transform.position -= increment;
        }
    }

    /// MOVEDIRECTION
    /// Description:
    /// Changes boolean to determine direction that map must move
    public void MoveDirection()
    {
        moveUp = !moveUp;
    }

    /// STAYUP
    /// Description:
    /// Changes boolean that keeps map up
    public void StayUp()
    {
        keepUp = !keepUp;
    }
}
