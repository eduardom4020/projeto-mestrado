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
    public VisualizationTypeEnum VisualizationType;
    public int NumberOfSeries;
    public string Title;
    public List<SeriesJSON> Series;

    public override string ToString() => $"Chart {Title} with {NumberOfSeries} series\n" +
        $"{string.Join("\n", Series)}";
}

[ExecuteAlways]
public class DatabaseReader : MonoBehaviour
{
    protected ChartJSON Data = null;
    public bool GeneratePiechart = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GeneratePiechart)
        {
            TextAsset textAsset = Resources.Load<TextAsset>("StaticDatasources/Piechart");

            Data = JsonUtility.FromJson<ChartJSON>(textAsset.text);

            Debug.Log(Data);

            var PiechartPrefab = Resources.Load<GameObject>("Charts/Piechart");
            var Instance = Instantiate(PiechartPrefab);

            //DestroyImmediate(Instance.GetComponent<ChartBuilder>());

            var Chart = Instance.GetComponent<Chart>();

            Chart.Properties.VisualizationType = Data.VisualizationType;
            Chart.Properties.NumberOfSeries = Data.NumberOfSeries;
            Chart.Properties.Title = Data.Title;

            Chart.Render();

            switch (Chart.Properties.VisualizationType)
            {
                case VisualizationTypeEnum.Piechart:
                    UpdatePiechartSeries(Instance.GetComponents<PieSeries>().ToList());
                    break;
                case VisualizationTypeEnum.StackedBarChart:
                    break;
                default:
                    break;
            }

            GeneratePiechart = false;
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
}
