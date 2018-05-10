using System;
using System.IO;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Represents a generator and provider data about found elements in a directory.
    /// </summary>
    internal sealed class DirectoryIterator
    {
        private const int RootLevelNumber = 0;


        public void IterateThroughDirectoryTree(DirectoryInfo root, Action<DirectoryElement> handleFoundElement, int level = 0)
        {
            var nextLevelNumber = level + 1;

            handleFoundElement(new DirectoryElement(root, level, level == RootLevelNumber ? DirectoryElementKind.Root : DirectoryElementKind.Directory));

            FileInfo[] files = null;

            try
            {
                files = root.GetFiles("*.*");
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }

            if (files == null)
                return;

            var subDirs = root.GetDirectories();

            foreach (var dirInfo in subDirs)
            {
                IterateThroughDirectoryTree(dirInfo, handleFoundElement, nextLevelNumber);
            }

            foreach (var fileInfo in files)
            {
                handleFoundElement(new DirectoryElement(fileInfo, nextLevelNumber, DirectoryElementKind.File));
            }
        }
    }
}
