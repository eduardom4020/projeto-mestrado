using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public abstract class ChartDrawer
{
    protected ChartProperties CurrentState;
    protected ChartProperties PreviousState;

    private RenderTexture RenderTexture;
    private Camera RenderCamera;
    private Material RenderMaterial;

    private bool Mounted = false;

    protected ChartDrawer(ChartProperties CurrentState, ChartProperties PreviousState) => SetStates(CurrentState, PreviousState);

    public void SetStates(ChartProperties CurrentState, ChartProperties PreviousState) 
    {
        this.CurrentState = CurrentState;
        this.PreviousState = PreviousState;
    }

    private GameObject GetChildByNameAndLayer(string name, int layerNum, Transform currTransform)
    {
        GameObject iterationResult = null;

        foreach (Transform child in currTransform)
        {
            if (child.gameObject.name == name && child.gameObject.layer == layerNum)
            {
                iterationResult = child.gameObject;
            }
            else
            {
                var childIterationResult = GetChildByNameAndLayer(name, layerNum, child);

                if (childIterationResult != null)
                    iterationResult = childIterationResult;
            }
        }

        return iterationResult;
    }

    public void ListenToPropsChange(Transform currTransform)
    {
        if (!Mounted || RenderTexture == null)
        {
            RenderTexture = new RenderTexture(400, 455, 16, RenderTextureFormat.ARGB32);
            var createdWithSuccess = RenderTexture.Create();

            if (!createdWithSuccess)
                RenderTexture = null;            
        }

        if(!Mounted || RenderCamera == null)
            RenderCamera = GetChildByNameAndLayer("RenderCamera", 5, currTransform)?.GetComponent<Camera>();
        
        if(!Mounted || RenderCamera != null && RenderCamera.targetTexture == null)
            RenderCamera.targetTexture = RenderTexture;

        if(!Mounted || RenderMaterial == null && RenderCamera?.targetTexture != null)
        {
            RenderMaterial = new Material(Shader.Find("Unlit/Transparent"));
            RenderMaterial.SetTexture("_MainTex", RenderCamera.targetTexture);
        }

        var mesh = GetChildByNameAndLayer("WorldSpaceProjection", 0, currTransform)?.GetComponent<MeshRenderer>();

        if (RenderMaterial != null)
            mesh.sharedMaterial = RenderMaterial;


        if(mesh?.sharedMaterials != null && mesh?.sharedMaterials?.Length > 1)
        {
            mesh.sharedMaterials = new Material[] { RenderMaterial };
        }


        UpdateTitleChangeIfChanged(currTransform);

        Mounted = true;
    }

    private void UpdateTitleChangeIfChanged(Transform currTransform)
    {
        if (CurrentState != null && CurrentState?.Title != PreviousState?.Title)
        {
            var titleComponent = GetChildByNameAndLayer("Title", 5, currTransform);

            if(titleComponent != null)
                titleComponent.gameObject.GetComponentInChildren<TMP_Text>().SetText(CurrentState.Title);
        }
    }
}
