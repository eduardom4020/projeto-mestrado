using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Entry<T> : IStateObservable
{
    public string Key;
    public T Value;

    public Func<T, T, bool> ValueComparer { get; set; } = null;

    public IStateObservable GenerateCopy() => new Entry<T>
    {
        Key = Key,
        Value = Value
    };

    public bool IsEqual(IStateObservable other)
    {
        var Other = other as Entry<T>;

        if(ValueComparer != null)
        {
            return Key == Other.Key && ValueComparer(Value, Other.Value);
        }

        throw new Exception("Provide a ValueComparer function");
    }
}

[Serializable]
public class SeriesProperties : IStateObservable
{
    public int Index;
    public string Name = string.Empty;

    public IStateObservable GenerateCopy() => new SeriesProperties
    {
        Index = Index,
        Name = Name
    };

    public bool IsEqual(IStateObservable other)
    {
        var Other = other as SeriesProperties;

        return (
            Index == Other.Index &&
            Name == Other.Name
        );
    }
}

public class StateChangeEntriesWatcher<T>
{
    private List<Entry<T>> previousState;
    private List<Entry<T>> currentState;
    public StateChangeEntriesWatcher(List<Entry<T>> initialState)
    {
        previousState = initialState?.Select(x => x.GenerateCopy() as Entry<T>)?.ToList();
        currentState = initialState;
    }

    // Put this in every Update

    public bool Changed
    {
        get
        {
            if (currentState?.Count != previousState?.Count)
            {
                previousState = currentState?.Select(x => x.GenerateCopy() as Entry<T>)?.ToList();
                return true;
            }

            for(var i = 0; i < (currentState?.Count ?? 0); i++)
            {
                if(!currentState[i].IsEqual(previousState[i]))
                {
                    previousState = currentState?.Select(x => x.GenerateCopy() as Entry<T>)?.ToList();
                    return true;
                }
            }

            previousState = currentState?.Select(x => x.GenerateCopy() as Entry<T>)?.ToList();
            return false;
        }
    }
}

[ExecuteAlways]
public abstract class Series<T> : MonoBehaviour
{
    protected List<Color32> Colors = new List<Color32>
    {
        new Color32(49, 86, 230, 255),
        new Color32(255, 61, 253, 255),
        new Color32(24, 66, 137, 255),
        new Color32(165, 64, 217, 255)
    };

public SeriesProperties Properties;
    protected StateChangeWatcher PropertiesWatcher;

    public List<Entry<T>> Entries;
    protected StateChangeEntriesWatcher<T> EntriesWatcher;

    public void SetSeriesIndex()
    {
        Properties.Index = transform.GetComponents<Series<T>>().Length;
    }

    public int GetIndex() => Properties.Index;

    protected void Start()
    {
        if(Properties.Index <= 0)
        {
            SetSeriesIndex();
        }
        
        Entries ??= new List<Entry<T>>();
        SetEntriesValueComparers();

        PropertiesWatcher = new StateChangeWatcher(Properties);
        EntriesWatcher = new StateChangeEntriesWatcher<T>(Entries);
    }

    protected abstract void SetEntriesValueComparers();

    public abstract void Render();

    protected void Update()
    {
        if(Entries == null)
        {
            Entries = new List<Entry<T>>();
        }

        SetEntriesValueComparers();

        if (PropertiesWatcher == null)
        {
            PropertiesWatcher = new StateChangeWatcher(Properties);
        }

        if (EntriesWatcher == null)
        {
            EntriesWatcher = new StateChangeEntriesWatcher<T>(Entries);
        }

        if(PropertiesWatcher.Changed || EntriesWatcher.Changed)
        {
            Render();
        }
    }


    public string GameObjectSeriesName
    {
        get { return $"Series_Index_{Properties.Index}"; }
    }

    //protected GameObject GetEntriesGameObject()
    //{
    //    var seriesGameObject = ObjectIterator.GetChildByNameAndLayer(GameObjectSeriesName, 5, transform);

    //    if (seriesGameObject == null)
    //    {
    //        return null;
    //    }

    //    var entriesGameObject = ObjectIterator.GetChildByNameAndLayer("Entries", 5, seriesGameObject.transform);

    //    return entriesGameObject;
    //}

    //protected void OnNameChanged(string oldVaue)
    //{
    //    var instance = ObjectIterator.GetChildByNameAndLayer($"Series_{oldVaue}", 5, transform);

    //    if (instance != null)
    //    {
    //        instance.name = $"Series_{Name}";
    //    }
    //}
    //protected virtual void OnEntriesChanged(List<SeriesValue> oldValue)
    //{
    //    var entriesToRemove = oldValue.Skip(Entries.Count).Take(oldValue.Count - Entries.Count);
    //    foreach (var entryToRemove in entriesToRemove)
    //    {
    //        DestroyImmediate
    //        (
    //            ObjectIterator.GetChildByNameAndLayer($"{GameObjectSeriesName}_{entryToRemove.Key}", 5, transform)
    //        );
    //    }

    //    foreach (var entry in Entries)
    //    {
    //        entry.OnKeyChanged ??= OnKeyChanged;
    //        entry.OnValueChanged ??= OnValueChanged;
    //    }
    //}

    //protected abstract string OnKeyChanged(SeriesValue seriesValue, string oldValue);

    //protected abstract string OnValueChanged(SeriesValue seriesValue, float oldValue);

    //public void AddEntry(SeriesValue Entry)
    //{
    //    var oldValue = new List<SeriesValue>(Entries);
    //    Entries.Add(Entry);

    //    OnEntriesChanged(oldValue);
    //}

    //public void AddEntries(List<SeriesValue> NewEntries)
    //{
    //    var oldValue = new List<SeriesValue>(Entries);
    //    Entries.AddRange(NewEntries);

    //    OnEntriesChanged(oldValue);
    //}

    //public void RemoveEntries(int index, int count)
    //{
    //    var oldValue = new List<SeriesValue>(Entries);
    //    Entries.RemoveRange(index, count);

    //    OnEntriesChanged(oldValue);
    //}

    //public abstract void Render(Transform parent);
    //public void Destroy(Transform parent)
    //{
    //    DestroyImmediate(ObjectIterator.GetChildByNameAndLayer(GameObjectSeriesName, 5, parent));
    //}

    //public abstract void RenderEntry(SeriesValue Entry, Transform parent);
}
