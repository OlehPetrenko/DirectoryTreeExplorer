using System.Collections.ObjectModel;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Holds a data about found resource. 
    /// </summary>
    public sealed class Node
    {
        public string Name { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }
    }
}
