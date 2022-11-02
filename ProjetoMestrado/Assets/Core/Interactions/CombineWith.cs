using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombineWith : MonoBehaviour
{
    public VisualizationTypeEnum targetVisualizationType;
    public VisualizationTypeEnum resultsInVisualizationType;

    protected GameObject Instance;
    protected Chart otherChart;
    protected Chart thisChart;

    private void Start()
    {
        thisChart = transform?.parent.GetComponent<Chart>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(thisChart == null)
        {
            thisChart = transform.parent.GetComponent<Chart>();
        }

        otherChart = other?.transform?.parent?.GetComponent<Chart>();

        if (thisChart != null && targetVisualizationType == otherChart?.Properties?.VisualizationType)
        {
            if
            (
                thisChart?.Properties.VisualizationType == VisualizationTypeEnum.Piechart &&
                resultsInVisualizationType == VisualizationTypeEnum.StackedBarChart
            )
            {
                other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                gameObject.GetComponent<MeshRenderer>().enabled = false;

                var currentPiechartSeries = transform.parent.GetComponent<PieSeries>();
                var otherPiechartSeries = other.transform.parent.GetComponent<PieSeries>();

                var prefabName = resultsInVisualizationType == VisualizationTypeEnum.Piechart
                    ? "Charts/Piechart"
                    : resultsInVisualizationType == VisualizationTypeEnum.StackedBarChart
                    ? "Charts/Barchart"
                    : string.Empty;

                var ChartPrefab = Resources.Load<GameObject>(prefabName);
                Instance = GameObject.Find($"{resultsInVisualizationType}_{thisChart.Id}")
                    ?? GameObject.Find($"{resultsInVisualizationType}_{otherChart.Id}")
                    ?? Instantiate(ChartPrefab);

                var Chart = Instance.GetComponent<Chart>();

                Chart.Id = thisChart.Id;
                Chart.Properties.VisualizationType = resultsInVisualizationType;
                Chart.Properties.NumberOfSeries = Math.Max(currentPiechartSeries.Entries.Count, otherPiechartSeries.Entries.Count);
                Chart.Properties.Title = $"{thisChart.Properties.Title} / {otherChart.Properties.Title}";

                Chart.Render();

                var BarchartSeries = Instance.GetComponents<BarchartSeries>().ToList();

                //BarchartSeries[1].Properties.Name = otherPiechartSeries.Properties.Name;
                //BarchartSeries[0].Properties.Name = currentPiechartSeries.Properties.Name;

                //var ListOfEntries = new List<ListM>

                for(var i = 0; i < Chart.Properties.NumberOfSeries; i++)
                {
                    BarchartSeries[i].Properties.Name = $"{currentPiechartSeries.Properties.Name} / {otherPiechartSeries.Properties.Name}";

                    BarchartSeries[i].Entries = new List<Entry<float>>
                    {
                        new Entry<float>()
                        {
                            Key = i < currentPiechartSeries.Entries.Count ? currentPiechartSeries.Entries[i].Key : $"Entry_{i+1}",
                            Value = i < currentPiechartSeries.Entries.Count ? currentPiechartSeries.Entries[i].Value : 0.0f
                        },
                        new Entry<float>()
                        {
                            Key = i < otherPiechartSeries.Entries.Count ? otherPiechartSeries.Entries[i].Key : $"Entry_{i+1}",
                            Value = i < otherPiechartSeries.Entries.Count ? otherPiechartSeries.Entries[i].Value : 0.0f
                        }
                    };

                    BarchartSeries[i].Render();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Destroy(Instance);

        var currentMeshRenderer = gameObject.GetComponent<MeshRenderer>();
        var otherMeshRenderer = other?.gameObject.GetComponent<MeshRenderer>();

        if (otherMeshRenderer != null)
        {
            otherMeshRenderer.enabled = true;
        }

        if (currentMeshRenderer != null)
        {
            currentMeshRenderer.enabled = true;
        }
    }
}