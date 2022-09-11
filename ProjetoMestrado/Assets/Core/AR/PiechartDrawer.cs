using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class PiechartDrawer : ChartDrawer
{
    public override void UpdateSeries()
    {
        throw new System.NotImplementedException();
    }

    //public PiechartDrawer(ChartProperties CurrentState, ChartProperties PreviousState) : base(CurrentState, PreviousState)
    //{
    //    UpdateSeries();
    //}

    //public override void UpdateSeries()
    //{
    //    CurrentSeries = CurrentState.Series.Select(series => (Series) new PiechartSeries(series.id, series.Name, series.Entries)).ToList();
    //    PreviousSeries = PreviousState.Series.Select(series => (Series) new PiechartSeries(series.id, series.Name, series.Entries)).ToList();
    //}

    protected override void HandleSeriesUpdate(Transform currTransform)
    {
        //if(ShouldUpdateSeries())
        //{
        //    //for(var i = 0; i < CurrentSeries.Count; i++)
        //    //{

        //    //}

        //    //foreach(var series in CurrentSeries)
        //    //{
        //    //    var currentDrawer = SeriesDrawer.FirstOrDefault(x => x.Name == series.Name);

        //    //    series.Render(ObjectIterator.GetChildByNameAndLayer("Canvas", 5, currTransform)?.transform);
        //    //}
        //}
    }
}
