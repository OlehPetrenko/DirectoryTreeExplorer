using System;
using System.IO;
using System.Security.Principal;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Information of a directory element.
    /// </summary>
    public sealed class DirectoryElement
    {
        private readonly FileSystemInfo _info;


        public string Name => _info.Name;
        public DateTime CreationTime => _info.CreationTime;
        public DateTime ModificationTime => _info.LastWriteTime;
        public DateTime LastAccessTime => _info.LastAccessTime;
        public FileAttributes Attributes => _info.Attributes;

        public long Size => (_info as FileInfo)?.Length ?? 0;

        public string Owner
        {
            get
            {
                try
                {
                    return File.GetAccessControl(_info.FullName).GetOwner(typeof(NTAccount)).Value;
                }
                catch (Exception)
                {
                    return "Not available";
                }
            }
        }

        public int Level { get; }
        public DirectoryElementKind Kind { get; }

        public DirectoryElement()
        {
        }

        public DirectoryElement(FileSystemInfo fileSystemInfo, int level, DirectoryElementKind kind)
            : base()
        {
            _info = fileSystemInfo;

            Level = level;
            Kind = kind;
        }
    }
}
