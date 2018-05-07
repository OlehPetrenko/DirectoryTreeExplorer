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
        public string SaveFileDialog()
        {
            var dialog = new SaveFileDialog
            {
                Filter = @"XML files (*.xml)|*.xml"
            };

            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : string.Empty;
        }

        public string ShowFolderBrowserDialog()
        {
            var dialog = new FolderBrowserDialog
            {
                Description = @"Please choose a folder to iterate through."
            };

            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }
    }
}
