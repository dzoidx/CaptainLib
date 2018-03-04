using System;
using Sandbox;
using CaptainLib.Collections;

namespace CaptainLib.Sandbox.Showcases
{
    class PoolingObjects : IShowcase
    {
        class PoolMe
        {
            public PoolMe()
            {
                Console.WriteLine("New instance of PoolMe");
            }
        }

        public void Start()
        {
            // 10 is max capacity no relevance to this test
            var pool = new ObjectPool<PoolMe>(10, () => new PoolMe());
            Console.WriteLine("Getting 1 from a pool");
            using (var obj1 = pool.Alloc())
            {
                Console.WriteLine("Getting 2 from a pool");
                using (var obj2 = pool.Alloc())
                {
                }
                Console.WriteLine("Returning 2 to a pool");

                Console.WriteLine("Getting 3 from a pool");
                using (var obj3 = pool.Alloc())
                {
                }
                Console.WriteLine("Returning 3 to a pool");
            }
            Console.WriteLine("Returning 1 to a pool");
        }
    }
}
