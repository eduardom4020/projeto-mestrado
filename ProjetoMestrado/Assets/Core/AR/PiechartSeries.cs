using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PiechartSeries : Series
{
    protected string NAME_ENTRY_MASK = "EntryMask";
    protected string NAME_ENTRY_SLICE = "EntrySlice";

    protected List<Color32> Colors = new List<Color32>();

    public PiechartSeries(string Name, List<SeriesValue> Entries) : base(Name, Entries)
    {
    }

    public PiechartSeries(string Name) : base(Name)
    {
    }

    public override void Render(Transform parent)
    {
        Debug.Log("Rendered series");

        var piechartSeries = Resources.Load<GameObject>("Charts/Series/PiechartSeries/PiechartSeries");
        var instantiatedSeries = Instantiate(piechartSeries, parent);
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
            //entryGameObject.gameObject.GetComponentInChildren<TMP_Text>().SetText(Title);
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

            var entryMask = ObjectIterator.GetChildByNameAndLayer("EntryMask", 5, entryGameObject.transform);
            var entrySlice = ObjectIterator.GetChildByNameAndLayer("EntrySlice", 5, entryGameObject.transform);
            var label = ObjectIterator.GetChildByNameAndLayer("Label", 5, entryGameObject.transform);

            if(entryMask == null && entrySlice == null && label == null) return null;

            var seriesAccValue = Entries.Take(entryIndex).Select(x => x.Value).Sum() / 100;

            var maskValue = 1 - seriesAccValue;
            var sliceValue = seriesAccValue + (seriesValue.Value / 100);

            var seriesAccDeg = (seriesAccValue + (seriesValue.Value * 0.5 / 100)) * 360 * 0.01745;

            entryMask.GetComponent<Image>().fillAmount = maskValue;
            entrySlice.GetComponent<Image>().fillAmount = sliceValue;

            if(seriesValue.Value > 0)
            {
                label.GetComponent<RectTransform>().localPosition = new Vector3
                (
                    (float)Math.Sin(seriesAccDeg) * 80 * -1,
                    (float)Math.Cos(seriesAccDeg) * 80,
                    0
                );
                label.GetComponent<TMP_Text>().SetText($"{seriesValue.Value}%");
            }
            else
            {
                label.GetComponent<RectTransform>().localPosition = Vector3.zero;
                label.GetComponent<TMP_Text>().SetText(string.Empty);
            }
            
        }

        return null;

    }

    public override void RenderEntry(SeriesValue Entry, Transform parent)
    {
        Debug.Log("Rendered entry");

        var piechartEntry = Resources.Load<GameObject>("Charts/Series/PiechartSeries/Entry");
        var instantiatedEntry = Instantiate(piechartEntry, parent);
        instantiatedEntry.name = $"{GameObjectSeriesName}_{Entry.Key}";

        if(Entries?.Count <= Colors.Count)
        {
            var entrySlice = ObjectIterator.GetChildByNameAndLayer("EntrySlice", 5, instantiatedEntry.transform);
            entrySlice.GetComponent<Image>().color = Colors[Entries.Count - 1];
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

        var newEntries = Entries.Skip(oldValue.Count);

        foreach (var entry in newEntries)
        {
            RenderEntry(entry, entriesGameObject.transform);
        }
    }
}
