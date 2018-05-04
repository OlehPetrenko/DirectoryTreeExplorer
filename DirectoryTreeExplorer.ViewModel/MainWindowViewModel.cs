using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using DirectoryTreeExplorer.Business;

namespace DirectoryTreeExplorer.ViewModel
{
    /// <summary>
    /// Represents the view model for main window.
    /// </summary>
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Node> Nodes { get; set; }

        public ICommand ClickCommandChooseDirectory
        {
            get;
        }

        private void ClickMethodChooseDirectory()
        {
            Nodes.Add(
                new Node
                {
                    Name = "_root",
                    Nodes = new ObservableCollection<Node>
                    {
                        new Node {Name = "1"},
                        new Node
                        {
                            Name = "2",
                            Nodes = new ObservableCollection<Node>
                            {
                                new Node {Name = "2.1"},
                                new Node {Name = "2.2"}
                            }
                        },
                        new Node {Name = "3"}
                    }
                }
            );
        }

        public MainWindowViewModel()
        {
            Nodes = new ObservableCollection<Node>();

            this.ClickCommandChooseDirectory = new Command(this.ClickMethodChooseDirectory);
        }
    }
}
