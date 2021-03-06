﻿using System;
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

        public ICommand ClickCommandBrowseXml { get; set; }
        public ICommand ClickCommandChooseDirectory { get; set; }

        public ObservableCollection<Node> Nodes { get; }
        public List<string> Logs { get; }

        public string XmlPath
        {
            get => _xmlPath;
            set
            {
                if (_xmlPath == value)
                    return;

                _xmlPath = value;
                OnPropertyChanged(nameof(XmlPath));
            }
        }


        public MainWindowViewModel(IDialogProvider folderBrowserDialogProvider)
        {
            _dialogProvider = folderBrowserDialogProvider;
            _logProvider = new ExternalHandlerLogProvider(AddLog);
            _iterateDirectoryHelper = new IterateDirectoryHelper(_logProvider);
            _xmlCreator = new XmlCreator(_logProvider);

            Nodes = new ObservableCollection<Node>();
            Logs = new List<string>();

            BindingOperations.EnableCollectionSynchronization(Nodes, _lock);

            InitializeCommands();
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
            while (!directoryElements.TryDequeue(out var _)) ;

            while (directoryElements.IsEmpty)
            {
                if (!_iterateDirectoryHelper.IsIterationActive)
                    return;
            }

            DirectoryElement currentRootElement;
            while (!directoryElements.TryDequeue(out currentRootElement)) ;

            cachedLastUsedNodes[currentRootElement.Level] = new Node(currentRootElement.Name, currentRootElement.Level);

            while (!directoryElements.IsEmpty || _iterateDirectoryHelper.IsIterationActive)
            {
                if (directoryElements.IsEmpty)
                    continue;

                var currentElement = directoryElements.Dequeue();
                var currentNode = new Node(currentElement.Name, currentElement.Level);

                if (currentNode.Level == topLevelNumber)
                {
                    Nodes.Add(cachedLastUsedNodes[topLevelNumber]);
                }
                else
                {
                    cachedLastUsedNodes[currentNode.Level - 1].Nodes.Add(currentNode);
                }

                cachedLastUsedNodes[currentNode.Level] = currentNode;
            }

            cachedLastUsedNodes.TryGetValue(topLevelNumber, out var lastUsedTopLevelNode);
            Nodes.Add(lastUsedTopLevelNode);

            _logProvider.Log("TreeView filling has been finished.");
        }

        private async Task StartProcesses(string path)
        {
            _iterateDirectoryHelper.StartIteration(path);

            if (!string.IsNullOrWhiteSpace(_xmlPath))
            {
                _xmlCreator.Create(_xmlPath, _iterateDirectoryHelper.FoundDataForXml, () => _iterateDirectoryHelper.IsIterationActive);
            }

            await Task.Run(() => FillNodes(_iterateDirectoryHelper.FoundDataForTreeView));
        }

        #region Commands

        private void InitializeCommands()
        {
            this.ClickCommandBrowseXml = new Command(this.ClickMethodBrowseXml);
            this.ClickCommandChooseDirectory = new Command(this.ClickMethodChooseDirectory);
        }

        private async void ClickMethodChooseDirectory()
        {
            var path = _dialogProvider.ShowFolderBrowserDialog();

            if (!Directory.Exists(path))
                return;

            ClearData();

            try
            {
                await StartProcesses(path);
            }
            catch (Exception exception)
            {
                _logProvider.Log(exception.Message);
            }
        }

        private void ClickMethodBrowseXml()
        {
            XmlPath = _dialogProvider.SaveFileDialog();
        }

        #endregion
    }
}
