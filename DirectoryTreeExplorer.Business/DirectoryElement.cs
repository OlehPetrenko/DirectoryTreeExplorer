using System;
using System.IO;
using System.Security.AccessControl;
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
                IdentityReference sid = null;

                try
                {
                    sid = File.GetAccessControl(_info.FullName, AccessControlSections.Owner).GetOwner(typeof(SecurityIdentifier));

                    GlobalCache.Instance.Owners.TryGetValue(sid.Value, out var owner);
                    if (!string.IsNullOrEmpty(owner))
                        return owner;

                    var ntAccount = sid.Translate(typeof(NTAccount));

                    GlobalCache.Instance.Owners.Add(sid.Value, ntAccount.ToString());

                    return ntAccount.ToString();
                }
                catch (Exception)
                {
                    const string notAvailableOwnerText = "Not available";

                    if (sid != null)
                        GlobalCache.Instance.Owners.Add(sid.Value, notAvailableOwnerText);

                    return notAvailableOwnerText;
                }
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
