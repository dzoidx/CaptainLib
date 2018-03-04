using System;
using System.Linq;
using Sandbox;
using CaptainLib.Collections;

namespace CaptainLib.Sandbox.Showcases
{
    class ShuffleList : IShowcase
    {
        public void Start()
        {
            var list = Enumerable.Range(0, 10);

            Console.Write("Original list: ");
            foreach (var n in list)
            {
                Console.Write($"{n} ");
            }
            Console.WriteLine();

            var shuffledList = list.Randomize();
            Console.Write("Shuffled list: ");
            foreach (var n in shuffledList)
            {
                Console.Write($"{n} ");
            }
            Console.WriteLine();
        }
    }
}
