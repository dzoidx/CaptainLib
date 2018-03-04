using System;
using CaptainLib.Numbers;

namespace Sandbox.Showcases
{
    class RandomDecimals : IShowcase
    {
        public void Start()
        {
            var d = SafeRandom.GetDecimal(0, decimal.MaxValue);
            Console.WriteLine(d);
            d = SafeRandom.GetDecimal(0, decimal.MaxValue);
            Console.WriteLine(d);
            d = SafeRandom.GetDecimal(0, decimal.MaxValue);
            Console.WriteLine(d);
            d = SafeRandom.GetDecimal(decimal.MinValue, decimal.MaxValue);
            Console.WriteLine(d);
        }
    }
}
