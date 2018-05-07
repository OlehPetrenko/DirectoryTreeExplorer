using System.Windows;
using DirectoryTreeExplorer.ViewModel;

namespace DirectoryTreeExplorer.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Starts application by opening its main window.
        /// </summary>
        public App()
        {
            var mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(new DialogProvider())
            };

            mainWindow.Show();
        }
    }
}
