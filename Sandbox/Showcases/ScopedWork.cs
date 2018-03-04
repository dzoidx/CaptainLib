using System;
using System.Threading;
using Sandbox;
using CaptainLib.Threading;

namespace CaptainLib.Sandbox.Showcases
{
    class SomeWork : ScopeWorker
    {
        private int _iteration;

        protected override void Update(float dt, CancellationToken cancellation)
        {
            Console.WriteLine($"{_iteration++}: {dt}");
        }
    }

    class ScopedWork : IShowcase
    {
        public void Start()
        {
            Console.WriteLine("Entering the scope");
            using (new SomeWork())
            {
                Thread.Sleep(5);
            }
            Console.WriteLine("Out of a scope");
        }
    }
}
