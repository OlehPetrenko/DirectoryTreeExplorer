using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using DirectoryTreeExplorer.Business.LockFreeQueue;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Provides logic to begin directory iteration process.
    /// </summary>
    public sealed class IterateDirectoryHelper
    {
        private Thread _iterateDirectoryThread;
        
        public IQueue<DirectoryElement> FoundData = new LockFreeQueue<DirectoryElement>(); 

        public bool IsIterationActive => _iterateDirectoryThread.IsAlive;

        public void StartIteration(string path)
        {
            _iterateDirectoryThread = new Thread(IterateThroughDirectory) { IsBackground = true };
            _iterateDirectoryThread.Start(path);
        }

        private void IterateThroughDirectory(object path)
        {
            var iterator = new DirectoryIterator();
            iterator.IterateThroughDirectoryTree(new DirectoryInfo((string)path), AddFoundElement);
        }

        private void AddFoundElement(DirectoryElement foundElement)
        {
            FoundData.Enqueue(foundElement);
        }
    }
}
