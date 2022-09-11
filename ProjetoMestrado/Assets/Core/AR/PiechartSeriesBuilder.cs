using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SeriesValueBuilder
{
    public string Key;
    public float Value;

    public SeriesValueBuilder(string Key, float Value)
    {
        this.Key = Key;
        this.Value = Value;
    }
}

[ExecuteInEditMode]
public class PiechartSeriesBuilder : MonoBehaviour
{
    [HideInInspector]
    public PiechartSeries PiechartSeries;

    public string Name;
    public List<SeriesValueBuilder> Entries;

    public void Init(PiechartSeries PiechartSeries)
    {
        Name = PiechartSeries.Name;
        Entries = PiechartSeries.Entries.Select(x => new SeriesValueBuilder(x.Key, x.Value)).ToList();

        this.PiechartSeries = PiechartSeries;
    }

    // Update is called once per frame
    void Update()
    {
        if (PiechartSeries != null)
        {
            PiechartSeries.Name = Name;

            if(Entries.Count >= PiechartSeries.Entries?.Count)
            {
                var updatedEntries = Entries
                .Take(PiechartSeries.Entries.Count)
                .ToList();

                for (int i = 0; i < updatedEntries.Count; i++)
                {
                    PiechartSeries.Entries[i].Key = updatedEntries[i].Key;
                    PiechartSeries.Entries[i].Value = updatedEntries[i].Value;
                }

                var newEntries = Entries
                    .Skip(PiechartSeries.Entries.Count)
                    .Select(x => new SeriesValue { Key = x.Key, Value = x.Value })
                    .ToList();

                //Debug.Log("ENTRIES > PIECHART SERIES ENTRIES");
                //Debug.Log(updatedEntries.Count);
                //Debug.Log(newEntries.Count);

                PiechartSeries.AddEntries(newEntries);
            }
            else
            {
                PiechartSeries.RemoveEntries(Entries.Count, PiechartSeries.Entries.Count - Entries.Count);
            }
        }
    }
}
