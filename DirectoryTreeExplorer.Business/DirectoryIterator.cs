using System;
using System.Collections.Generic;
using System.IO;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Generates and provides data about found elements in a directory.
    /// </summary>
    public sealed class DirectoryIterator
    {
        public List<DirectoryElement> FoundData = new List<DirectoryElement>();


        public void IterateThroughDirectoryTree(DirectoryInfo root, int level = 0)
        {
            AddFoundDirectoryElement(root.Name, level);

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
                IterateThroughDirectoryTree(dirInfo, level + 1);
            }

            foreach (var file in files)
            {
                AddFoundDirectoryElement(file.Name, level + 1);
            }
        }

        private void AddFoundDirectoryElement(string name, int level)
        {
            FoundData.Add(new DirectoryElement
            {
                Name = name,
                Level = level
            });
        }
    }
}
