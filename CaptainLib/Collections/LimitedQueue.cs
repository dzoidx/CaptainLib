using System;
using System.Collections.Generic;
using System.Text;

namespace CaptainLib.Collections
{
    public class LimitedQueue<T> : Queue<T>
    {
        private int _capacity;
        public LimitedQueue(int capacity)
        {
            if (capacity < 1)
                throw new Exception("Capacity <1. Use regular queue.");
            _capacity = capacity;
        }

        public new void Enqueue(T item)
        {
            while (Count >= _capacity)
                Dequeue();

            base.Enqueue(item);
        }
    }
}
