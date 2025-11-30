using LINQPad.Extensibility.DataContext;
using System.ComponentModel;
using System.Xml.Linq;

namespace LearningLINQPad.GitList
{
	/// <summary>
	/// Wrapper to read/write connection properties. This acts as our ViewModel - we will bind to it in ConnectionDialog.xaml.
	/// </summary>
	class ConnectionProperties : INotifyPropertyChanged
	{
		public IConnectionInfo ConnectionInfo { get; private set; }

		XElement DriverData => ConnectionInfo.DriverData;

		public ConnectionProperties (IConnectionInfo cxInfo)
		{
			ConnectionInfo = cxInfo;
		}

		public string RepositoryPath
		{
			get => (string)DriverData.Element("RepositoryPath") ?? "";
			set
			{
				DriverData.SetElementValue("RepositoryPath", value);
				OnPropertyChanged(nameof(RepositoryPath));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}