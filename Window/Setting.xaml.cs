using LambdaLauncher.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace LambdaLauncher {
	public partial class Setting : Window {
		private string Language;
		private string Theme;
		private bool DarkMode;
		private bool KeyboardDouble;
		private bool MouseDouble;
		private int LambdaFunction;

		public Setting() {
			InitializeComponent();
		}

		/// <summary>
		/// 确认更新，则通过Data，将当前界面所有信息写回lls配置文件，然后重新读取设置
		/// </summary>
		private void Confirm(object sender, RoutedEventArgs e) {
			Data.SaveLlsSettings(Language, Theme, DarkMode, KeyboardDouble, MouseDouble, LambdaFunction);
			Data.LoadLlsSettings();
		}

		private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

		private void CloseWindow(object sender, RoutedEventArgs e) => Close();

		private void TempChangeLanguage(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			int index = boxLanguage.SelectedValue.ToString().IndexOf(' ');
			if (index != -1) {
				Language = boxLanguage.SelectedValue.ToString().Substring(index+1);
				if (Language != string.Empty) {
					string head = @"pack://application:,,,/";
					Application.Current.Resources.MergedDictionaries[0].Source = new Uri(head + "Language/" + Data.LanguageDictionary[Language] + ".xaml");
				}
			}
		}

		private void TempChangeTheme(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			int index = boxTheme.SelectedValue.ToString().IndexOf(' ');
			if (index != -1) {
				Theme = boxTheme.SelectedValue.ToString().Substring(index + 1);
				if (Theme != string.Empty) {
					string head = @"pack://application:,,,/";
					Application.Current.Resources.MergedDictionaries[1].Source = new Uri(head + "Resource/Themes/" + Data.ThemeDictionary[Theme] + ".xaml");
				}
			}
		}
	}
}
