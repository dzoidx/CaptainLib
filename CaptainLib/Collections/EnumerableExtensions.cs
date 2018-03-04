using System.Collections.Generic;
using System.Linq;
using  CaptainLib.Numbers;

namespace CaptainLib.Collections
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Randomly changes order of elements in the sequence of elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> e)
        {
            return e.ToList().RandomizeMutable();
        }

        /// <summary>
        /// Same as Randomize, but changes the original List's elements order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IEnumerable<T> RandomizeMutable<T>(this IList<T> e)
        {
            var n = e.Count;
            while (n > 1)
            {
                n--;
                var k = SafeRandom.GetInt(n + 1);
                T value = e[k];
                e[k] = e[n];
                e[n] = value;
            }

            return e;
        }
    }
}
