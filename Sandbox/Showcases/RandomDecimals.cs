using System;
using System.Collections.Generic;
using System.Text;
using CaptainLib.Numbers;

namespace Sandbox.Showcases
{
    class RandomDecimals : IShowcase
    {
        public void Start()
        {
            var d = SafeRandom.GetDecimal(min: 0);
            Console.WriteLine(d);
            d = SafeRandom.GetDecimal(min: 0);
            Console.WriteLine(d);
            d = SafeRandom.GetDecimal(min: 0);
            Console.WriteLine(d);
            d = SafeRandom.GetDecimal();
            Console.WriteLine(d);
        }
    }
}
