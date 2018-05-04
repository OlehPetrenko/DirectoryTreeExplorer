using System;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Directory element information.
    /// </summary>
    public sealed class DirectoryElement
    {
        public string Name { get; set; }

        public int Level { get; set; }
    }
}
