using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace LambdaLauncher {
	public partial class Setting : Window {
		private Dictionary<string, string> LanguageDictionary = new Dictionary<string, string> {
			{"中文(中国)","zh_cn"},
			{"English","en_us"},
		};

		public Setting() {
			InitializeComponent();
		}

		private void ChangeLanguage(object sender, RoutedEventArgs e) {

		}

		private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

		private void CloseWindow(object sender, RoutedEventArgs e) => Close();

		private void TempChangeLanguage(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			int index = boxLanguage.SelectedValue.ToString().IndexOf(' ');
			if (index != -1) {
				string language = boxLanguage.SelectedValue.ToString().Substring(index+1);
				if (language != string.Empty) {
					string head = @"pack://application:,,,/";
					App.Current.Resources.MergedDictionaries[0].Source = new Uri(head + "Language/" + LanguageDictionary[language] + ".xaml");
				}
			}
		}
	}
}
