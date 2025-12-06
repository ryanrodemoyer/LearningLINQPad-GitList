using System.ComponentModel;

using LINQPad.Extensibility.DataContext;

namespace LearningLINQPad.GitList
{
    /// <summary>
    /// Wrapper to read/write connection properties. This acts as our ViewModel - we will bind to it in ConnectionDialog.xaml.
    /// </summary>
    class ConnectionProperties : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
            get => (string)ConnectionInfo.DriverData.Element("BeyondComparePath") ?? "";
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