using System;
using System.IO;
using System.Threading;
using DirectoryTreeExplorer.Business.LockFreeQueue;
using DirectoryTreeExplorer.Business.Logs;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Represents a logic to begin directory iteration process.
    /// </summary>
    public sealed class IterateDirectoryHelper
    {
        private readonly ILogProvider _logProvider;

        private Thread _iterateDirectoryThread;

        public IQueue<DirectoryElement> FoundDataForTreeView { get; private set; }
        public IQueue<DirectoryElement> FoundDataForXml { get; private set; }


        public bool IsIterationActive => _iterateDirectoryThread.IsAlive;

        public IterateDirectoryHelper(ILogProvider logProvider = null)
        {
            _logProvider = logProvider;
        }

        public void StartIteration(string path)
        {
            if (_iterateDirectoryThread != null && _iterateDirectoryThread.IsAlive)
                _iterateDirectoryThread.Abort();

            FoundDataForTreeView = new LockFreeQueue<DirectoryElement>();
            FoundDataForXml = new LockFreeQueue<DirectoryElement>();

            _iterateDirectoryThread = new Thread(IterateThroughDirectory) { IsBackground = true };
            _iterateDirectoryThread.Start(path);
        }

        private void IterateThroughDirectory(object path)
        {
            _logProvider?.Log($"Iteration of '{path}' has been started.");

            var iterator = new DirectoryIterator(_logProvider);
            iterator.IterateThroughDirectoryTree(new DirectoryInfo((string)path), AddFoundElement);

            _logProvider?.Log($"Iteration of '{path}' has been finished.");
        }

        private void AddFoundElement(DirectoryElement foundElement)
        {
            FoundDataForTreeView.Enqueue(foundElement);
            FoundDataForXml.Enqueue(foundElement);
        }
    }
}
