//
//  Original Creator: Max Kaiser
//  Contributors:
//  Date Created: 5/30/19
//  Last Modified: 6/12/19
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{

    #region Attributes

    //Canvas panels
    public GameObject levelPanel;
    public GameObject popUpPanel;
    public GameObject targetPanel;
    public GameObject mainPanel;

    //Large projectors on the walls of the level select scene
    public GameObject mainProjector;
    public GameObject leftProjector;
    public GameObject rightProjector;
    public GameObject howToPanel;

    //Images to be displayed on the projector
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    public Sprite sprite4;

    //warTable map spawning stuff
    public Transform mapPoint;
    public GameObject spawnedMap;
    public GameObject mapObject1;
    public GameObject mapObject2;
    public GameObject mapObject3;
    public GameObject mapObject4;
    public GameObject mapObject5;
    public GameObject mapObject6;

    //needed to update which scene will be loaded next
    public GameObject sceneManager;

    #endregion

    #region Initialization

    void Start()
    {
        mainPanel = GameObject.Find("Main");
        targetPanel = mainPanel;
        levelPanel = GameObject.Find("levelPanel");
        levelPanel.SetActive(false);
        popUpPanel = GameObject.Find("popUpPanel");
        popUpPanel.SetActive(false);

        mainProjector = GameObject.Find("mainProj");
        leftProjector = GameObject.Find("leftProj");
        rightProjector = GameObject.Find("rightProj");

        mapPoint = GameObject.Find("mapAnchor").transform;

        sceneManager = GameObject.Find("SceneManager");

        howToPanel = mainProjector.transform.GetChild(0).gameObject;
    }

    #endregion

    /// <summary>
    ///  Currently Unused
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// Exits the computer terminal by lerping away
    /// </summary>
    public void exitTerminal()
    {
        GameObject.Find("objCam").GetComponent<movCam>()._camState = movCam.CameraState.outlerping;
    }

    #region ButtonHighlighting

    /// <summary>
    /// Reveals the usually hidden icons bordering interactable buttons
    /// </summary>
    /// <param name="image"></param>
    public void reveal(GameObject image)
    {
        image.SetActive(true);
    }

    /// <summary>
    /// Hides the revealed icons bordering interactable buttons
    /// </summary>
    /// <param name="image"></param>
    public void unreveal(GameObject image)
    {
        image.SetActive(false);
    }

    #endregion

    #region PanelToggling

    /// <summary>
    /// Toggles the level select screen panel of the canavas
    /// </summary>
    public void toggleLevel()
    {
        if (!levelPanel.activeInHierarchy)
        {
            levelPanel.SetActive(true);
            targetPanel = levelPanel;
        }
        else
        {
            levelPanel.SetActive(false);
            targetPanel = mainPanel;
        }
    }

    /// <summary>
    /// toggles the level selected pop up window of the canavas
    /// </summary>
    public void togglepopUp()
    {
        if (!popUpPanel.activeInHierarchy)
        {
            popUpPanel.SetActive(true);
            targetPanel = popUpPanel;
        }
        else
        {
            popUpPanel.SetActive(false);
            targetPanel = mainPanel;
        }
    }

    #endregion

    #region GameSlection

    /// <summary>
    /// Update the projectors with new sprites and panels, spawn a map on the war table
    /// </summary>
    public void setGame1()
    {
        clearGame();
        leftProjector.GetComponent<Image>().overrideSprite = sprite1;
        rightProjector.GetComponent<Image>().overrideSprite = sprite1;
        howToPanel.SetActive(false);
        mainProjector.transform.GetChild(1).gameObject.SetActive(true);

        sceneManager.GetComponent<sceneManager>().nextSceneName = "Menu";

        spawnedMap = GameObject.Instantiate(mapObject1, mapPoint.position, Quaternion.identity);
        spawnedMap.transform.SetParent(GameObject.Find("warTable").transform);
    }

    /// <summary>
    /// Update the projectors with new sprites and panels, spawn a map on the war table
    /// </summary>
    public void setGame2()
    {
        clearGame();
        leftProjector.GetComponent<Image>().overrideSprite = sprite2;
        rightProjector.GetComponent<Image>().overrideSprite = sprite2;
        howToPanel.SetActive(false);
        mainProjector.transform.GetChild(2).gameObject.SetActive(true);

        sceneManager.GetComponent<sceneManager>().nextSceneName = "EOCGame";

        spawnedMap = GameObject.Instantiate(mapObject2, mapPoint.position, Quaternion.identity);
        spawnedMap.transform.Rotate(new Vector3(0, 180, 0));
        spawnedMap.transform.SetParent(GameObject.Find("warTable").transform);
    }

    /// <summary>
    /// Update the projectors with new sprites and panels, spawn a map on the war table
    /// </summary>
    public void setGame3()
    {
        clearGame();
        leftProjector.GetComponent<Image>().overrideSprite = sprite4;
        rightProjector.GetComponent<Image>().overrideSprite = sprite4;
        mainProjector.GetComponent<Image>().overrideSprite = sprite4;

        sceneManager.GetComponent<sceneManager>().nextSceneName = "nullScene";

        //spawnedMap = GameObject.Instantiate(mapObject3, mapPoint.position, Quaternion.identity);
        //spawnedMap.transform.SetParent(GameObject.Find("warTable").transform);
    }

    /// <summary>
    /// Update the projectors with new sprites and panels, spawn a map on the war table
    /// </summary>
    public void setGame4()
    {
        clearGame();
        leftProjector.GetComponent<Image>().overrideSprite = sprite4;
        rightProjector.GetComponent<Image>().overrideSprite = sprite4;
        mainProjector.GetComponent<Image>().overrideSprite = sprite4;

        sceneManager.GetComponent<sceneManager>().nextSceneName = "nullScene";

        //spawnedMap = GameObject.Instantiate(mapObject4, mapPoint.position, Quaternion.identity);
        //spawnedMap.transform.SetParent(GameObject.Find("warTable").transform);
    }

    /// <summary>
    /// Update the projectors with new sprites and panels, spawn a map on the war table
    /// </summary>
    public void setGame5()
    {
        clearGame();
        leftProjector.GetComponent<Image>().overrideSprite = sprite4;
        rightProjector.GetComponent<Image>().overrideSprite = sprite4;
        mainProjector.GetComponent<Image>().overrideSprite = sprite4;

        sceneManager.GetComponent<sceneManager>().nextSceneName = "nullScene";

        //spawnedMap = GameObject.Instantiate(mapObject5, mapPoint.position, Quaternion.identity);
        //spawnedMap.transform.SetParent(GameObject.Find("warTable").transform);
    }

    /// <summary>
    /// Update the projectors with new sprites and panels, spawn a map on the war table
    /// </summary>
    public void setGame6()
    {
        clearGame();
        leftProjector.GetComponent<Image>().overrideSprite = sprite4;
        rightProjector.GetComponent<Image>().overrideSprite = sprite4;
        mainProjector.GetComponent<Image>().overrideSprite = sprite4;

        sceneManager.GetComponent<sceneManager>().nextSceneName = "nullScene";

        //spawnedMap = GameObject.Instantiate(mapObject6, mapPoint.position, Quaternion.identity);
        //spawnedMap.transform.SetParent(GameObject.Find("warTable").transform);
    }

    /// <summary>
    /// Update the projectors with default sprites, clears any displayed panels, destroys any spawned maps
    /// </summary>
    public void clearGame()
    {
        //if (howToPanel.activeInHierarchy)
        //howToPanel.SetActive(false);
        leftProjector.GetComponent<Image>().overrideSprite = sprite3;
        rightProjector.GetComponent<Image>().overrideSprite = sprite3;
        //mainProjector.GetComponent<Image>().overrideSprite = sprite3;

        //turn off the panel displays of the projector
        for (int i = 0; i < mainProjector.transform.childCount; i++)
        {
            mainProjector.transform.GetChild(i).gameObject.SetActive(false);
        }

        howToPanel.SetActive(true);

        //destroy war table map if it exists
        if (spawnedMap != null)
        {
            GameObject.Destroy(spawnedMap);
        }
    }

    #endregion

}
