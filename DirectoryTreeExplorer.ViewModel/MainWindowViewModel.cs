using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DirectoryTreeExplorer.Business;
using DirectoryTreeExplorer.Business.LockFreeQueue;
using DirectoryTreeExplorer.Business.Logs;
using DirectoryTreeExplorer.ViewModel.Providers;

namespace DirectoryTreeExplorer.ViewModel
{
    /// <summary>
    /// Represents the view model for main window.
    /// </summary>
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly object _lock = new object();
        private string _xmlPath;

        private readonly ILogProvider _logProvider;
        private readonly IDialogProvider _dialogProvider;

        private readonly IterateDirectoryHelper _iterateDirectoryHelper;
        private readonly XmlCreator _xmlCreator;

        public ObservableCollection<Node> Nodes { get; }
        public List<string> Logs { get; }

        public ICommand ClickCommandBrowseXml { get; }
        public ICommand ClickCommandChooseDirectory { get; }


        public MainWindowViewModel(IDialogProvider folderBrowserDialogProvider)
        {
            _dialogProvider = folderBrowserDialogProvider;
            _logProvider = new ExternalHandlerLogProvider(AddLog);
            _iterateDirectoryHelper = new IterateDirectoryHelper(_logProvider);
            _xmlCreator = new XmlCreator(_logProvider);

            Nodes = new ObservableCollection<Node>();
            Logs = new List<string>();

            BindingOperations.EnableCollectionSynchronization(Nodes, _lock);

            this.ClickCommandBrowseXml = new Command(this.ClickMethodBrowseXml);
            this.ClickCommandChooseDirectory = new Command(this.ClickMethodChooseDirectory);
        }

        public string XmlPath
        {
            get => _xmlPath;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || _xmlPath == value)
                    return;

                _xmlPath = value;
                OnPropertyChanged(nameof(XmlPath));
            }
        }

        private void AddLog(string message)
        {
            Logs.Add($"{DateTime.Now.TimeOfDay}: {message}");
            OnPropertyChanged(nameof(Logs));
        }

        private void ClearData()
        {
            Nodes.Clear();
            Logs.Clear();
        }

        private void FillNodes(IQueue<DirectoryElement> directoryElements)
        {
            _logProvider.Log("TreeView filling has been started.");

            const int topLevelNumber = 1;

            var cachedLastUsedNodes = new Dictionary<int, Node>();

            //
            // Skip a main root directory.
            // Not needed to show it on the UI.
            //
            while (!directoryElements.TryDequeue(out var mainRootElement)) ;

            while (directoryElements.IsEmpty)
            {
                if (!_iterateDirectoryHelper.IsIterationActive)
                    return;
            }

            DirectoryElement currentRootElement;
            while (!directoryElements.TryDequeue(out currentRootElement)) ;

            var currentRootNode = new Node(currentRootElement.Name, currentRootElement.Level);

            cachedLastUsedNodes[currentRootNode.Level] = currentRootNode;

            while (!directoryElements.IsEmpty || _iterateDirectoryHelper.IsIterationActive)
            {
                if (directoryElements.IsEmpty)
                    continue;

                directoryElements.TryDequeue(out var currentElement);

                var currentNode = new Node(currentElement.Name, currentElement.Level);

                if (currentNode.Level == topLevelNumber)
                {
                    Nodes.Add(currentRootNode);
                    currentRootNode = currentNode;
                }
                else
                {
                    cachedLastUsedNodes[currentNode.Level - 1].Nodes.Add(currentNode);
                }

                cachedLastUsedNodes[currentNode.Level] = currentNode;
            }

            Nodes.Add(currentRootNode);

            _logProvider.Log("TreeView filling has been finished.");
        }

        private async void ClickMethodChooseDirectory()
        {
            var path = _dialogProvider.ShowFolderBrowserDialog();

            if (!Directory.Exists(path))
                return;

            ClearData();

            _iterateDirectoryHelper.StartIteration(path);

            _xmlCreator.Create(_xmlPath, _iterateDirectoryHelper.FoundDataForXml, () => _iterateDirectoryHelper.IsIterationActive);

            await Task.Run(() => FillNodes(_iterateDirectoryHelper.FoundDataForTreeView));
        }

        private void ClickMethodBrowseXml()
        {
            XmlPath = _dialogProvider.SaveFileDialog();
        }
    }
}
