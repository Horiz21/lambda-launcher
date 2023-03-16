using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace LambdaLauncher {
	public partial class Setting : Window {
		public Setting() {
			InitializeComponent();
		}

		private void ChangeLanguage(object sender, RoutedEventArgs e) {
			string appXamlPath = System.IO.Path.GetFullPath(@"..\..\..\App.xaml");
			XDocument appXaml = XDocument.Load(appXamlPath);

			XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";

			// 找到当前语言的字典路径
			var currentLanguageDict = appXaml.Descendants(x + "ResourceDictionary")
				.Where(d => d.Attribute("x:Name")?.Value == "CurrentLanguage")
				.FirstOrDefault();

			// 若存在这条路径（通常情况下都是存在的），则修改原始路径，然后写回
			if (currentLanguageDict != null) {
				var sourceAttr = currentLanguageDict.Attribute("Source");
				if (sourceAttr != null) {
					sourceAttr.Value = "pack://application:,,,/Language/en_us.xaml";
				}
			}
			appXaml.Save(appXamlPath);
		}

		private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

		private void CloseWindow(object sender, RoutedEventArgs e) => Close();
	}
}
