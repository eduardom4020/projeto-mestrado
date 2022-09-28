using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarchartSeries : Series
{
    protected int EntryIndex = 0;
    protected List<Color32> Colors = new List<Color32>();

    public BarchartSeries(string Name, List<SeriesValue> Entries) : base(Name, Entries)
    {
    }

    public BarchartSeries(string Name) : base(Name)
    {
    }

    public override void Render(Transform parent)
    {
        Debug.Log("Rendered series");

        var barchartSeries = Resources.Load<GameObject>("Charts/Series/BarchartSeries/BarchartSeries");
        var instantiatedSeries = Instantiate(barchartSeries, parent);
        instantiatedSeries.name = GameObjectSeriesName;

        Colors = new List<Color32>
        {
            new Color32(49, 86, 230, 255),
            new Color32(255, 61, 253, 255),
            new Color32(24, 66, 137, 255),
            new Color32(165, 64, 217, 255)
        };
    }

    protected override string OnKeyChanged(SeriesValue seriesValue, string oldValue)
    {
        Debug.Log("On key changed!");
        var entriesGameObject = GetEntriesGameObject();

        if (entriesGameObject == null)
        {
            return null;
        }

        var entryGameObject = ObjectIterator.GetChildByNameAndLayer($"{GameObjectSeriesName}_{oldValue}", 5, entriesGameObject.transform);

        if (entryGameObject != null)
        {
            entryGameObject.name = $"{GameObjectSeriesName}_{seriesValue.Key}";
        }

        return null;
    }

    protected override string OnValueChanged(SeriesValue seriesValue, float oldValue)
    {
        Debug.Log("On value changed!");
        var entriesGameObject = GetEntriesGameObject();

        if (entriesGameObject == null)
        {
            return null;
        }

        var entryGameObject = ObjectIterator.GetChildByNameAndLayer($"{GameObjectSeriesName}_{seriesValue.Key}", 5, entriesGameObject.transform);

        if (entryGameObject != null)
        {
            var entryIndex = Entries.FindIndex(x => x.Key == seriesValue.Key);

            if (entryIndex < 0) return null;

            var bar = ObjectIterator.GetChildByNameAndLayer("Bar", 5, entryGameObject.transform);

            if (bar == null) return null;

            var label = ObjectIterator.GetChildByNameAndLayer("Label", 5, bar.transform);

            if (label == null) return null;

            //if (seriesValue.Value > 0)
            //{
            //    label.GetComponent<RectTransform>().localPosition = new Vector3
            //    (
            //        (float)Math.Sin(seriesAccDeg) * 80 * -1,
            //        (float)Math.Cos(seriesAccDeg) * 80,
            //        0
            //    );
            //    label.GetComponent<TMP_Text>().SetText($"{seriesValue.Value}%");
            //}
            //else
            //{
            //    label.GetComponent<RectTransform>().localPosition = Vector3.zero;
            //    label.GetComponent<TMP_Text>().SetText(string.Empty);
            //}

        }

        return null;

    }

    protected void PlaceBar(int index, GameObject instantiatedEntry, GameObject bar)
    {
        var plotSize = instantiatedEntry.GetComponent<RectTransform>().rect.width;
        var sliceSize = 1.0f / Entries.Count;

        var posX = (sliceSize * index + sliceSize / 2.0f) * plotSize;

        var rectTransformPosition = bar.GetComponent<RectTransform>().localPosition;
        bar.GetComponent<RectTransform>().localPosition = new Vector3(posX - 350, rectTransformPosition.y, rectTransformPosition.z);
    }

    public override void RenderEntry(SeriesValue Entry, Transform parent)
    {
        Debug.Log("Rendered entry");

        var barchartEntry = Resources.Load<GameObject>("Charts/Series/BarchartSeries/Entry");
        var instantiatedEntry = Instantiate(barchartEntry, parent);
        instantiatedEntry.name = $"{GameObjectSeriesName}_{Entry.Key}";

        var bar = ObjectIterator.GetChildByNameAndLayer("Bar", 5, instantiatedEntry.transform);

        PlaceBar(EntryIndex, instantiatedEntry, bar);

        if (Entries?.Count <= Colors.Count)
        {
            bar.GetComponent<Image>().color = Colors[0];
        }

        Entry.Key = $"Entry_{Entries.Count}";
        Entry.Value = 0;

        OnKeyChanged(Entry, string.Empty);
        OnValueChanged(Entry, 0);
    }

    protected override void OnEntriesChanged(List<SeriesValue> oldValue)
    {
        var entriesGameObject = GetEntriesGameObject();

        if(entriesGameObject == null)
        {
            return;
        }

        base.OnEntriesChanged(oldValue);

        EntryIndex = 0;

        var oldEntries = Entries.Take(oldValue.Count);

        foreach (var entry in oldEntries)
        {
            var instantiatedEntry = ObjectIterator.GetChildByNameAndLayer($"{GameObjectSeriesName}_{entry.Key}", 5, entriesGameObject.transform);
            var bar = ObjectIterator.GetChildByNameAndLayer("Bar", 5, instantiatedEntry.transform);

            PlaceBar(EntryIndex, instantiatedEntry, bar);
            EntryIndex += 1;
        }

        var newEntries = Entries.Skip(oldValue.Count);

        foreach (var entry in newEntries)
        {
            RenderEntry(entry, entriesGameObject.transform);
            EntryIndex += 1;
        }

        EntryIndex = 0;
    }
}
