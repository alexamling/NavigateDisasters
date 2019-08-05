using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Authors: Jordan Machalek & Nicolas San Jose
 */
public class MapZoom : MonoBehaviour {

    // Variables
    public GameObject[] mapLayers;
    public float zoomRate = 1;
    private Vector3 lastMousePosition;
    private List<Vector3> defaultScales = new List<Vector3>();
    private List<Vector3> defaultPositions = new List<Vector3>();
    public bool paused = false;
    private Camera cam;

	// Use this for initialization
	void Start ()
    {
        lastMousePosition = Input.mousePosition;
        cam = Camera.main;

        for (int i = 0; i < mapLayers.Length; i++)
        {
            defaultScales.Add(mapLayers[i].GetComponent<RectTransform>().localScale);
            defaultPositions.Add(mapLayers[i].GetComponent<RectTransform>().localPosition);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!paused)
        {
            // Ignore scrolling on paper when zooming map
            int fpOverlayLayerMask = 1 << 12;
            int mapLayerMask = 1 << 13;
            int mapBlockerLayerMask = 1 << 14;

            int layerMask = mapLayerMask | mapBlockerLayerMask | fpOverlayLayerMask;
            RaycastHit hitInfo;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
            {
                if (hitInfo.collider.gameObject.layer != 13)
                {
                    return;
                }
            }

            if (Input.GetMouseButton(2))
            {
                for (int j = 0; j < defaultScales.Count; j++)
                {
                    mapLayers[j].GetComponent<RectTransform>().localPosition = new Vector3(
                        mapLayers[j].GetComponent<RectTransform>().localPosition.x + Input.GetAxis("Mouse X"),
                        mapLayers[j].GetComponent<RectTransform>().localPosition.y + Input.GetAxis("Mouse Y"),
                        0);

                    ClampPositionOfMapLayer(j);
                }
            }

            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                for (int j = 0; j < defaultScales.Count; j++)
                {

                    mapLayers[j].GetComponent<RectTransform>().localScale = new Vector3(
                        Mathf.Clamp(mapLayers[j].GetComponent<RectTransform>().localScale.x + Input.GetAxis("Mouse ScrollWheel") * zoomRate * defaultScales[j].x, defaultScales[j].x, defaultScales[j].x * 4),
                        Mathf.Clamp(mapLayers[j].GetComponent<RectTransform>().localScale.y + Input.GetAxis("Mouse ScrollWheel") * zoomRate * defaultScales[j].y, defaultScales[j].y, defaultScales[j].y * 4),
                        0);

                    ClampPositionOfMapLayer(j);
                }
            }
        }
	}

    private void ClampPositionOfMapLayer(int j)
    {
        Vector3 scaleDifference = mapLayers[0].GetComponent<RectTransform>().localScale - defaultScales[0];
        Vector2 offsetClampCurrentScale = scaleDifference * 25;

        mapLayers[j].GetComponent<RectTransform>().localPosition = new Vector3(
            Mathf.Clamp(
                mapLayers[j].GetComponent<RectTransform>().localPosition.x,
                defaultPositions[j].x - offsetClampCurrentScale.x, defaultPositions[j].x + offsetClampCurrentScale.x),
            Mathf.Clamp(
                mapLayers[j].GetComponent<RectTransform>().localPosition.y,
                defaultPositions[j].y - offsetClampCurrentScale.y, defaultPositions[j].y + offsetClampCurrentScale.y),
            0);
    }
}
