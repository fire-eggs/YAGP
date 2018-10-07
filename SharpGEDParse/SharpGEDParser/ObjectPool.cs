using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SharpGEDParser
{
    public class ObjectPool<T>
    {
        private ConcurrentBag<T> _objects;
        private Func<T> _objectGenerator;

        public ObjectPool(Func<T> objectGenerator)
        {
            if (objectGenerator == null)
                throw new ArgumentNullException("objectGenerator");
            _objects = new ConcurrentBag<T>();
            _objectGenerator = objectGenerator;
        }

        public T GetObject()
        {
            T item;
            if (_objects.TryTake(out item)) return item;
            return _objectGenerator();
        }

        public void PutObject(T item)
        {
            _objects.Add(item);
        }
    }

    public class DisposableValue<T> : IDisposable
    {
        private readonly Action _dispose;

        public DisposableValue(T value, Action dispose)
        {
            _dispose = dispose;
            Value = value;
        }

        public T Value { get; private set; }

        public void Dispose()
        {
            _dispose();
        }

        public static DisposableValue<T> Create(T value, Action dispose)
        {
            return new DisposableValue<T>(value, dispose);
        }
    }

    public class ArrayPool<T>
    {
        private readonly Dictionary<int, Stack<T[]>> _pool = new Dictionary<int, Stack<T[]>>();

        public readonly T[] Empty = new T[0];

        public DisposableValue<T[]> AllocateDisposable(int size)
        {
            var array = Allocate(size);
            return DisposableValue<T[]>.Create(array, () => Free(array));
        }

        public DisposableValue<T[]> Resize(DisposableValue<T[]> source, int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException("size", "Must be positive.");

            var dest = AllocateDisposable(size);
            Array.Copy(source.Value, dest.Value, size < source.Value.Length ? size : source.Value.Length);
            source.Dispose();
            return dest;
        }

        public virtual void Clear()
        {
            _pool.Clear();
        }

        internal virtual T[] Allocate(int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException("size", "Must be positive.");

            if (size == 0) return Empty;

            Stack<T[]> candidates;
            return _pool.TryGetValue(size, out candidates) && candidates.Count > 0 ? candidates.Pop() : new T[size];
        }

        internal virtual void Free(T[] array)
        {
            if (array == null) throw new ArgumentNullException("array");

            if (array.Length == 0) return;

            Stack<T[]> candidates;
            if (!_pool.TryGetValue(array.Length, out candidates))
                _pool.Add(array.Length, candidates = new Stack<T[]>());
            candidates.Push(array);
        }
    }

    public class ConcurrentArrayPool<T> : ArrayPool<T>
    {
        internal override T[] Allocate(int size)
        {
            lock (this)
            {
                return base.Allocate(size);
            }
        }

        internal override void Free(T[] array)
        {
            lock (this)
            {
                base.Free(array);
            }
        }

        public override void Clear()
        {
            lock (this)
            {
                base.Clear();
            }
        }

    }
}
