﻿namespace CaptainLib.Numbers
{
    public class Range<T>
    {
        public T From { get; private set; }
        public T To { get; private set; }

        public Range(T from, T to)
        {
            From = from;
            To = to;
        }
    }
}
