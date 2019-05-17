using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TFSPeeker.Interrogation;

namespace Desktop.WPF
{
	/// <summary>
	/// Interaction logic for MainForm.xaml
	/// </summary>
	public partial class MainForm : Window
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void HandleDoubleClick(object sender, MouseButtonEventArgs e)
		{
			TestCaseDescription itemClicked = (TestCaseDescription) ((ListViewItem) sender).Content;
			Process.Start(itemClicked.Url);
		}
	}
}
