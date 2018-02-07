using System;
using System.Collections.Generic;
using System.Linq;

namespace CaptainLib.Collections
{
    public class LimitedList<T> : List<T>
    {

        public LimitedList(int limit)
            : base()
        {
            _limit = limit;
        }

        public LimitedList(IEnumerable<T> collection, int limit)
            : base(collection)
        {
            _limit = limit;
        }

        public LimitedList(int capacity, int limit)
            : base(capacity)
        {
            _limit = limit;
        }


        public new void Add(T item)
        {
            FreeSpaceFor(1);
            base.Add(item);
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            FreeSpaceFor(collection.Count());
            base.AddRange(collection);
        }

        private void FreeSpaceFor(int amount)
        {
            if (amount > _limit)
                throw new IndexOutOfRangeException();

            var cnt = Count + amount;
            while (cnt > _limit)
            {
                RemoveAt(0);
                --cnt;
            }
        }

        private int _limit;

    }
}
