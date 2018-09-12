using System;
using System.Linq;

namespace CaptainLib.Collections
{
    public class ObjectPoolContext<T> : IDisposable
    {
        ObjectPool<T> _pool;
        T _value;

        public static implicit operator T(ObjectPoolContext<T> ctx)
        {
            return ctx._value;
        }

        public ObjectPoolContext(T value, ObjectPool<T> pool)
        {
            _value = value;
            _pool = pool;
        }

        public void Dispose()
        {
            _pool.Free(_value);
        }
    }

    public class ArrayObjectPoolContext<T> : IDisposable
    {
        ObjectPool<T> _pool;
        T[] _value;

        public static implicit operator T[](ArrayObjectPoolContext<T> ctx)
        {
            return ctx._value;
        }

        public ArrayObjectPoolContext(T[] value, ObjectPool<T> pool)
        {
            _value = value;
            _pool = pool;
        }

        public void Dispose()
        {
            _pool.FreeArray(_value);
        }
    }

    public class ObjectPool<T>
    {
        LimitedQueue<T> _pool;
        LimitedList<T[]> _arrayPool;
        Func<T> _create;
        Func<int, T[]> _createArray;

        public ObjectPool(int poolMaxCapacity, Func<T> create, Func<int, T[]> createArray = null, Action<T> remove = null)
        {
            _create = create;
            remove = remove ?? (o => { });
            var remove_array = new Action<T[]>(arr =>
            {
                foreach (var a in arr) remove(a);
            });
            _createArray = createArray ?? ((s) => new T[s]);
            _pool = new LimitedQueue<T>(poolMaxCapacity, remove);
            _arrayPool = new LimitedList<T[]>(poolMaxCapacity, remove_array);
        }

        public ArrayObjectPoolContext<T> AllocArray(int size, bool arraySizeMatch = true)
        {
            T[] result = null;
            if (arraySizeMatch)
                result = _arrayPool.Where(a => a.Length == size).FirstOrDefault();
            else
                result = _arrayPool.Where(a => a.Length >= size).OrderBy(a => a.Length).FirstOrDefault();

            if (result == null)
                result = _createArray(size);

            return new ArrayObjectPoolContext<T>(result, this);
        }

        public void FreeArray(T[] array)
        {
            _arrayPool.Add(array);
        }

        public ObjectPoolContext<T> Alloc()
        {
            if (_pool.Any())
                return new ObjectPoolContext<T>(_pool.Dequeue(), this);
            return new ObjectPoolContext<T>(_create(), this);
        }

        public void Free(T obj)
        {
            _pool.Enqueue(obj);
        }
    }

    public static class StaticObjectPool<T> where T: new()
    {
        private static ObjectPool<T> _instance = new ObjectPool<T>(100, () => new T());

        public static ObjectPool<T> Instance => _instance;
    }
}
