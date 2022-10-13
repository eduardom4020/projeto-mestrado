//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TMPro;
//using UnityEngine;

//public struct SeriesDrawer
//{
//    public string Name;
//    public GameObject Root;
//    public List<GameObject> Entries;
//}

//[ExecuteInEditMode]
//public abstract class ChartDrawer
//{
//    //protected ChartProperties CurrentState;
//    //protected ChartProperties PreviousState;

//    protected List<Series> CurrentSeries;
//    protected List<Series> PreviousSeries;

//    protected List<SeriesDrawer> SeriesDrawer;

//    private RenderTexture RenderTexture;
//    private Camera RenderCamera;
//    private Material RenderMaterial;

//    private bool Mounted = false;

//    //protected ChartDrawer(ChartProperties CurrentState, ChartProperties PreviousState)
//    //{
//    //    SeriesDrawer = new List<SeriesDrawer>();
//    //    SetStates(CurrentState, PreviousState);
//    //}

//    //public void SetStates(ChartProperties CurrentState, ChartProperties PreviousState) 
//    //{
//    //    this.CurrentState = CurrentState;
//    //    this.PreviousState = PreviousState;
//    //}

//    public void ListenToPropsChange(Transform currTransform)
//    {
//        if (!Mounted || RenderTexture == null)
//        {
//            RenderTexture = new RenderTexture(400, 455, 16, RenderTextureFormat.ARGB32);
//            var createdWithSuccess = RenderTexture.Create();

//            if (!createdWithSuccess)
//                RenderTexture = null;            
//        }

//        if(!Mounted || RenderCamera == null)
//            RenderCamera = ObjectIterator.GetChildByNameAndLayer("Camera", 5, currTransform)?.GetComponent<Camera>();
        
//        if(!Mounted || RenderCamera != null && RenderCamera.targetTexture == null)
//            RenderCamera.targetTexture = RenderTexture;

//        if(!Mounted || RenderMaterial == null && RenderCamera?.targetTexture != null)
//        {
//            RenderMaterial = new Material(Shader.Find("Unlit/Transparent"));
//            RenderMaterial.SetTexture("_MainTex", RenderCamera.targetTexture);
//        }

//        var mesh = ObjectIterator.GetChildByNameAndLayer("RenderedSurface", 0, currTransform)?.GetComponent<MeshRenderer>();

//        if (RenderMaterial != null)
//            mesh.sharedMaterial = RenderMaterial;


//        if(mesh?.sharedMaterials != null && mesh?.sharedMaterials?.Length > 1)
//        {
//            mesh.sharedMaterials = new Material[] { RenderMaterial };
//        }


//        //HandleTitleUpdate(currTransform);
//        HandleSeriesUpdate(currTransform);

//        Mounted = true;
//    }

//    //private void HandleTitleUpdate(Transform currTransform)
//    //{
//    //    if (CurrentState != null && CurrentState?.Title != PreviousState?.Title)
//    //    {
//    //        var titleComponent = ObjectIterator.GetChildByNameAndLayer("Title", 5, currTransform);

//    //        if(titleComponent != null)
//    //            titleComponent.gameObject.GetComponentInChildren<TMP_Text>().SetText(CurrentState.Title);
//    //    }
//    //}

//    protected bool ShouldUpdateSeries()
//    {
//        //Debug.Log("Comparing series");
//        //Debug.Log(CurrentSeries[0].Name);
//        //Debug.Log(PreviousSeries[0].Name);
//        //Debug.Log(CurrentSeries[0].id);
//        //Debug.Log(PreviousSeries[0].id);
//        //if
//        //(
//        //    (CurrentSeries != null && PreviousSeries == null) ||
//        //    (CurrentSeries == null && PreviousSeries != null) ||
//        //    (CurrentSeries.Count != PreviousSeries.Count)
//        //)
//        //{
//        //    return true;
//        //}

//        //for (var i = 0; i < CurrentSeries.Count; i++)
//        //{
//        //    if (
//        //        (CurrentSeries[i] != null && PreviousSeries[i] == null) ||
//        //        (CurrentSeries[i] == null && PreviousSeries[i] != null) ||
//        //        (CurrentSeries[i].Entries.Count != PreviousSeries[i].Entries.Count) ||
//        //        (!CurrentSeries[i].Equals(PreviousSeries[i]))
//        //    )
//        //    {
//        //        return true;
//        //    }
//        //}

//        return false;
//    }

//    public abstract void UpdateSeries();

//    protected abstract void HandleSeriesUpdate(Transform currTransform);
//}
