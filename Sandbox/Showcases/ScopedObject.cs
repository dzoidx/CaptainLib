using System;
using Sandbox;

namespace CaptainLib.Sandbox.Showcases
{
    class ScopedObject : IShowcase
    {

        public void Start()
        {
            Func<int> createFunc = () => { Console.WriteLine("Object created"); return 0; };
            Action<int> removeFunc = (o) => Console.WriteLine($"Object removed. Value '{o}'");
            Console.WriteLine("Entering the object scope");
            using (new Common.ScopedObject<int>(createFunc, removeFunc))
            {
                Console.WriteLine("Inside an object scope");
            }
            Console.WriteLine("Out of an object scope");
        }
    }
}
