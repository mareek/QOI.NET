using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QOI.Viewer;

internal class SimpleCache<T>
{
    private readonly int _size;
    private readonly Dictionary<string, (int order, T value)> _cache = new();

    private int _order;

    public SimpleCache(int size)
    {
        _size = size;
        _order = 0;
    }

    public T GetOrAdd(string key, Func<string, T> valueFactory)
    {
        lock (_cache)
        {
            if (_cache.TryGetValue(key, out var result))
                return result.value;

            if (_cache.Count == _size)
            {
                var oldestEntry = _cache.First(kvp => kvp.Value.order <= (_order - _size));
                _cache.Remove(oldestEntry.Key);
            }

            var newValue = valueFactory(key);
            _cache[key] = (_order++, newValue);
            return newValue;
        }
    }
}
