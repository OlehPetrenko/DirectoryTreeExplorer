using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DirectoryTreeExplorer.ViewModel
{
    /// <summary>
    /// Represents the base logic for all view models.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
