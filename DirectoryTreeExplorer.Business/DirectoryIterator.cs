using System;
using System.IO;
using DirectoryTreeExplorer.Business.Logs;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Represents a generator and provider data about found elements in a directory.
    /// </summary>
    internal sealed class DirectoryIterator
    {
        private const int RootLevelNumber = 0;

        private readonly ILogProvider _logProvider;


        public DirectoryIterator(ILogProvider logProvider = null)
        {
            _logProvider = logProvider;
        }

        public void IterateThroughDirectoryTree(DirectoryInfo root, Action<DirectoryElement> handleFoundElement, int level = 0)
        {
            var nextLevelNumber = level + 1;

            handleFoundElement(new DirectoryElement(root, level, level == RootLevelNumber ? DirectoryElementKind.Root : DirectoryElementKind.Directory));

            try
            {
                foreach (var dirInfo in root.EnumerateDirectories())
                {
                    IterateThroughDirectoryTree(dirInfo, handleFoundElement, nextLevelNumber);
                }

                foreach (var fileInfo in root.EnumerateFiles("*.*"))
                {
                    handleFoundElement(new DirectoryElement(fileInfo, nextLevelNumber, DirectoryElementKind.File));
                }
            }
            catch (Exception exception)
            {
                _logProvider?.Log(exception.Message);
            }
        }
    }
}
