using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ProjectorMatrix : MonoBehaviour {

    public matrixName GlobalMatrixName;
    public Transform ProjectiveTranform;
    public bool UpdateOnStart;
    public bool CanUpdate = true;

    private Transform t;

    void Start()
    {
        t = transform;
        if (UpdateOnStart) UpdateMatrix();
    }

    void Update()
    {
        if (!UpdateOnStart) UpdateMatrix();
#if UNITY_EDITOR
        if(!Application.isPlaying) UpdateMatrix();
#endif
    }

    public void UpdateMatrix()
    {
        if (!CanUpdate)
            return;
       
        Shader.SetGlobalMatrix(GlobalMatrixName.ToString(), ProjectiveTranform.worldToLocalMatrix * t.localToWorldMatrix);
    }

    public enum matrixName
    {
        _projectiveMatrWaves,
        _projectiveMatrCausticScale
    }
}

