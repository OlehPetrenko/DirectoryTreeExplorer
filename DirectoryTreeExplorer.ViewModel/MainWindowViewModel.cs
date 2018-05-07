using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DirectoryTreeExplorer.Business;
using DirectoryTreeExplorer.Business.LockFreeQueue;
using DirectoryTreeExplorer.ViewModel.Providers;

namespace DirectoryTreeExplorer.ViewModel
{
    /// <summary>
    /// Represents the view model for main window.
    /// </summary>
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly object _lock = new object();
        private readonly IterateDirectoryHelper _iterateDirectoryHelper;
        private readonly IDialogProvider _dialogProvider;

        public ObservableCollection<Node> Nodes { get; set; }

        public ICommand ClickCommandChooseDirectory { get; }


        public MainWindowViewModel(IDialogProvider folderBrowserDialogProvider)
        {
            _iterateDirectoryHelper = new IterateDirectoryHelper();
            _dialogProvider = folderBrowserDialogProvider;

            Nodes = new ObservableCollection<Node>();
            BindingOperations.EnableCollectionSynchronization(Nodes, _lock);

            this.ClickCommandChooseDirectory = new Command(this.ClickMethodChooseDirectory);
        }

        private async void ClickMethodChooseDirectory()
        {
            var path = _dialogProvider.ShowFolderBrowserDialog();

            if(!Directory.Exists(path))
                return;

            _iterateDirectoryHelper.StartIteration(path);

            await Task.Run(() => FillNodes(_iterateDirectoryHelper.FoundDataForTreeView));
        }

        private void FillNodes(IQueue<DirectoryElement> directoryElements)
        {
            const int topLevelNumber = 1;

            var cachedLastUsedNodes = new Dictionary<int, Node>();

            DirectoryElement currentRootElement;
            while (!directoryElements.TryDequeue(out currentRootElement)) ;

            var currentRootNode = new Node(currentRootElement.Name, currentRootElement.Level);

            cachedLastUsedNodes[currentRootNode.Level] = currentRootNode;

            while (_iterateDirectoryHelper.IsIterationActive || !directoryElements.IsEmpty)
            {
                DirectoryElement currentElement;
                while (!directoryElements.TryDequeue(out currentElement)) ;

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
        }
    }
}
