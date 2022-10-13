using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PieSeries : Series<float>
{
    private bool CompareFloatEntries(float x, float y) => x == y;

    protected override void SetEntriesValueComparers()
    {
        foreach(var entry in Entries)
        {
            entry.ValueComparer = CompareFloatEntries;
        }
    }

    //    protected string NAME_ENTRY_MASK = "EntryMask";
    //    protected string NAME_ENTRY_SLICE = "EntrySlice";

    //    protected List<Color32> Colors = new List<Color32>();

    //    public PiechartSeries(string Name, List<SeriesValue> Entries) : base(Name, Entries)
    //    {
    //    }

    //    public PiechartSeries(string Name) : base(Name)
    //    {
    //    }

    public override void Render()
    {
        var piechartSeries = Resources.Load<GameObject>("Charts/Series/PiechartSeries/PiechartSeries");

        var canvasTransform = ObjectIterator.GetChildByNameAndLayer("Canvas", 5, transform)?.transform;

        var instantiatedSeries = ObjectIterator.GetChildByNameAndLayer(GameObjectSeriesName, 5, canvasTransform) ??
            Instantiate(piechartSeries, canvasTransform);

        instantiatedSeries.name = GameObjectSeriesName;

        // Todo: Execute something when series name chages

        var entriesGameObject = ObjectIterator.GetChildByNameAndLayer("Entries", 5, instantiatedSeries.transform);

        foreach(var Entry in Entries)
        {
            var sameNameEntries = Entries.Where(x => x.Key == Entry.Key).ToList();
            if (sameNameEntries.Count > 1)
            {
                for(var i = 1; i < sameNameEntries.Count; i++)
                {
                    sameNameEntries[i].Key += $" ({i})";
                }
            }
        }

        foreach (Transform entryObject in entriesGameObject.transform)
        {
            if(Entries.Count(x => $"{GameObjectSeriesName}_Entry_{x.Key}" == entryObject.gameObject.name) != 1)
            {
                DestroyImmediate(entryObject.gameObject);
            }
        }

        for(var i = 0; i < Entries.Count; i++)
        {
            var Entry = Entries[i];
            var piechartEntry = Resources.Load<GameObject>("Charts/Series/PiechartSeries/Entry");
            var instantiatedEntry = ObjectIterator.GetChildByNameAndLayer($"{GameObjectSeriesName}_Entry_{Entry.Key}", 5, entriesGameObject.transform) ??
                Instantiate(piechartEntry, entriesGameObject.transform);

            instantiatedEntry.name = $"{GameObjectSeriesName}_Entry_{Entry.Key}";

            if (i < Colors.Count)
            {
                var image = ObjectIterator.GetChildByNameAndLayer("EntrySlice", 5, instantiatedEntry.transform)?.GetComponent<Image>();
                image.color = Colors[i];
            }

            var entryMask = ObjectIterator.GetChildByNameAndLayer("EntryMask", 5, instantiatedEntry.transform);
            var entrySlice = ObjectIterator.GetChildByNameAndLayer("EntrySlice", 5, instantiatedEntry.transform);
            var label = ObjectIterator.GetChildByNameAndLayer("Label", 5, instantiatedEntry.transform);

            var seriesAccValue = Entries.Take(i).Select(x => x.Value).Sum() / 100.0f;

            var maskValue = 1 - seriesAccValue;
            var sliceValue = seriesAccValue + (Entry.Value / 100.0f);

            var seriesAccDeg = (seriesAccValue + (Entry.Value * 0.5f / 100.0f)) * 360.0f * 0.01745f;

            entryMask.GetComponent<Image>().fillAmount = maskValue;
            entrySlice.GetComponent<Image>().fillAmount = sliceValue;

            if (Entry.Value > 0)
            {
                label.GetComponent<RectTransform>().localPosition = new Vector3
                (
                    (float)Math.Sin(seriesAccDeg) * 80 * -1,
                    (float)Math.Cos(seriesAccDeg) * 80,
                    0
                );
                label.GetComponent<TMP_Text>().SetText($"{Entry.Value}%");
            }
            else
            {
                label.GetComponent<RectTransform>().localPosition = Vector3.zero;
                label.GetComponent<TMP_Text>().SetText(string.Empty);
            }
        }
    }

    //    protected override string OnValueChanged(SeriesValue seriesValue, float oldValue)
    //    {
    //        Debug.Log("On value changed!");
    //        var entriesGameObject = GetEntriesGameObject();

    //        if (entriesGameObject == null)
    //        {
    //            return null;
    //        }

    //        var entryGameObject = ObjectIterator.GetChildByNameAndLayer($"{GameObjectSeriesName}_{seriesValue.Key}", 5, entriesGameObject.transform);

    //        if (entryGameObject != null)
    //        {
    //            var entryIndex = Entries.FindIndex(x => x.Key == seriesValue.Key);

    //            if (entryIndex < 0) return null;

    //            var entryMask = ObjectIterator.GetChildByNameAndLayer("EntryMask", 5, entryGameObject.transform);
    //            var entrySlice = ObjectIterator.GetChildByNameAndLayer("EntrySlice", 5, entryGameObject.transform);
    //            var label = ObjectIterator.GetChildByNameAndLayer("Label", 5, entryGameObject.transform);

    //            if (entryMask == null && entrySlice == null && label == null) return null;

    //            var seriesAccValue = Entries.Take(entryIndex).Select(x => x.Value).Sum() / 100;

    //            var maskValue = 1 - seriesAccValue;
    //            var sliceValue = seriesAccValue + (seriesValue.Value / 100);

    //            var seriesAccDeg = (seriesAccValue + (seriesValue.Value * 0.5 / 100)) * 360 * 0.01745;

    //            entryMask.GetComponent<Image>().fillAmount = maskValue;
    //            entrySlice.GetComponent<Image>().fillAmount = sliceValue;

    //            if (seriesValue.Value > 0)
    //            {
    //                label.GetComponent<RectTransform>().localPosition = new Vector3
    //                (
    //                    (float)Math.Sin(seriesAccDeg) * 80 * -1,
    //                    (float)Math.Cos(seriesAccDeg) * 80,
    //                    0
    //                );
    //                label.GetComponent<TMP_Text>().SetText($"{seriesValue.Value}%");
    //            }
    //            else
    //            {
    //                label.GetComponent<RectTransform>().localPosition = Vector3.zero;
    //                label.GetComponent<TMP_Text>().SetText(string.Empty);
    //            }

    //        }

    //        return null;

    //    }

    //    public override void RenderEntry(SeriesValue Entry, Transform parent)
    //    {
    //        Debug.Log("Rendered entry");

    //        var piechartEntry = Resources.Load<GameObject>("Charts/Series/PiechartSeries/Entry");
    //        var instantiatedEntry = Instantiate(piechartEntry, parent);
    //        instantiatedEntry.name = $"{GameObjectSeriesName}_{Entry.Key}";

    //        if (Entries?.Count <= Colors.Count)
    //        {
    //            var entrySlice = ObjectIterator.GetChildByNameAndLayer("EntrySlice", 5, instantiatedEntry.transform);
    //            entrySlice.GetComponent<Image>().color = Colors[Entries.Count - 1];
    //        }

    //        //Entry.Key = $"Entry_{Entries.Count}";
    //        //Entry.Value = 0;

    //        OnKeyChanged(Entry, string.Empty);
    //        OnValueChanged(Entry, 0);
    //    }

    //    protected override void OnEntriesChanged(List<SeriesValue> oldValue)
    //    {
    //        var entriesGameObject = GetEntriesGameObject();

    //        if (entriesGameObject == null)
    //        {
    //            return;
    //        }

    //        base.OnEntriesChanged(oldValue);

    //        var newEntries = Entries.Skip(oldValue.Count);

    //        Debug.Log("OnEntriesChanged");
    //        Debug.Log("Old value:");
    //        Debug.Log(string.Join("\n", oldValue.Select(x => $"{x.Key}: {x.Value}")));
    //        Debug.Log("New value:");
    //        Debug.Log(string.Join("\n", Entries.Select(x => $"{x.Key}: {x.Value}")));
    //        Debug.Log("Value to render:");
    //        Debug.Log(string.Join("\n", newEntries.Select(x => $"{x.Key}: {x.Value}")));

    //        foreach (var entry in newEntries)
    //        {
    //            RenderEntry(entry, entriesGameObject.transform);
    //        }
    //    }
}
