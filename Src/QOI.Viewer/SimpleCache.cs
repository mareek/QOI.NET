using System;
using System.Collections.Generic;
using System.Linq;

namespace QOI.Viewer;

internal class SimpleCache<T>(int size)
{
    private readonly int _size = size;
    private readonly Dictionary<string, (int order, T value)> _cache = [];

    private int _order = 0;

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
