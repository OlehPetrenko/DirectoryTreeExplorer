using System;

namespace DirectoryTreeExplorer.Business.LockFreeQueue
{
    /// <summary>
    /// Defines methods to manipulate FIFO collection.
    /// </summary>
    /// <typeparam name="T">The type of the elements contained in the queue.</typeparam>
    public interface IQueue<T>
    {
        bool IsEmpty { get; }

        void Enqueue(T item);
        T Dequeue();

        bool TryDequeue(out T item);
    }
}
