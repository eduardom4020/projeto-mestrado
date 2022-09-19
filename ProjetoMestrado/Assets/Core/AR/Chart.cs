using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class Chart : MonoBehaviour
{
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

    //public List<Series> Series;


    //public ChartProperties Properties;
    //private ChartProperties PreviousState;

    //private ChartDrawer ChartDrawer;

    //private void SetPreviousState() 
    //{
    //    if(Properties != null)
    //    {
    //        PreviousState = new ChartProperties
    //        {
    //            Title = Properties.Title,
    //            VisualizationType = Properties.VisualizationType,
    //            UnitSymbol = Properties.UnitSymbol,
    //            UnitPosition = Properties.UnitPosition,
    //            ShowUnit = Properties.ShowUnit,
    //            Series = new List<Series>()
    //        };

    //        PreviousState.Series.AddRange(Properties.Series);
    //    }
    //}

    //private ChartDrawer NewChartDrawer()
    //{
    //    switch (Properties.VisualizationType)
    //    {
    //        case VisualizationTypeEnum.Piechart:
    //            return new PiechartDrawer(Properties, PreviousState);
    //        case VisualizationTypeEnum.StackedBarChart:
    //            throw new System.NotImplementedException();
    //        default:
    //            return null;
    //    }
    //}

    protected void OnValidate()
    {
        NumberOfSeries = NumberOfSeries > 0 ? NumberOfSeries : 0;
    }

    private void Start()
    {
        //Debug.Log("HERE Starting Chart!!");
        //SetPreviousState();

        //ChartDrawer = NewChartDrawer();
    }

    protected void OnVisualizationTypeChanged(VisualizationTypeEnum oldValue)
    {
        //Debug.Log("Changed VisualizationType from ");
        //Debug.Log(oldValue);
        //Debug.Log(" TO ");
        //Debug.Log(VisualizationType);

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
                    Debug.Log("Adding series");
                    Debug.Log($"Series_{i + 1}");
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

    void Update()
    {
        FlushSeriesComponents();
        //    Debug.Log("On update Chart Drawer");
        //    Debug.Log(ChartDrawer);

        //    if (ChartDrawer == null)
        //        ChartDrawer = NewChartDrawer();

        //    ChartDrawer.UpdateSeries();

        //    //if(Properties != null && PreviousState != null)
        //    //    PiechartDrawer.SetStates(Properties, PreviousState);

        //    ChartDrawer.ListenToPropsChange(transform);

        //    //switch (VisualizationType)
        //    //{
        //    //    case VisualizationTypeEnum.Piechart:

        //    //    default:
        //    //        return;
        //    //}

        //    SetPreviousState();
    }
}
