using System;
using System.Collections.Generic;

namespace CaptainLib.Collections
{
    public class LimitedQueue<T> : Queue<T>
    {
        private int _capacity;
        private readonly Action<T> _remove;

        public LimitedQueue(int capacity, Action<T> remove = null)
        {
            if (capacity < 1)
                throw new Exception("Capacity <1. Use regular queue.");
            _capacity = capacity;
            _remove = remove ?? (o => { });
        }

        public new void Enqueue(T item)
        {
            while (Count >= _capacity)
                _remove(Dequeue());

            base.Enqueue(item);
        }
    }
}
