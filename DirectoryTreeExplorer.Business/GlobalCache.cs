using System;
using System.Collections.Generic;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Represents a global cache for application. 
    /// </summary>
    public sealed class GlobalCache
    {
        private static readonly Lazy<GlobalCache> Lazy = new Lazy<GlobalCache>(() => new GlobalCache());

        private GlobalCache()
        {
            Owners = new Dictionary<string, string>();
        }

        public static GlobalCache Instance => Lazy.Value;


        public Dictionary<string, string> Owners { get; }
    }
}
