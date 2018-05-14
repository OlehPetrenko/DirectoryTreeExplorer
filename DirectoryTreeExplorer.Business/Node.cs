using System.Collections.ObjectModel;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Holds information about node of TreeView on the UI.
    /// </summary>
    public sealed class Node
    {
        public string Name { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }

        public int Level { get; set; }


        public Node(string name, int level)
        {
            Name = name;
            Level = level;

            Nodes = new ObservableCollection<Node>();
        }
    }
}
