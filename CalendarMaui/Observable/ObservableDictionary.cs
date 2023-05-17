using System.Collections.ObjectModel;

namespace CalendarMaui.Observable;

public class ObservableDictionary<TKey, TValue> : ObservableCollection<KeyValuePair<TKey, TValue>>
{
    public void Add(TKey key, TValue value)
    {
        Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public bool Remove(TKey key)
    {
        var item = this.FirstOrDefault(i => i.Key.Equals(key));
        return item.Key != null && Remove(item);
    }
    
    public bool ContainsKey(TKey key)
    {
        return this.Any(item => item.Key.Equals(key));
    }
    
    public void Clear()
    {
        ClearItems();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        var item = this.FirstOrDefault(i => i.Key.Equals(key));
        if (item.Key != null)
        {
            value = item.Value;
            return true;
        }
        value = default(TValue);
        return false;
    }

    public TValue this[TKey key]
    {
        get
        {
            var item = this.FirstOrDefault(i => i.Key.Equals(key));
            return item.Value;
        }
        set
        {
            var item = this.FirstOrDefault(i => i.Key.Equals(key));
            if (item.Key != null)
            {
                var index = IndexOf(item);
                this[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else
            {
                Add(new KeyValuePair<TKey, TValue>(key, value));
            }
        }
    }
}