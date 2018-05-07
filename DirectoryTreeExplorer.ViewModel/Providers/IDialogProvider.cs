using System;

namespace DirectoryTreeExplorer.ViewModel.Providers
{
    /// <summary>
    /// Defines methods to provide common used dialogs.
    /// </summary>
    public interface IDialogProvider
    {
        string ShowFolderBrowserDialog();
    }
}
