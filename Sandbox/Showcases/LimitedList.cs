using System;
using System.Linq;
using Sandbox;

namespace CaptainLib.Sandbox.Showcases
{
    class LimitedList : IShowcase
    {
        public void Start()
        {
            var list = new Collections.LimitedList<int>(10);
            foreach (var n in Enumerable.Range(0, 50))
            {
                list.Add(n);
            }

            Console.Write("Limited list content: ");
            foreach (var n in list)
            {
                Console.Write($"{n} ");
            }
        }
    }
}
