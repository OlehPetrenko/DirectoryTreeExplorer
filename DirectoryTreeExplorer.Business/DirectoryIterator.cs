using System;
using System.IO;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Generates and provides data about found elements in a directory.
    /// </summary>
    public sealed class DirectoryIterator
    {
        public void IterateThroughDirectoryTree(DirectoryInfo root, Action<DirectoryElement> handleFoundElement, int level = 0)
        {
            const int rootLevelNumber = 0;
            var nextLevelNumber = level + 1;

            //
            // Skip root directory.
            //
            if (level != rootLevelNumber)
                handleFoundElement(CreateDirectoryElement(root, level));

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

            foreach (var file in files)
            {
                handleFoundElement(CreateDirectoryElement(file, nextLevelNumber));
            }
        }

        private DirectoryElement CreateDirectoryElement(FileInfo file, int level)
        {
            return
                new DirectoryElement
                {
                    Name = file.Name,
                    Level = level
                };
        }

        private DirectoryElement CreateDirectoryElement(DirectoryInfo directory, int level)
        {
            return
                new DirectoryElement
                {
                    Name = directory.Name,
                    Level = level
                };
        }
    }
}
