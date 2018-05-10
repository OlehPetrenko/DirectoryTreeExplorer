using System;
using System.IO;
using System.Threading;
using DirectoryTreeExplorer.Business.LockFreeQueue;
using DirectoryTreeExplorer.Business.Logs;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Provides logic to begin directory iteration process.
    /// </summary>
    public sealed class IterateDirectoryHelper
    {
        private readonly ILogProvider _logProvider;

        private Thread _iterateDirectoryThread;

        public IQueue<DirectoryElement> FoundDataForTreeView { get; }
        public IQueue<DirectoryElement> FoundDataForXml { get; }


        public IterateDirectoryHelper(ILogProvider logProvider = null)
        {
            _logProvider = logProvider;

            FoundDataForTreeView = new LockFreeQueue<DirectoryElement>();
            FoundDataForXml = new LockFreeQueue<DirectoryElement>();
        }

        public bool IsIterationActive => _iterateDirectoryThread.IsAlive;

        public void StartIteration(string path)
        {
            _iterateDirectoryThread = new Thread(IterateThroughDirectory) { IsBackground = true };
            _iterateDirectoryThread.Start(path);
        }

        private void IterateThroughDirectory(object path)
        {
            _logProvider?.Log($"Iteration of '{path}' has been started.");

            var iterator = new DirectoryIterator();
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
