using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiechartSeries : Series
{
    protected string NAME_ENTRY_MASK = "EntryMask";
    protected string NAME_ENTRY_SLICE = "EntrySlice";

    public PiechartSeries(string Name, List<SeriesValue> Entries) : base(Name, Entries)
    {
    }

    public PiechartSeries(string Name) : base(Name)
    {
    }

    //public static PiechartSeries FromSeries(Series series)
    //{
    //    return new PiechartSeries(series.Name, series.Entries);
    //}

    public override void Render(Transform parent)
    {
        Debug.Log("Rendered series");
        //Debug.Log(renderedSeries);
        //if (renderedSeries == null && parent.Find(GameObjectSeriesName) == null)
        //{
        //    var piechartSeries = Resources.Load<GameObject>("Charts/Series/PiechartSeries");
        //    var instantiatedObject = Object.Instantiate(piechartSeries, parent);
        //    renderedSeries = ObjectIterator.GetChildByNameAndLayer(instantiatedObject.name, 5, parent);
        //    renderedSeries.name = GameObjectSeriesName;
        //}
        //Debug.Log(renderedSeries);
        //if (renderedSeries != null && renderedSeries.name != GameObjectSeriesName)
        //{
        //    renderedSeries.name = GameObjectSeriesName;
        //}

        //if(renderedSeries != null)
        //{
        //    var mask = ObjectIterator.GetChildByNameAndLayer(NAME_ENTRY_MASK, 5, renderedSeries.transform);
        //    var slice = ObjectIterator.GetChildByNameAndLayer(NAME_ENTRY_SLICE, 5, renderedSeries.transform);
        //}
    }
}
