using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SeriesValue
{
    public static bool operator ==(SeriesValue seriesValueA, SeriesValue seriesValueB)
    {
        if (seriesValueA is null && seriesValueA is null)
        {
            return true;
        }

        if (!(seriesValueA is null) && seriesValueA is null)
        {
            return false;
        }

        return seriesValueA.Key == seriesValueB.Key && seriesValueA.Value == seriesValueB.Value;
    }

    public static bool operator !=(SeriesValue seriesValueA, SeriesValue seriesValueB)
    {
        if (seriesValueA is null && seriesValueA is null)
        {
            return false;
        }

        if (!(seriesValueA is null) && seriesValueA is null)
        {
            return true;
        }

        return seriesValueA.Key != seriesValueB.Key || seriesValueA.Value != seriesValueB.Value;
    }

    public Func<SeriesValue, string, string> OnKeyChanged { get; set; } = null;
    public Func<SeriesValue, float, string> OnValueChanged { get; set; } = null;

    private string _Key;
    public string Key
    {
        get { return _Key; }
        set
        {
            if ((_Key ?? string.Empty) == value) return;

            var oldVaue = _Key;
            _Key = value;

            if(OnKeyChanged != null)
            {
                OnKeyChanged(this, oldVaue);
            }
        }
    }

    private float _Value;
    public float Value
    {
        get { return _Value; }
        set
        {
            if (_Value == value) return;

            var oldVaue = _Value;
            _Value = value;

            if(OnValueChanged != null)
            {
                OnValueChanged(this, oldVaue);
            }
        }
    }

    public override bool Equals(object obj)
    {
        return obj is SeriesValue value &&
               Key == value.Key &&
               Value == value.Value;
    }

    public override int GetHashCode()
    {
        int hashCode = 206514262;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Key);
        hashCode = hashCode * -1521134295 + Value.GetHashCode();
        return hashCode;
    }
}

public abstract class Series : MonoBehaviour
{
    public int? SeriesIndex { get; set; } = null;

    public static bool operator ==(Series seriesA, Series seriesB)
    {
        if (seriesA is null && seriesB is null)
        {
            return true;
        }

        if (!(seriesA is null) && seriesB is null)
        {
            return false;
        }

        if (seriesA.Entries?.Count == seriesB.Entries?.Count)
        {
            bool noEntriesChanged = true;

            for (var i = 0; i < (seriesA.Entries?.Count ?? 0); i++)
            {
                if (seriesA.Entries[i] != seriesB.Entries[i])
                {
                    noEntriesChanged = false;
                }
            }

            var nameEquals = seriesA.Name == seriesB.Name;

            return nameEquals && noEntriesChanged;
        }
        else
        {
            return false;
        }
    }

    public static bool operator !=(Series seriesA, Series seriesB)
    {
        if (seriesA is null && seriesB is null)
        {
            return false;
        }

        if (!(seriesA is null) && seriesB is null)
        {
            return true;
        }

        if
        (
            seriesA.Name != seriesB.Name ||
            seriesA.Entries?.Count != seriesB.Entries?.Count
        )
        {
            return true;
        }
        else if (seriesA.Entries?.Count == seriesB.Entries?.Count)
        {
            for (var i = 0; i < (seriesA.Entries?.Count ?? 0); i++)
            {
                if (seriesA.Entries[i] != seriesB.Entries[i])
                {
                    return true;
                }
            }
        }

        return false;
    }

    private string _Name;
    public string Name
    {
        get { return _Name; }
        set
        {
            if (_Name == value) return;

            var oldVaue = _Name;
            _Name = value;

            OnNameChanged(oldVaue);
        }
    }

    private List<SeriesValue> _Entries;
    public List<SeriesValue> Entries
    {
        get { return _Entries; }
        set
        {
            if (_Entries?.Count == value?.Count)
            {
                bool noValuesChanged = true;

                for(var i=0; i < (_Entries?.Count ?? 0); i++)
                {
                    if(_Entries[i] != value[i])
                    {
                        noValuesChanged = false;
                    }
                }

                if (noValuesChanged)
                {
                    return;
                }
            }

            if(_Entries == null)
            {
                _Entries = new List<SeriesValue>();
            }

            var oldValue = new List<SeriesValue>(_Entries);

            if (value.Count >= _Entries.Count)
            {
                var updatedEntries = value
                .Take(_Entries.Count)
                .ToList();

                for (int i = 0; i < updatedEntries.Count; i++)
                {
                    _Entries[i].Key = updatedEntries[i].Key;
                    _Entries[i].Value = updatedEntries[i].Value;
                }

                var newEntries = value
                    .Skip(_Entries.Count)
                    .Select(x => new SeriesValue { Key = x.Key, Value = x.Value })
                    .ToList();

                if (newEntries.Count > 0)
                {
                    _Entries.AddRange(newEntries);
                }
            }
            else
            {
                _Entries.RemoveRange(value.Count, _Entries.Count - value.Count);
            }

            OnEntriesChanged(oldValue);
        }
    }

    public Series(string Name, List<SeriesValue> Entries) => Init(Name, Entries);

    public Series(string Name) => Init(Name);

    public void Init(string Name, List<SeriesValue> Entries)
    {
        this.Name = Name;
        this.Entries = Entries;
    }

    public void Init(string Name)
    {
        this.Name = Name;
        Entries = new List<SeriesValue>();
    }

    public string GameObjectSeriesName
    {
        get { return $"Series_{Name.Trim()}"; }
    }

    protected GameObject GetEntriesGameObject()
    {
        var seriesGameObject = ObjectIterator.GetChildByNameAndLayer(GameObjectSeriesName, 5, transform);

        if (seriesGameObject == null)
        {
            return null;
        }

        var entriesGameObject = ObjectIterator.GetChildByNameAndLayer("Entries", 5, seriesGameObject.transform);

        return entriesGameObject;
    }

    public override bool Equals(object obj)
    {
        return obj is Series series &&
               base.Equals(obj) &&
               Name == series.Name &&
               EqualityComparer<List<SeriesValue>>.Default.Equals(Entries, series.Entries);
    }

    public override int GetHashCode()
    {
        int hashCode = 377656463;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
        hashCode = hashCode * -1521134295 + EqualityComparer<List<SeriesValue>>.Default.GetHashCode(Entries);
        return hashCode;
    }

    protected void OnNameChanged(string oldVaue)
    {
        var instance = ObjectIterator.GetChildByNameAndLayer($"Series_{oldVaue}", 5, transform);

        if(instance != null)
        {
            instance.name = $"Series_{Name}";
        }
    }
    protected virtual void OnEntriesChanged(List<SeriesValue> oldValue)
    {
        var entriesToRemove = oldValue.Skip(Entries.Count).Take(oldValue.Count - Entries.Count);
        foreach (var entryToRemove in entriesToRemove)
        {
            DestroyImmediate
            (
                ObjectIterator.GetChildByNameAndLayer($"{GameObjectSeriesName}_{entryToRemove.Key}", 5, transform)
            );
        }

        foreach (var entry in Entries)
        {
            entry.OnKeyChanged ??= OnKeyChanged;
            entry.OnValueChanged ??= OnValueChanged;
        }

        Debug.Log("OnEntriesChanged");
    }

    protected abstract string OnKeyChanged(SeriesValue seriesValue, string oldValue);

    protected abstract string OnValueChanged(SeriesValue seriesValue, float oldValue);

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

    public abstract void Render(Transform parent);
    public void Destroy(Transform parent)
    {
        DestroyImmediate(ObjectIterator.GetChildByNameAndLayer(GameObjectSeriesName, 5, parent));
    }

    public abstract void RenderEntry(SeriesValue Entry, Transform parent);
}
