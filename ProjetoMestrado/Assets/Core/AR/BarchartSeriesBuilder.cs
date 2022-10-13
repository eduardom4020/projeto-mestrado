//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//[ExecuteInEditMode]
//public class BarchartSeriesBuilder : MonoBehaviour
//{
//    [HideInInspector]
//    public BarchartSeries BarchartSeries;

//    public string Name;
//    public List<SeriesValueBuilder> Entries;

//    public void Init(BarchartSeries BarchartSeries)
//    {
//        Name = BarchartSeries.Name;
//        Entries = BarchartSeries.Entries?.Select(x => new SeriesValueBuilder(x.Key, x.Value))?.ToList() ?? new List<SeriesValueBuilder>();

//        this.BarchartSeries = BarchartSeries;
//    }

//    void Update()
//    {
//        if (BarchartSeries != null)
//        {
//            BarchartSeries.Name = Name;
//            BarchartSeries.Entries = Entries.Select(x => new SeriesValue { Key = x.Key, Value = x.Value }).ToList();

//            var entryKeyChanged = BarchartSeries.Entries.Where((x, index) => x.Key != Entries[index].Key).Count() > 0;

//            if (entryKeyChanged)
//            {
//                for (var i = 0; i < BarchartSeries.Entries.Count; i++)
//                {
//                    Entries[i].Key = BarchartSeries.Entries[i].Key;
//                }
//            }
//        }
//    }
//}
