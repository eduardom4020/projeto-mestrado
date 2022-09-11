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


    private string _Key;
    public string Key
    {
        get { return _Key; }
        set
        {
            //Debug.Log("KEY CHANGED?");
            //Debug.Log(_Key);
            //Debug.Log(value);
            //Debug.Log(value == (_Key ?? string.Empty));
            if ((_Key ?? string.Empty) == value) return;

            var oldVaue = _Key;
            _Key = value;

            OnKeyChanged(oldVaue);
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

            OnValueChanged(oldVaue);
        }
    }

    protected void OnKeyChanged(string oldValue)
    {
        Debug.Log("OnKeyChanged");
        Debug.Log("Current value ");
        Debug.Log(Key);
        Debug.Log("Old value");
        Debug.Log(oldValue);
    }

    protected void OnValueChanged(float oldValue)
    {
        Debug.Log("OnValueChanged");
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

            var oldVaue = _Entries;
            _Entries = value;

            OnEntriesChanged(oldVaue);
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

    protected string GameObjectSeriesName
    {
        get { return $"Series_{Name.Trim()}"; }
    }

    public abstract void Render(Transform parent);

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
        Debug.Log("OnNameChanged");
    }
    protected void OnEntriesChanged(List<SeriesValue> oldVaue)
    {
        Debug.Log("OnEntriesChanged");
    }

    public void AddEntry(SeriesValue Entry)
    {
        var oldValue = new List<SeriesValue>(Entries);
        Entries.Add(Entry);

        OnEntriesChanged(oldValue);
    }

    public void AddEntries(List<SeriesValue> NewEntries)
    {
        var oldValue = new List<SeriesValue>(Entries);
        Entries.AddRange(NewEntries);

        OnEntriesChanged(oldValue);
    }

    public void RemoveEntries(int index, int count)
    {
        var oldValue = new List<SeriesValue>(Entries);
        Entries.RemoveRange(index, count);

        OnEntriesChanged(oldValue);
    }
}
