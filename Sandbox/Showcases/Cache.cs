using System;
using System.Threading;
using Sandbox;
using CaptainLib.Collections;

namespace CaptainLib.Sandbox.Showcases
{
    class Cache : IShowcase
    {
        class CacheMe : IDisposable
        {
            private readonly int _key;

            public CacheMe(int key)
            {
                _key = key;
                Console.WriteLine($"New instance of CacheMe. Key {_key}");
            }

            public void Dispose()
            {
                Console.WriteLine($"CacheMe disposed. Key {_key}");
            }
        }

        public void Start()
        {
            var cache = new MemCache<int, CacheMe>();

            Console.WriteLine("Getting non-cached object two times");
            cache.GetOrCreate(0, () => new CacheMe(0), TimeSpan.Zero);
            cache.GetOrCreate(0, () => new CacheMe(0), TimeSpan.Zero);
            Console.WriteLine("Creating cached object");
            cache.GetOrCreate(1, () => new CacheMe(1), TimeSpan.FromMilliseconds(100));
            Console.WriteLine("Getting cached object two times");
            cache.GetOrCreate(1, () => new CacheMe(1), TimeSpan.FromMilliseconds(100));
            cache.GetOrCreate(1, () => new CacheMe(1), TimeSpan.FromMilliseconds(100));
            Console.WriteLine("Wait cache to expire");
            Thread.Sleep(100);
            cache.GetOrCreate(1, () => new CacheMe(1), TimeSpan.FromMilliseconds(100));
        }
    }
}
