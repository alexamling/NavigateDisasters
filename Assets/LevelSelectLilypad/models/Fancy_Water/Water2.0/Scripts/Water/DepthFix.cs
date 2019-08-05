using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DepthFix : MonoBehaviour {
	
	// Update is called once per frame
    void OnWillRenderObject()
	{
        Camera.current.depthTextureMode |= DepthTextureMode.Depth;		
	}
}