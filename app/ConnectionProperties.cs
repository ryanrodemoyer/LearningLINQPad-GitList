using System.ComponentModel;
using System.IO;

using LINQPad.Extensibility.DataContext;

namespace LearningLINQPad.GitList
{
    /// <summary>
    /// Wrapper to read/write connection properties. This acts as our ViewModel - we will bind to it in ConnectionDialog.xaml.
    /// </summary>
    class ConnectionProperties : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const string DefaultBeyondComparePath = @"C:\Program Files\Beyond Compare 4\BComp.exe";

        public IConnectionInfo ConnectionInfo { get; private set; }

        public ConnectionProperties(IConnectionInfo cxInfo)
        {
            ConnectionInfo = cxInfo;
        }

        public string RepositoryPath
        {
            get => (string)ConnectionInfo.DriverData.Element("RepositoryPath") ?? "";
            set
            {
                ConnectionInfo.DriverData.SetElementValue("RepositoryPath", value);
                OnPropertyChanged(nameof(RepositoryPath));
            }
        }

        public string BeyondComparePath
        {
            get
            {
                var savedPath = (string)ConnectionInfo.DriverData.Element("BeyondComparePath");

                // If a path is saved, return it (even if empty, user may have cleared it intentionally)
                if (savedPath != null)
                    return savedPath;

                // No saved value - check if default Beyond Compare path exists
                if (File.Exists(DefaultBeyondComparePath))
                    return DefaultBeyondComparePath;

                // Default path doesn't exist, return empty
                return "";
            }
            set
            {
                ConnectionInfo.DriverData.SetElementValue("BeyondComparePath", value);
                OnPropertyChanged(nameof(BeyondComparePath));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}