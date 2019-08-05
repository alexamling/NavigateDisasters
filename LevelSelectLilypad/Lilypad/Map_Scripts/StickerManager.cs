using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerManager : MonoBehaviour
{
    public Camera cam;
    public GameObject m_stickerObject;
    //public List<Sprite> m_stickerSprites;
    public bool m_IsInteractionDisabled;
    [SerializeField]
    private Text uiStickerCurrentCount;
    [SerializeField]
    private Text uiStickerMaxCount;
    
    [SerializeField]
    private int m_maxSpriteCount = 10;
    private List<GameObject> m_newStickerObjects;

    public void Start()
    {
        m_newStickerObjects = new List<GameObject>();
        uiStickerMaxCount.text = m_maxSpriteCount.ToString();
    }

    public void Update()
    {
        if (!m_IsInteractionDisabled)
        {
            // input add sticker (exclusive if statements prevent simultaneous sticker addition/removal)
            if (Input.GetMouseButtonDown(0))
            {
                // limit amount of stickers
                if (m_newStickerObjects.Count < m_maxSpriteCount)
                {
                    // layer of map plane to place stickers on
                    int addStickerLayerMask = 1 << 9;
                    int removeStickerLayerMask = 1 << 10;
                    int blockStickerLayerMask = 1 << 11;
                    int fpOverlayLayerMask = 1 << 12;
                    int layerMask = addStickerLayerMask | removeStickerLayerMask | blockStickerLayerMask | fpOverlayLayerMask;
                    RaycastHit hitInfo;
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                    // test clicking on map
                    if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
                    {
                        // ignore clicks on other stickers and ui
                        if (hitInfo.collider.gameObject.layer == 9)
                        {
                            // create new sticker
                            GameObject newSticker = Instantiate(m_stickerObject);
                            newSticker.transform.position = hitInfo.point;
                            newSticker.transform.SetParent(this.transform);
                            newSticker.transform.localRotation = Quaternion.identity;
                            newSticker.transform.localScale = Vector3.one * 4;

                            // keep a reference
                            m_newStickerObjects.Add(newSticker);

                            // update ui counter
                            uiStickerCurrentCount.text = m_newStickerObjects.Count.ToString();
                        }
                    }
                }
            }
            // input remove sticker
            else if (Input.GetMouseButtonDown(1))
            {
                // layer of existing stickers
                int layerMask = 1 << 10;
                RaycastHit hitInfo;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                // test clicking on sticker
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
                {
                    // remove sticker
                    m_newStickerObjects.Remove(hitInfo.collider.gameObject);
                    Destroy(hitInfo.collider.gameObject);

                    // update ui counter
                    uiStickerCurrentCount.text = m_newStickerObjects.Count.ToString();
                }
            }
        }
    }

    //Sets color of map marker
    public void SetSprite(Sprite sprite)
    {
        m_stickerObject.GetComponent<Image>().sprite = sprite;
    }
}
