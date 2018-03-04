using System;
using System.Text;
using System.Threading;
using CaptainLib.Collections;

namespace CaptainLib.Numbers
{
    /// <summary>
    /// The problem:
    /// - https://blogs.msdn.microsoft.com/pfxteam/2009/02/19/getting-random-numbers-in-a-thread-safe-way/
    /// </summary>
    public class SafeRandom
    {
#if NET45 || NETSTANDARD1_0
        private static ThreadLocal<System.Random> _rnd = new ThreadLocal<System.Random>(() => new System.Random());
#else
        private static Random _rnd = new Random();
        private static object _sync = new object();
#endif
        private static readonly string _maxDecimal;

        static SafeRandom()
        {
            _maxDecimal = decimal.MaxValue.ToString();
        }

        public static int GetInt()
        {
#if NET45 || NETSTANDARD1_0
            return _rnd.Value.Next();
#else
            lock (_sync)
                return _rnd.Next();
#endif
        }

        public static int GetInt(int max)
        {
#if NET45 || NETSTANDARD1_0
            return _rnd.Value.Next(max);
#else
            lock (_sync)
                return _rnd.Next(max);
#endif
            
        }

        public static int GetInt(int min, int max)
        {
#if NET45 || NETSTANDARD1_0
            return _rnd.Value.Next(min, max);
#else
            lock (_sync)
                return _rnd.Next(min, max);
#endif
        }

        public static bool Test()
        {
            return RollTheDice(0.5f);
        }

        public static bool RollTheDice(float successChance)
        {
            if (successChance < 0.0001f)
                return false;
            if (successChance > 0.9999f)
                return true;

            return GetDouble() <= successChance;
        }

        public static float GetFloat()
        {
#if NET45 || NETSTANDARD1_0
            return (float)_rnd.Value.NextDouble();
#else
            lock (_sync)
                return (float)_rnd.NextDouble();
#endif
        }

        public static void RandomBytes(byte[] buffer)
        {
#if NET45 || NETSTANDARD1_0
            _rnd.Value.NextBytes(buffer);
#else
            lock (_sync)
                _rnd.NextBytes(buffer);
#endif
        }

        public static decimal GetDecimal(decimal min, decimal max)
        {
            if (min >= max)
                throw new InvalidOperationException($"{min} > {max}");

            string resultStr = null;
            using (var buffCtx = StaticObjectPool<byte>.Instance.AllocArray(_maxDecimal.Length))
            {
                byte[] buff = buffCtx;
                RandomBytes(buff);
                for (var i = 0; i < buff.Length; ++i)
                {
                    buff[i] = (byte)(buff[i] % 10);
                }
                var strB = new StringBuilder(_maxDecimal.Length);
                var idx = _maxDecimal[0] < (char)(0x30 | buff[0]) ? 1 : 0;
                for (var i = idx; i < buff.Length; ++i)
                {
                    strB.Append((char)(0x30 | buff[i]));
                }
                resultStr = strB.ToString();
            }
            var result = decimal.Parse(resultStr);
            return result % max + min;
        }

        public static double GetDouble()
        {
#if NET45 || NETSTANDARD1_0
            return _rnd.Value.NextDouble();
#else
            lock (_sync)
                return _rnd.NextDouble();
#endif
        }
    }
}
