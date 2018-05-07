using System;
using System.Windows.Forms;
using DirectoryTreeExplorer.ViewModel.Providers;

namespace DirectoryTreeExplorer.Presentation
{
    /// <summary>
    /// Represents common used dialogs.
    /// </summary>
    public sealed class DialogProvider : IDialogProvider
    {
        public string ShowFolderBrowserDialog()
        {
            var fbd = new FolderBrowserDialog
            {
                Description = @"Please choose a folder to iterate through."
            };

            return fbd.ShowDialog() == DialogResult.OK ? fbd.SelectedPath : string.Empty;
        }
    }
}
