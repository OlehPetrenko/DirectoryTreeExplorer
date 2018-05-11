using System;
using System.IO;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Information of a directory element.
    /// </summary>
    public sealed class DirectoryElement
    {
        private readonly FileSystemInfo _info;
        private string _owner;


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
                if (string.IsNullOrEmpty(_owner))
                    _owner = new OwnerProvider().GetOwner(_info.FullName);

                return _owner;
            }
        }

        public int Level { get; }
        public DirectoryElementKind Kind { get; }

        public DirectoryElement(FileSystemInfo fileSystemInfo, int level, DirectoryElementKind kind)
            : base()
        {
            _info = fileSystemInfo;

            Level = level;
            Kind = kind;
        }

        public DirectoryElement()
        {
        }
    }
}
