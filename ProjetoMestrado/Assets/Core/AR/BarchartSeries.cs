using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarchartSeries : Series
{
    public float SeriesMaxValue { get; protected set; } = 0.0f;

    public int EntryIndex { get; set; } = 0;
    protected List<Color32> Colors = new List<Color32>();

    public BarchartSeries(string Name, List<SeriesValue> Entries) : base(Name, Entries)
    {
    }

    public BarchartSeries(string Name) : base(Name)
    {
    }

    public override void Render(Transform parent)
    {
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

        //SeriesMaxValue = 0.0f;
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

    protected void CalculatePlotMaxValue()
    {
        var StackedSeriesMaxValues = new List<float>();

        var BarchartSeries = GetComponents<BarchartSeries>();
        //.Where((_, i) => i != SeriesIndex)
        //.ToList();

        foreach (var Series in BarchartSeries)
        {
            for (var i = 0; i < Series.Entries.Count; i++)
            {
                if (StackedSeriesMaxValues.Count < i + 1)
                {
                    StackedSeriesMaxValues.Add(0);
                }

                StackedSeriesMaxValues[i] += Series.Entries[i].Value;
            }
        }

        SeriesMaxValue = StackedSeriesMaxValues.Count > 0 ? StackedSeriesMaxValues.Max() : 0.0f;
    }

    protected override string OnValueChanged(SeriesValue seriesValue, float oldValue)
    {
        Debug.Log("On value changed!");
        var entriesGameObject = GetEntriesGameObject();

        if (entriesGameObject == null)
        {
            return null;
        }

        var plotHeight = entriesGameObject.GetComponent<RectTransform>().rect.height;

        CalculatePlotMaxValue();

        var PreviousBarchartSeries = GetComponents<BarchartSeries>()
            .Where((_, i) => i < SeriesIndex)
            .ToList();

        //foreach (var Series in BarchartSeries)
        //{
        //    for (var i = 0; i < Series.Entries.Count; i++)
        //    {
        //        if (StackedSeriesMaxValues.Count < i + 1)
        //        {
        //            StackedSeriesMaxValues.Add(0);
        //        }

        //        StackedSeriesMaxValues[i] += Series.Entries[i].Value;
        //    }
        //}

        for (var i = 0; i < Entries.Count; i++)
        {
            var entry = Entries[i];

            var instantiatedEntry = ObjectIterator.GetChildByNameAndLayer($"{GameObjectSeriesName}_{entry.Key}", 5, entriesGameObject.transform);
            var bar = ObjectIterator.GetChildByNameAndLayer("Bar", 5, instantiatedEntry.transform);
            var label = ObjectIterator.GetChildByNameAndLayer("Label", 5, bar.transform);

            var currLabelPosition = label.GetComponent<RectTransform>().localPosition;

            var previousAccValues = PreviousBarchartSeries.Sum(x => x.Entries[i].Value);

            //Debug.Log("previousAccValues");
            //Debug.Log(previousAccValues);

            var barRelativeHeight = (previousAccValues + entry.Value) / SeriesMaxValue;
            bar.GetComponent<Image>().fillAmount = barRelativeHeight;


            var barMask = ObjectIterator.GetChildByNameAndLayer("BarMask", 5, instantiatedEntry.transform);

            var maskRelativeHeight = (1 - entry.Value) / SeriesMaxValue;
            barMask.GetComponent<Image>().fillAmount = maskRelativeHeight;

            if (entry.Value > 0)
            {
                label.GetComponent<RectTransform>().localPosition = new Vector3(
                    currLabelPosition.x,
                    (barRelativeHeight * plotHeight - plotHeight) + 180,
                    currLabelPosition.z
                );
                label.GetComponent<TMP_Text>().SetText($"{entry.Value}");
            }
            else
            {
                label.GetComponent<RectTransform>().localPosition = new Vector3(currLabelPosition.x, (plotHeight * -1.0f) + 180, currLabelPosition.z);
                label.GetComponent<TMP_Text>().SetText(string.Empty);
            }
        }

        return null;

    }

    protected void PlaceBar(int index, GameObject instantiatedEntry, GameObject bar)
    {
        var plotSize = instantiatedEntry.GetComponent<RectTransform>().rect.width;

        var maximumPlacedBars = 0;

        var BarchartSeries = GetComponents<BarchartSeries>();

        foreach (var Series in BarchartSeries)
        {
            if (maximumPlacedBars < Series?.Entries?.Count)
            {
                maximumPlacedBars = Series.Entries.Count;
            }
        }

        var sliceSize = 1.0f / maximumPlacedBars;

        var posX = (sliceSize * index + sliceSize / 2.0f) * plotSize;

        var rectTransformPosition = bar.GetComponent<RectTransform>().localPosition;
        bar.GetComponent<RectTransform>().localPosition = new Vector3(posX - 350, rectTransformPosition.y, rectTransformPosition.z);
    }

    public void FlushBars()
    {
        var entriesGameObject = GetEntriesGameObject();

        if (entriesGameObject == null)
        {
            return;
        }

        EntryIndex = 0;

        foreach (var entry in Entries)
        {
            var instantiatedEntry = ObjectIterator.GetChildByNameAndLayer($"{GameObjectSeriesName}_{entry.Key}", 5, entriesGameObject.transform);
            var barMask = ObjectIterator.GetChildByNameAndLayer("BarMask", 5, instantiatedEntry.transform);

            PlaceBar(EntryIndex, instantiatedEntry, barMask);
            EntryIndex += 1;
        }

        EntryIndex = 0;

        var plotHeight = entriesGameObject.GetComponent<RectTransform>().rect.height;

        CalculatePlotMaxValue();

        var PreviousBarchartSeries = GetComponents<BarchartSeries>()
            .Where((_, i) => i < SeriesIndex)
            .ToList();

        for (var i = 0; i < Entries.Count; i++)
        {
            var entry = Entries[i];

            var instantiatedEntry = ObjectIterator.GetChildByNameAndLayer($"{GameObjectSeriesName}_{entry.Key}", 5, entriesGameObject.transform);
            var bar = ObjectIterator.GetChildByNameAndLayer("Bar", 5, instantiatedEntry.transform);
            var label = ObjectIterator.GetChildByNameAndLayer("Label", 5, bar.transform);

            var currLabelPosition = label.GetComponent<RectTransform>().localPosition;

            var previousAccValues = PreviousBarchartSeries.Sum(x => x.Entries.Count > i ? x.Entries[i].Value : 0);

            Debug.Log("previousAccValues");
            Debug.Log(previousAccValues);

            Debug.Log("entry.Value");
            Debug.Log(entry.Value);

            var barRelativeHeight = (previousAccValues + entry.Value) / SeriesMaxValue;

            Debug.Log("barRelativeHeight");
            Debug.Log(barRelativeHeight);
            bar.GetComponent<Image>().fillAmount = barRelativeHeight;


            var barMask = ObjectIterator.GetChildByNameAndLayer("BarMask", 5, instantiatedEntry.transform);

            var maskRelativeHeight = 1.0f - previousAccValues / SeriesMaxValue;

            Debug.Log("maskRelativeHeight");
            Debug.Log(maskRelativeHeight);

            barMask.GetComponent<Image>().fillAmount = maskRelativeHeight;

            if (entry.Value > 0)
            {
                label.GetComponent<RectTransform>().localPosition = new Vector3(
                    currLabelPosition.x,
                    (barRelativeHeight * plotHeight - plotHeight) + 180,
                    currLabelPosition.z
                );
                label.GetComponent<TMP_Text>().SetText($"{entry.Value}");
            }
            else
            {
                label.GetComponent<RectTransform>().localPosition = new Vector3(currLabelPosition.x, (plotHeight * -1.0f) + 180, currLabelPosition.z);
                label.GetComponent<TMP_Text>().SetText(string.Empty);
            }
        }
    }

    public override void RenderEntry(SeriesValue Entry, Transform parent)
    {
        Debug.Log("Rendered entry");

        var barchartEntry = Resources.Load<GameObject>("Charts/Series/BarchartSeries/Entry");
        var instantiatedEntry = Instantiate(barchartEntry, parent);
        instantiatedEntry.name = $"{GameObjectSeriesName}_{Entry.Key}";

        var barMask = ObjectIterator.GetChildByNameAndLayer("BarMask", 5, instantiatedEntry.transform);

        PlaceBar(EntryIndex, instantiatedEntry, barMask);

        if(SeriesIndex != null)
        {
            var bar = ObjectIterator.GetChildByNameAndLayer("Bar", 5, barMask.transform);
            bar.GetComponent<Image>().color = Colors[SeriesIndex.Value];
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
            var barMask = ObjectIterator.GetChildByNameAndLayer("BarMask", 5, instantiatedEntry.transform);

            PlaceBar(EntryIndex, instantiatedEntry, barMask);
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
