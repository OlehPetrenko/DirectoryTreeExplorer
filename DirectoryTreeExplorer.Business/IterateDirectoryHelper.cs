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

        private Thread IterateDirectoryThread
        {
            get
            {
                if (_iterateDirectoryThread != null)
                    return _iterateDirectoryThread;

                return _iterateDirectoryThread = new Thread(IterateThroughDirectory) { IsBackground = true };
            }
        }


        public IterateDirectoryHelper(ILogProvider logProvider = null)
        {
            _logProvider = logProvider;
        }

        public bool IsIterationActive => IterateDirectoryThread.IsAlive;

        public void StartIteration(string path)
        {
            TerminateCurrentIteration();

            FoundDataForTreeView = new LockFreeQueue<DirectoryElement>();
            FoundDataForXml = new LockFreeQueue<DirectoryElement>();

            IterateDirectoryThread.Start(path);
        }

        private void AddFoundElement(DirectoryElement foundElement)
        {
            FoundDataForTreeView.Enqueue(foundElement);
            FoundDataForXml.Enqueue(foundElement);
        }

        private void TerminateCurrentIteration()
        {
            if (IterateDirectoryThread == null)
                return;

            if (IterateDirectoryThread.IsAlive)
            {
                IterateDirectoryThread.Abort();
                IterateDirectoryThread.Join();
            }

            _iterateDirectoryThread = null;
        }

        private void IterateThroughDirectory(object path)
        {
            _logProvider?.Log($"Iteration of '{path}' has been started.");

            var iterator = new DirectoryIterator(_logProvider);
            iterator.IterateThroughDirectoryTree(new DirectoryInfo((string)path), AddFoundElement);

            _logProvider?.Log($"Iteration of '{path}' has been finished.");
        }
    }
}
