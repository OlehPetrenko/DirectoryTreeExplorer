using System;

namespace DirectoryTreeExplorer.Business.LockFreeQueue
{
    /// <summary>
    /// Represents a item for <see cref="LockFreeQueue{T}"/>. 
    /// </summary>
    /// <typeparam name="T">The type of the data contained in the item.</typeparam>
    public sealed class LockFreeQueueItem<T> where T : class, new()
    {
        public T Data { get; set; }
        public LockFreeQueueItem<T> Next { get; set; }

        public LockFreeQueueItem(T data = null, LockFreeQueueItem<T> next = null)
        {
            Data = data;
            Next = next;
        }
    }
}
