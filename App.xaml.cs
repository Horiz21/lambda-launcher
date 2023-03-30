using LambdaLauncher.Utility;
using System.Windows;

namespace LambdaLauncher {

	public partial class App : Application {

		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);
			Data.LoadData();
		}
	}
}