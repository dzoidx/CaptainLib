using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CaptainLib.Collections;

namespace CaptainLib.Numbers
{
    public class SafeRandom
    {
        private static ThreadLocal<System.Random> _rnd = new ThreadLocal<System.Random>(() => new System.Random());
        private static readonly string _maxDecimal;

        static SafeRandom()
        {
            _maxDecimal = decimal.MaxValue.ToString();
        }

        public static int GetInt()
        {
            return _rnd.Value.Next();
        }

        public static int GetInt(int max)
        {
            return _rnd.Value.Next(max);
        }

        public static int GetInt(int min, int max)
        {
            return _rnd.Value.Next(min, max);
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
            return (float)_rnd.Value.NextDouble();
        }

        public static void RandomBytes(byte[] buffer)
        {
            _rnd.Value.NextBytes(buffer);
        }

        public static decimal GetDecimal(decimal min = decimal.MinValue, decimal max = decimal.MaxValue)
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
            return _rnd.Value.NextDouble();
        }
    }
}
