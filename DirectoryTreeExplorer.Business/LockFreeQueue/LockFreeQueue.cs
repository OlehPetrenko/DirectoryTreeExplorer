using System;
using System.Collections.Concurrent;
using System.Threading;

namespace DirectoryTreeExplorer.Business.LockFreeQueue
{
    /// <summary>
    /// Represents a lock free FIFO collection. 
    /// Use it to avoid <see cref="ConcurrentQueue{T}"/> in some specific cases.
    /// </summary>
    /// <typeparam name="T">The type of the elements contained in the queue.</typeparam>
    public sealed class LockFreeQueue<T> : IQueue<T> where T : class, new()
    {
        private LockFreeQueueItem<T> _head;
        private LockFreeQueueItem<T> _tail;


        public bool IsEmpty => _head == _tail;

        public LockFreeQueue()
        {
            var firstElement = new LockFreeQueueItem<T>();

            _head = firstElement;
            _tail = firstElement;
        }

        public void Enqueue(T item)
        {
            var newItem = new LockFreeQueueItem<T>(item);

            _tail.Next = newItem;

            Interlocked.Exchange(ref _tail, newItem);
        }

        public T Dequeue()
        {
            if (IsEmpty)
                return null;

            Interlocked.Exchange(ref _head, _head.Next);

            return _head.Data;
        }

        public bool TryDequeue(out T item)
        {
            item = null;

            if (IsEmpty)
                return false;

            Interlocked.Exchange(ref _head, _head.Next);
            Interlocked.Exchange(ref item, _head.Data);

            return true;
        }
    }
}
