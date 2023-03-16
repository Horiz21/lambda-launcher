using LambdaLauncher.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace LambdaLauncher {
	public partial class Option : Window {
		public Option() {
			InitializeComponent();
			ResourceDictionary appResources = Application.Current.Resources;

			//appResources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("zh_cn.xaml", UriKind.RelativeOrAbsolute) });
			//appResources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("en_us.xaml", UriKind.RelativeOrAbsolute) });
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
	}
}
