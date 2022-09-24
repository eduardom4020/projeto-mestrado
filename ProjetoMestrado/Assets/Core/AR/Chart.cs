using System;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class Chart : MonoBehaviour
{
    private bool Mounted = false;

    private RenderTexture RenderTexture;
    private Camera RenderCamera;
    private Material RenderMaterial;

    private VisualizationTypeEnum _VisualizationType = VisualizationTypeEnum.Piechart;
    public VisualizationTypeEnum VisualizationType
    {
        get { return _VisualizationType; }
        set
        {
            if (_VisualizationType == value) return;

            var oldVaue = _VisualizationType;
            _VisualizationType = value;

            OnVisualizationTypeChanged(oldVaue);
        }
    }

    private int _NumberOfSeries = 0;
    public int NumberOfSeries
    {
        get { return _NumberOfSeries; }
        set
        {
            if (_NumberOfSeries == value) return;

            var oldVaue = _NumberOfSeries;
            _NumberOfSeries = value;

            OnNumberOfSeriesChanged(oldVaue);
        }
    }

    private string _Title;
    public string Title
    {
        get { return _Title; }
        set
        {
            if (_Title == value) return;

            var oldVaue = _Title;
            _Title = value;

            OnTitleChanged(oldVaue);
        }
    }

    private string _UnitSymbol;
    public string UnitSymbol
    {
        get { return _UnitSymbol; }
        set
        {
            if (_UnitSymbol == value) return;

            var oldVaue = _UnitSymbol;
            _UnitSymbol = value;

            OnUnitSymbolChanged(oldVaue);
        }
    }

    private UnitPositionEnum _UnitPosition;
    public UnitPositionEnum UnitPosition
    {
        get { return _UnitPosition; }
        set
        {
            if (_UnitPosition == value) return;

            var oldVaue = _UnitPosition;
            _UnitPosition = value;

            OnUnitPositionChanged(oldVaue);
        }
    }

    private bool _ShowUnit;
    public bool ShowUnit
    {
        get { return _ShowUnit; }
        set
        {
            if (_ShowUnit == value) return;

            var oldVaue = _ShowUnit;
            _ShowUnit = value;

            OnShowUnitChanged(oldVaue);
        }
    }

    protected void OnValidate()
    {
        NumberOfSeries = NumberOfSeries > 0 ? NumberOfSeries : 0;
    }

    protected void OnVisualizationTypeChanged(VisualizationTypeEnum oldValue)
    {
        RemoveSeriesWithOtherTypes();
    }

    protected void OnNumberOfSeriesChanged(int oldValue)
    {
        RemoveSeriesWithOtherTypes();
        FlushSeriesComponents();
    }

    protected void OnTitleChanged(string oldValue)
    {
        var titleComponent = ObjectIterator.GetChildByNameAndLayer("Title", 5, transform);

        if (titleComponent != null)
        {
            titleComponent.gameObject.GetComponentInChildren<TMP_Text>().SetText(Title);
        }
    }

    protected void OnUnitSymbolChanged(string oldValue)
    {

    }

    protected void OnUnitPositionChanged(UnitPositionEnum oldValue)
    {

    }

    protected void OnShowUnitChanged(bool oldValue)
    {

    }

    private void RemoveSeriesWithOtherTypes()
    {
        if (VisualizationType != VisualizationTypeEnum.Piechart)
        {
            var PiechartSeries = GetComponents<PiechartSeries>();
            for (var i = PiechartSeries.Length - 1; i >= 0; i--)
            {
                DestroyImmediate(PiechartSeries[i]);
                Array.Resize(ref PiechartSeries, i);
            }
        }
    }

    //foreach (var series in existingSeries)
    //{
    //    Debug.Log("series.Name");
    //    Debug.Log(series.name.Replace("Series_", string.Empty));
    //    Debug.Log("series.Entries");
    //    foreach (Transform entry in ObjectIterator.GetChildByNameAndLayer("Entries", 5, series.transform)?.transform)
    //    {
    //        Debug.Log($"{entry.name.Replace($"{series.name}_", string.Empty)}: {ObjectIterator.GetChildByNameAndLayer("Label", 5, entry)?.GetComponentInChildren<TMP_Text>().text.Replace("%", string.Empty)}");
    //    }
    //}

    private void FlushSeriesComponents()
    {
        if (VisualizationType == VisualizationTypeEnum.Piechart)
        {
            var PiechartSeries = GetComponents<PiechartSeries>();
            var seriesCount = PiechartSeries.Length;

            if (PiechartSeries == null || (NumberOfSeries > seriesCount))
            {
                PiechartSeries = new PiechartSeries[NumberOfSeries];

                for (var i = seriesCount; i < NumberOfSeries; i++)
                {
                    PiechartSeries[i] = gameObject.AddComponent<PiechartSeries>();
                    PiechartSeries[i].Init($"Series_{i + 1}");
                    PiechartSeries[i].Render(ObjectIterator.GetChildByNameAndLayer("Canvas", 5, transform)?.transform);
                }
            }
            else if(NumberOfSeries < seriesCount)
            {
                for (var i = seriesCount - 1; i >= NumberOfSeries; i--)
                {
                    PiechartSeries[i].Destroy(ObjectIterator.GetChildByNameAndLayer("Canvas", 5, transform)?.transform);
                    DestroyImmediate(PiechartSeries[i]);
                    Array.Resize(ref PiechartSeries, i);
                }
            }
        }
    }

    private void SetupRenderer(Transform parentTransform)
    {
        int initialImageRendererPosition = 0;
        foreach(var arObject in GameObject.FindGameObjectsWithTag("ARObject"))
        {
            var imageRenderer = ObjectIterator.GetChildByNameAndLayer("ImageRenderer", 5, arObject.transform);

            if(imageRenderer)
            {
                imageRenderer.transform.localPosition = new Vector3
                (
                    imageRenderer.transform.localPosition.x,
                    imageRenderer.transform.localPosition.y,
                    initialImageRendererPosition += 100
                );
            }
        }

        if (RenderTexture == null)
        {
            RenderTexture = new RenderTexture(400, 455, 16, RenderTextureFormat.ARGB32);
            RenderTexture.depth = 24;

            var createdWithSuccess = RenderTexture.Create();

            if (!createdWithSuccess)
                RenderTexture = null;
        }

        if (RenderCamera == null)
            RenderCamera = ObjectIterator.GetChildByNameAndLayer("Camera", 5, parentTransform)?.GetComponent<Camera>();

        if (RenderCamera != null && RenderCamera.targetTexture == null)
            RenderCamera.targetTexture = RenderTexture;

        if (RenderMaterial == null && RenderCamera?.targetTexture != null)
        {
            RenderMaterial = new Material(Shader.Find("Unlit/Transparent"));
            RenderMaterial.SetTexture("_MainTex", RenderCamera.targetTexture);
        }

        var mesh = ObjectIterator.GetChildByNameAndLayer("RenderedSurface", 0, parentTransform)?.GetComponent<MeshRenderer>();

        if (RenderMaterial != null)
            mesh.sharedMaterial = RenderMaterial;


        if (mesh?.sharedMaterials != null && mesh?.sharedMaterials?.Length > 1)
        {
            mesh.sharedMaterials = new Material[] { RenderMaterial };
        }
    }

    private void Start()
    {
        Mounted = false;
    }

    void Update()
    {
        if(!Mounted)
        {
            SetupRenderer(transform);
            Mounted = true;
        }

        FlushSeriesComponents();
    }
}
