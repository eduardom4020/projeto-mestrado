using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class EntriesJSON
{
    public string Key;
    public float Value;
    public override string ToString() => $"{{ {Key}: {Value} }}";
}

[Serializable]
public class SeriesJSON
{
    public string Name;
    public List<EntriesJSON> Entries;
    public override string ToString() => $"Series {Name}\n{string.Join("\n", Entries.Select(x => $"\t{x}"))}";
}

[Serializable]
public class ChartJSON
{
    public string VisualizationType;
    public int NumberOfSeries;
    public string Title;
    public List<SeriesJSON> Series;

    public override string ToString() => $"{VisualizationType} {Title} with {NumberOfSeries} series\n" +
        $"{string.Join("\n", Series)}";
}

[ExecuteAlways]
public class DatabaseReader : MonoBehaviour
{
    protected ChartJSON Data = null;
    public VisualizationTypeEnum ChartVisualizationType = VisualizationTypeEnum.Piechart;
    public bool GenerateChart = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GenerateChart)
        {
            var assetPath = ChartVisualizationType == VisualizationTypeEnum.Piechart
                ? "StaticDatasources/Piechart"
                : ChartVisualizationType == VisualizationTypeEnum.StackedBarChart
                ? "StaticDatasources/Barchart"
                : string.Empty;

            TextAsset textAsset = Resources.Load<TextAsset>(assetPath);

            Data = JsonUtility.FromJson<ChartJSON>(textAsset.text);

            Debug.Log(Data);

            var prefabName = ChartVisualizationType == VisualizationTypeEnum.Piechart
                ? "Charts/Piechart"
                : ChartVisualizationType == VisualizationTypeEnum.StackedBarChart
                ? "Charts/Barchart"
                : string.Empty;

            var ChartPrefab = Resources.Load<GameObject>(prefabName);
            var Instance = Instantiate(ChartPrefab);

            //DestroyImmediate(Instance.GetComponent<ChartBuilder>());

            var Chart = Instance.GetComponent<Chart>();

            Chart.Properties.VisualizationType = Data.VisualizationType == "Piechart"
                ? VisualizationTypeEnum.Piechart
                : Data.VisualizationType == "StackedBarChart"
                ? VisualizationTypeEnum.StackedBarChart
                : VisualizationTypeEnum.Piechart;
            Chart.Properties.NumberOfSeries = Data.NumberOfSeries;
            Chart.Properties.Title = Data.Title;

            Chart.Render();

            switch (Chart.Properties.VisualizationType)
            {
                case VisualizationTypeEnum.Piechart:
                    UpdatePiechartSeries(Instance.GetComponents<PieSeries>().ToList());
                    break;
                case VisualizationTypeEnum.StackedBarChart:
                    UpdateBarchartSeries(Instance.GetComponents<BarchartSeries>().ToList());
                    break;
                default:
                    break;
            }

            GenerateChart = false;
        }
    }

    protected void UpdatePiechartSeries(List<PieSeries> PieSeries)
    {
        for (var i = 0; i < PieSeries.Count; i++)
        {
            PieSeries[i].Properties.Name = Data.Series[i].Name;
            PieSeries[i].Entries = Data.Series[i].Entries
                .Select(x => new Entry<float>() { Key = x.Key, Value = x.Value })
                .ToList();

            PieSeries[i].Render();

            //Debug.Log("PieSeries");
            //Debug.Log(PieSeries[i].Properties.Name);
            //Debug.Log(string.Join("\n", PieSeries[i].Entries.Select(x => $"{x.Key}: {x.Value}")));
        }
    }

    protected void UpdateBarchartSeries(List<BarchartSeries> BarchartSeries)
    {
        for (var i = 0; i < BarchartSeries.Count; i++)
        {
            BarchartSeries[i].Properties.Name = Data.Series[i].Name;
            BarchartSeries[i].Entries = Data.Series[i].Entries
                .Select(x => new Entry<float>() { Key = x.Key, Value = x.Value })
                .ToList();

            BarchartSeries[i].Render();

            //Debug.Log("PieSeries");
            //Debug.Log(PieSeries[i].Properties.Name);
            //Debug.Log(string.Join("\n", PieSeries[i].Entries.Select(x => $"{x.Key}: {x.Value}")));
        }
    }
}
