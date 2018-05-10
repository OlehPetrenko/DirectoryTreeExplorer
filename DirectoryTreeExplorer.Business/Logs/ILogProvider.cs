using System;

namespace DirectoryTreeExplorer.Business.Logs
{
    /// <summary>
    /// Defines methods to log information.
    /// </summary>
    public interface ILogProvider
    {
        void Log(string messsage);
    }
}
