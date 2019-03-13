using System;
using System.Collections.Generic;
using System.Linq;

namespace CaptainLib.Collections
{
    public class LimitedList<T> : List<T>
    {

        public LimitedList(int limit)
        :this(limit, null)
        {
        }

        public LimitedList(int limit, Action<T> remove)
            : base()
        {
            _limit = limit;
            _remove = remove ?? (o => { });
        }

        public LimitedList(IEnumerable<T> collection, int limit, Action<T> remove)
            : base(collection)
        {
            _limit = limit;
            _remove = remove ?? (o => { });
        }

        public LimitedList(int capacity, int limit, Action<T> remove)
            : base(capacity)
        {
            _limit = limit;
            _remove = remove ?? (o => { });
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
                _remove(this[0]);
                RemoveAt(0);
                --cnt;
            }
        }

        private int _limit;
        private readonly Action<T> _remove;
    }
}
