using System;
using System.Collections.Generic;
using System.Linq;

namespace CaptainLib.Collections
{
    public class MemCache<Key, Value>
    {
        private class MemCacheEntry
        {
            public DateTime ExpirationDate;
            public Value Data;
        }

        public Value GetOrCreate(Key k, Func<Value> creator, DateTime expDate)
        {
            Update();
            if (_cache.TryGetValue(k, out MemCacheEntry entry))
                return entry.Data;

            entry = new MemCacheEntry() { Data = creator(), ExpirationDate = expDate };
            _cache[k] = entry;

            return entry.Data;
        }

        public Value GetOrCreate(Key k, Func<Value> creator, TimeSpan? lifeTime = null)
        {
            DateTime expDate;
            if (!lifeTime.HasValue)
                expDate = DateTime.MaxValue;
            else
                expDate = DateTime.UtcNow + lifeTime.Value;

            return GetOrCreate(k, creator, expDate);
        }

        public bool Invalidate(Key k)
        {
            if (_cache[k].Data is IDisposable)
                (_cache[k].Data as IDisposable).Dispose();
            return _cache.Remove(k);
        }

        private void Update()
        {
            var expKeys = _cache.Where(pair => DateTime.UtcNow > pair.Value.ExpirationDate)
                .Select(pair => pair.Key)
                .ToArray();
            foreach (var expKey in expKeys)
            {
                if(_cache[expKey].Data is IDisposable)
                    (_cache[expKey].Data as IDisposable).Dispose();
                _cache.Remove(expKey);
            }
        }

        private Dictionary<Key, MemCacheEntry> _cache = new Dictionary<Key, MemCacheEntry>();
    }
}
