using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChartBuilder : MonoBehaviour
{
    private Chart Chart;

    public VisualizationTypeEnum VisualizationType;
    public int NumberOfSeries = 0;
    public string Title;
    public string UnitSymbol;
    public UnitPositionEnum UnitPosition;
    public bool ShowUnit;

    private void SetChart()
    {
        if (GetComponent<Chart>() == null)
        {
            Chart = gameObject.AddComponent<Chart>();
        }
        else
        {
            Chart = GetComponent<Chart>();
        }
    }

    private void RemoveSeriesBuildersComponents()
    {
        var PiechartSeriesBuilder = GetComponents<PiechartSeriesBuilder>();
        for (var i = PiechartSeriesBuilder.Length - 1; i >= 0; i--)
        {
            DestroyImmediate(PiechartSeriesBuilder[i]);
            Array.Resize(ref PiechartSeriesBuilder, i);
        }
    }

    private void AddSeriesBuildersComponents()
    {
        if(VisualizationType == VisualizationTypeEnum.Piechart)
        {
            var PiechartSeries = GetComponents<PiechartSeries>();
            foreach(var piechartSeries in PiechartSeries)
            {
                var piechartBuilder = gameObject.AddComponent<PiechartSeriesBuilder>();
                piechartBuilder.Init(piechartSeries);
            }
        }
    }

    private void FlushSeriesBuildersComponents()
    {
        if (VisualizationType == VisualizationTypeEnum.Piechart)
        {
            var PiechartSeries = GetComponents<PiechartSeries>();
            var PiechartSeriesBuilders = GetComponents<PiechartSeriesBuilder>();

            for (int i = 0; i < PiechartSeries.Length; i++)
            {
                PiechartSeriesBuilders[i].Init(PiechartSeries[i]);
            }
        }
    }

    private void OnValidate()
    {
        NumberOfSeries = NumberOfSeries > 0 ? NumberOfSeries : 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetChart();
    }

    // Update is called once per frame
    void Update()
    {
        if(Chart == null)
        {
            SetChart();
        }

        var numberOfSeriesChanged = Chart.NumberOfSeries != NumberOfSeries;
        var visualizationTypeChanged = Chart.VisualizationType != VisualizationType;

        Chart.VisualizationType = VisualizationType;
        Chart.NumberOfSeries = NumberOfSeries;
        Chart.Title = Title;
        Chart.UnitSymbol = UnitSymbol;
        Chart.UnitPosition = UnitPosition;
        Chart.ShowUnit = ShowUnit;

        if (numberOfSeriesChanged || visualizationTypeChanged)
        {
            RemoveSeriesBuildersComponents();
            AddSeriesBuildersComponents();
        }
    }
}
