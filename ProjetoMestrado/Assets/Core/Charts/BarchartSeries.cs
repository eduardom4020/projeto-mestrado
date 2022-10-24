using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarchartSeries : Series<float>
{
    public float SeriesMaxValue { get; protected set; } = 0.0f;

    private bool CompareFloatEntries(float x, float y) => x == y;

    protected override void SetEntriesValueComparers()
    {
        foreach (var entry in Entries)
        {
            entry.ValueComparer = CompareFloatEntries;
        }
    }

    protected void CalculatePlotMaxValue()
    {
        var StackedSeriesMaxValues = new List<float>();

        var BarchartSeries = GetComponents<BarchartSeries>();

        foreach (var Series in BarchartSeries)
        {
            for (var i = 0; i < (Series.Entries?.Count ?? 0); i++)
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

        //TODO: Adjust this size and positioning

        var rectTransformPosition = bar.GetComponent<RectTransform>().localPosition;
        bar.GetComponent<RectTransform>().localPosition = new Vector3(posX - 350, rectTransformPosition.y, rectTransformPosition.z);
    }

    public void FlushSeries()
    {
        var piechartSeries = Resources.Load<GameObject>("Charts/Series/BarchartSeries/BarchartSeries");

        var canvasTransform = ObjectIterator.GetChildByNameAndLayer("Canvas", 5, transform)?.transform;

        var instantiatedSeries = ObjectIterator.GetChildByNameAndLayer(GameObjectSeriesName, 5, canvasTransform) ??
            Instantiate(piechartSeries, canvasTransform);

        instantiatedSeries.name = GameObjectSeriesName;

        if(Entries == null)
        {
            return;
        }

        // Todo: Execute something when series name chages

        var entriesGameObject = ObjectIterator.GetChildByNameAndLayer("Entries", 5, instantiatedSeries.transform);

        foreach (var Entry in Entries)
        {
            var sameNameEntries = Entries.Where(x => x.Key == Entry.Key).ToList();
            if (sameNameEntries.Count > 1)
            {
                for (var i = 1; i < sameNameEntries.Count; i++)
                {
                    sameNameEntries[i].Key += $" ({i})";
                }
            }
        }

        foreach (Transform entryObject in entriesGameObject.transform)
        {
            if (Entries.Count(x => $"{GameObjectSeriesName}_Entry_{x.Key}" == entryObject.gameObject.name) != 1)
            {
                DestroyImmediate(entryObject.gameObject);
            }
        }

        var barchartEntry = Resources.Load<GameObject>("Charts/Series/BarchartSeries/Entry");

        for (var i = 0; i < Entries.Count; i++)
        {
            var Entry = Entries[i];

            var instantiatedEntry = ObjectIterator.GetChildByNameAndLayer($"{GameObjectSeriesName}_Entry_{Entry.Key}", 5, entriesGameObject.transform) ??
                    Instantiate(barchartEntry, entriesGameObject.transform);

            var barMask = ObjectIterator.GetChildByNameAndLayer("BarMask", 5, instantiatedEntry.transform);

            instantiatedEntry.name = $"{GameObjectSeriesName}_Entry_{Entry.Key}";

            PlaceBar(i, instantiatedEntry, barMask);
        }

        var plotHeight = entriesGameObject.GetComponent<RectTransform>().rect.height;

        CalculatePlotMaxValue();

        var PreviousBarchartSeries = GetComponents<BarchartSeries>()
            .Where((_, i) => i < GetIndex() - 1)
            .ToList();

        for (var i = 0; i < Entries.Count; i++)
        {
            var Entry = Entries[i];

            var instantiatedEntry = ObjectIterator.GetChildByNameAndLayer($"{GameObjectSeriesName}_Entry_{Entry.Key}", 5, entriesGameObject.transform);
            var bar = ObjectIterator.GetChildByNameAndLayer("Bar", 5, instantiatedEntry.transform);
            var label = ObjectIterator.GetChildByNameAndLayer("Label", 5, bar.transform);

            var currLabelPosition = label.GetComponent<RectTransform>().localPosition;

            var previousAccValues = PreviousBarchartSeries.Sum(x => x.Entries.Count > i ? x.Entries[i].Value : 0);

            var barRelativeHeight = (previousAccValues + Entry.Value) / SeriesMaxValue;
            bar.GetComponent<Image>().fillAmount = barRelativeHeight;

            var barMask = ObjectIterator.GetChildByNameAndLayer("BarMask", 5, instantiatedEntry.transform);

            var maskRelativeHeight = 1.0f - previousAccValues / SeriesMaxValue;

            barMask.GetComponent<Image>().fillAmount = maskRelativeHeight;

            if (i < Colors.Count)
            {
                var image = ObjectIterator.GetChildByNameAndLayer("Bar", 5, barMask.transform)?.GetComponent<Image>();
                image.color = Colors[GetIndex() - 1];
            }

            if (Entry.Value > 0)
            {
                label.GetComponent<RectTransform>().localPosition = new Vector3(
                    currLabelPosition.x,
                    (barRelativeHeight * plotHeight - plotHeight) + 180,
                    currLabelPosition.z
                );
                label.GetComponent<TMP_Text>().SetText($"{Entry.Value}");
            }
            else
            {
                label.GetComponent<RectTransform>().localPosition = new Vector3(currLabelPosition.x, (plotHeight * -1.0f) + 180, currLabelPosition.z);
                label.GetComponent<TMP_Text>().SetText(string.Empty);
            }
        }
    }

    public override void Render()
    {
        FlushSeries();

        var OtherBarchartSeries = GetComponents<BarchartSeries>()
            .Where((_, i) => i != GetIndex() - 1)
            .ToList();

        foreach(var OtherSeries in OtherBarchartSeries)
        {
            OtherSeries.FlushSeries();
        }
    }
}
