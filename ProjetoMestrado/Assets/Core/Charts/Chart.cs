using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IStateObservable
{
    public IStateObservable GenerateCopy();
    public bool IsEqual(IStateObservable other);
}

public class StateChangeWatcher
{
    private IStateObservable previousState;
    private IStateObservable currentState;
    public StateChangeWatcher(IStateObservable initialState)
    {
        previousState = initialState.GenerateCopy();
        currentState = initialState;
    }

    // Put this in every Update

    public bool Changed {
        get
        {
            var result = !currentState.IsEqual(previousState);

            previousState = currentState.GenerateCopy();

            return result;
        }
    }
}

[Serializable]
public class ChartProperties : IStateObservable
{
    public VisualizationTypeEnum VisualizationType = VisualizationTypeEnum.Piechart;
    public int NumberOfSeries = 0;
    public string Title;

    public string UnitSymbol;
    public UnitPositionEnum UnitPosition;
    public bool ShowUnit = false;

    public IStateObservable GenerateCopy() => new ChartProperties
    {
        NumberOfSeries = NumberOfSeries,
        Title = Title,
        UnitSymbol = UnitSymbol,
        UnitPosition = UnitPosition,
        ShowUnit = ShowUnit,
        VisualizationType = VisualizationType
    };

    public bool IsEqual(IStateObservable other)
    {
        var Other = other as ChartProperties;

        return (
            NumberOfSeries == Other.NumberOfSeries &&
            Title == Other.Title &&
            UnitSymbol == Other.UnitSymbol &&
            UnitPosition == Other.UnitPosition &&
            ShowUnit == Other.ShowUnit &&
            VisualizationType == Other.VisualizationType
        );
    }
}

[ExecuteAlways]
public class Chart : MonoBehaviour
{
    public Guid? Id { get; set; }

    private RenderTexture RenderTexture;
    private Camera RenderCamera;
    private Material RenderMaterial;

    private StateChangeWatcher PropertiesWatcher;

    public ChartProperties Properties;

    //protected void OnValidate()
    //{
    //    NumberOfSeries = NumberOfSeries > 0 ? NumberOfSeries : 0;
    //}

    //private void RemoveSeriesWithOtherTypes()
    //{
    //    if (VisualizationType != VisualizationTypeEnum.Piechart)
    //    {
    //        var PiechartSeries = GetComponents<PiechartSeries>();
    //        for (var i = PiechartSeries.Length - 1; i >= 0; i--)
    //        {
    //            DestroyImmediate(PiechartSeries[i]);
    //            Array.Resize(ref PiechartSeries, i);
    //        }
    //    }

    //    if (VisualizationType != VisualizationTypeEnum.StackedBarChart)
    //    {
    //        var BarchartSeries = GetComponents<BarchartSeries>();
    //        for (var i = BarchartSeries.Length - 1; i >= 0; i--)
    //        {
    //            DestroyImmediate(BarchartSeries[i]);
    //            Array.Resize(ref BarchartSeries, i);
    //        }
    //    }
    //}

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

    //private void FlushSeriesComponents()
    //{
    //    int seriesCount = 0;
    //    switch (VisualizationType)
    //    {
    //        case VisualizationTypeEnum.Piechart:
    //            var PiechartSeries = GetComponents<PiechartSeries>();
    //            seriesCount = PiechartSeries.Length;

    //            if (PiechartSeries == null || (NumberOfSeries > seriesCount))
    //            {
    //                PiechartSeries = new PiechartSeries[NumberOfSeries];

    //                for (var i = seriesCount; i < NumberOfSeries; i++)
    //                {
    //                    PiechartSeries[i] = gameObject.AddComponent<PiechartSeries>();
    //                    PiechartSeries[i].Init($"Series_{i + 1}");
    //                    PiechartSeries[i].Render(ObjectIterator.GetChildByNameAndLayer("Canvas", 5, transform)?.transform);
    //                }
    //            }
    //            else if (NumberOfSeries < seriesCount)
    //            {
    //                for (var i = seriesCount - 1; i >= NumberOfSeries; i--)
    //                {
    //                    PiechartSeries[i].Destroy(ObjectIterator.GetChildByNameAndLayer("Canvas", 5, transform)?.transform);
    //                    DestroyImmediate(PiechartSeries[i]);
    //                    Array.Resize(ref PiechartSeries, i);
    //                }
    //            }

    //            for (var i = 0; i < PiechartSeries.Length; i++)
    //            {
    //                PiechartSeries[i].SeriesIndex = i;
    //            }

    //            break;
    //        case VisualizationTypeEnum.StackedBarChart:
    //            var BarchartSeries = GetComponents<BarchartSeries>();
    //            seriesCount = BarchartSeries.Length;

    //            if (BarchartSeries == null || (NumberOfSeries > seriesCount))
    //            {
    //                BarchartSeries = new BarchartSeries[NumberOfSeries];

    //                for (var i = seriesCount; i < NumberOfSeries; i++)
    //                {
    //                    BarchartSeries[i] = gameObject.AddComponent<BarchartSeries>();
    //                    BarchartSeries[i].Init($"Series_{i + 1}");
    //                    BarchartSeries[i].Render(ObjectIterator.GetChildByNameAndLayer("Canvas", 5, transform)?.transform);
    //                }
    //            }
    //            else if (NumberOfSeries < seriesCount)
    //            {
    //                for (var i = seriesCount - 1; i >= NumberOfSeries; i--)
    //                {
    //                    BarchartSeries[i].Destroy(ObjectIterator.GetChildByNameAndLayer("Canvas", 5, transform)?.transform);
    //                    DestroyImmediate(BarchartSeries[i]);
    //                    Array.Resize(ref BarchartSeries, i);
    //                }
    //            }

    //            for (var i = 0; i < BarchartSeries.Length; i++)
    //            {
    //                if (BarchartSeries[i] != null)
    //                {
    //                    BarchartSeries[i].SeriesIndex = i;
    //                    BarchartSeries[i].FlushBars();
    //                }
    //            }

    //            break;
    //        default:
    //            break;
    //    }
    //}

    private void SetupRenderer(Transform parentTransform)
    {
        int initialImageRendererPosition = 0;
        foreach (var arObject in GameObject.FindGameObjectsWithTag("ARObject"))
        {
            var imageRenderer = ObjectIterator.GetChildByNameAndLayer("ImageRenderer", 5, arObject.transform);

            if (imageRenderer)
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
            var ImageRenderer = ObjectIterator.GetChildByNameAndLayer("ImageRenderer", 5, parentTransform);
            var BgRectTransform = ObjectIterator.GetChildByNameAndLayer("Bg", 5, ImageRenderer.transform)?.GetComponent<RectTransform>();

            RenderTexture = new RenderTexture((int)BgRectTransform.rect.width, (int)BgRectTransform.rect.height, 16, RenderTextureFormat.ARGB32);
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
        Id = Guid.NewGuid();
        gameObject.name = $"{Properties.VisualizationType}_{Id}";

        SetupRenderer(transform);
        PropertiesWatcher = new StateChangeWatcher(Properties);
        Render();
    }

    void Update()
    {
        if(RenderTexture == null || RenderCamera == null || RenderMaterial == null)
        {
            SetupRenderer(transform);
        }

        if(PropertiesWatcher == null)
        {
            PropertiesWatcher = new StateChangeWatcher(Properties);
        }

        if(PropertiesWatcher.Changed)
        {
            Render();
        }
    }

    public void Render()
    {
        if(Id == null)
        {
            Id = Guid.NewGuid();
        }

        gameObject.name = $"{Properties.VisualizationType}_{Id}";

        var titleComponent = ObjectIterator.GetChildByNameAndLayer("Title", 5, transform);

        if (titleComponent != null)
        {
            titleComponent.gameObject.GetComponentInChildren<TMP_Text>().SetText(Properties.Title);
        }

        Series<float>[] Series = { };

        switch (Properties.VisualizationType)
        {
            case VisualizationTypeEnum.Piechart:
                Series = GetComponents<PieSeries>();
                break;
            case VisualizationTypeEnum.StackedBarChart:
                Series = GetComponents<BarchartSeries>();
                break;
            default:
                throw new Exception("Invalid VisualizationType!");
        }

        var seriesCount = Series.Length;

        if (Series == null || (Properties.NumberOfSeries > seriesCount))
        {
            switch (Properties.VisualizationType)
            {
                case VisualizationTypeEnum.Piechart:
                    Series = new PieSeries[Properties.NumberOfSeries];
                    break;
                case VisualizationTypeEnum.StackedBarChart:
                    Series = new BarchartSeries[Properties.NumberOfSeries];
                    break;
                default:
                    throw new Exception("Invalid VisualizationType!");
            }
            
            for (var i = seriesCount; i < Properties.NumberOfSeries; i++)
            {
                switch (Properties.VisualizationType)
                {
                    case VisualizationTypeEnum.Piechart:
                        Series[i] = gameObject.AddComponent<PieSeries>();
                        break;
                    case VisualizationTypeEnum.StackedBarChart:
                        Series[i] = gameObject.AddComponent<BarchartSeries>();
                        break;
                    default:
                        throw new Exception("Invalid VisualizationType!");
                }

                if(Series[i].Properties == null)
                {
                    Series[i].Properties = new SeriesProperties();
                }

                Series[i].SetSeriesIndex();
                Series[i].Properties.Name = $"Series_{Series[i].GetIndex()}";

                //Init($"Series_{i + 1}");

                //Series[i].Init($"Series_{i + 1}");
                //Series[i].Render(ObjectIterator.GetChildByNameAndLayer("Canvas", 5, transform)?.transform);
            }
        }
        else if (Properties.NumberOfSeries < seriesCount)
        {
            for (var i = seriesCount - 1; i >= Properties.NumberOfSeries; i--)
            {
                DestroyImmediate(ObjectIterator.GetChildByNameAndLayer(Series[i].Properties.Name, 5, transform));
                DestroyImmediate(Series[i]);
                Array.Resize(ref Series, i);
            }
        }

        //foreach (var someSeries in Series)
        //{
        //    someSeries.Render();
        //}
    }
}
