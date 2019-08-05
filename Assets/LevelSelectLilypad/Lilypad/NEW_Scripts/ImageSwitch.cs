using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// NSF REU
/// Elliot Privateer
/// Class used to host and change sprites for a single image object
public class ImageSwitch : MonoBehaviour
{
    // Base values for holding and switching images
    public List<Sprite> images;
    public GameObject mainImage;
    int switchNum;

    // Start is called before the first frame update
    void Start()
    {
        switchNum = 0;
        mainImage.GetComponent<Image>().sprite = images[switchNum];
    }

    /// FORWARD
    /// Description:
    /// Increments switchNum value which determines which index of the sprites
    /// is used to set the current sprite of the main image
    public void Forward()
    {
        switchNum++;
        if (switchNum > images.Count - 1)
            switchNum = 0;

        mainImage.GetComponent<Image>().sprite = images[switchNum];
    }

    /// BACK
    /// Description:
    /// Decrements switchNum value which determines which index of the sprites
    /// is used to set the current sprite of the main image
    public void Back()
    {
        switchNum--;
        if (switchNum < 0)
            switchNum = images.Count - 1;

        mainImage.GetComponent<Image>().sprite = images[switchNum];
    }
}
