using System;

namespace DirectoryTreeExplorer.Business.Logs
{
    /// <summary>
    /// Represents a logic to log information using external handlers. 
    /// </summary>
    public sealed class ExternalHandlerLogProvider : ILogProvider
    {
        private readonly Action<string> _addLogAction;


        public ExternalHandlerLogProvider(Action<string> addLogAction)
        {
            _addLogAction = addLogAction;
        }

        public void Log(string message)
        {
            _addLogAction(message);
        }
    }
}
