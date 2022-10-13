//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//[System.Serializable]
//public class SeriesValueBuilder
//{
//    public string Key;
//    public float Value;

//    public SeriesValueBuilder(string Key, float Value)
//    {
//        this.Key = Key;
//        this.Value = Value;
//    }
//}

//[ExecuteInEditMode]
//public class PiechartSeriesBuilder : MonoBehaviour
//{
//    [HideInInspector]
//    public PiechartSeries PiechartSeries;

//    public string Name;
//    public List<SeriesValueBuilder> Entries;

//    public void Init(PiechartSeries PiechartSeries)
//    {
//        Name = PiechartSeries.Name;
//        Entries = PiechartSeries.Entries?.Select(x => new SeriesValueBuilder(x.Key, x.Value))?.ToList() ?? new List<SeriesValueBuilder>();

//        this.PiechartSeries = PiechartSeries;
//    }

//    void Update()
//    {
//        if (PiechartSeries != null)
//        {
//            PiechartSeries.Name = Name;
//            PiechartSeries.Entries = Entries.Select(x => new SeriesValue { Key = x.Key, Value = x.Value }).ToList();

//            var entryKeyChanged = PiechartSeries.Entries.Where((x, index) => x.Key != Entries[index].Key).Count() > 0;

//            if (entryKeyChanged)
//            {
//                for (var i = 0; i < PiechartSeries.Entries.Count; i++)
//                {
//                    Entries[i].Key = PiechartSeries.Entries[i].Key;
//                }
//            }
//        }
//    }
//}
