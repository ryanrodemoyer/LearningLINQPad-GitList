using System;
using System.Windows;
using System.IO;

using LINQPad.Extensibility.DataContext;

namespace LearningLINQPad.GitList
{
    public partial class ConnectionDialog : Window
    {
        readonly IConnectionInfo _cxInfo;

        public ConnectionDialog(IConnectionInfo cxInfo)
        {
            _cxInfo = cxInfo;

            // ConnectionProperties is your view-model.
            DataContext = new ConnectionProperties(cxInfo);

            InitializeComponent();
        }

        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var props = (ConnectionProperties)DataContext;

            // Validate that a repository path is specified
            if (string.IsNullOrWhiteSpace(props.RepositoryPath))
            {
                MessageBox.Show("Please specify a git repository path.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate that the path exists
            if (!Directory.Exists(props.RepositoryPath))
            {
                MessageBox.Show("The specified path does not exist.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate that it's a git repository (has .git folder)
            string gitPath = System.IO.Path.Combine(props.RepositoryPath, ".git");
            if (!Directory.Exists(gitPath) && !File.Exists(gitPath)) // .git can be a file in worktrees
            {
                MessageBox.Show("The specified path is not a git repository (no .git folder found).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
        }

        void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "Select Git Repository Folder"
            };

            var props = (ConnectionProperties)DataContext;
            if (!string.IsNullOrWhiteSpace(props.RepositoryPath) && Directory.Exists(props.RepositoryPath))
            {
                dialog.InitialDirectory = props.RepositoryPath;
            }

            if (dialog.ShowDialog() == true)
            {
                props.RepositoryPath = dialog.FolderName;
            }
        }

        void btnBrowseBeyondCompare_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select Beyond Compare Executable",
                Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*",
                CheckFileExists = true
            };

            var props = (ConnectionProperties)DataContext;
            if (!string.IsNullOrWhiteSpace(props.BeyondComparePath) && File.Exists(props.BeyondComparePath))
            {
                dialog.FileName = props.BeyondComparePath;
            }

            if (dialog.ShowDialog() == true)
            {
                props.BeyondComparePath = dialog.FileName;
            }
        }
    }
}