using System;

namespace CaptainLib.Common
{
    public class ScopedObject<T> : IDisposable
    {
        private bool _disposed;
        private T _obj;
        Action<T> _removeObject;

        public T Obj
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("");
                return _obj;
            }
        }

        public ScopedObject(T obj, Action<T> removeObject)
        {
            _removeObject = removeObject;
        }

        public ScopedObject(Func<T> createObject, Action<T> removeObject)
            :this(createObject(), removeObject)
        {
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            if (_removeObject != null)
                _removeObject(_obj);
            _disposed = true;
        }
    }
}
