using LambdaLauncher.Utility;
using System;
using System.Windows;

namespace LambdaLauncher {

	public partial class Setting : Window {
		private new int Language = Data.Language;
		private int Theme = Data.Theme;
		private bool DarkMode = Data.DarkMode;
		private bool KeyboardDouble = Data.KeyboardDouble;
		private bool MouseDouble = Data.MouseDouble;
		private int LambdaFunction = Data.LambdaFunction;

		public Setting() {
			InitializeComponent();

			// 在设置页面复原当前设置
			boxLanguage.SelectedIndex = Language;
			boxTheme.SelectedIndex = Theme;
			boxLambdaFunction.SelectedIndex = LambdaFunction;

			if (DarkMode) radioDarkModeOn.IsChecked = true;
			else radioDarkModeOff.IsChecked = true;

			if (KeyboardDouble) radioKeyboardDoubleOn.IsChecked = true;
			else radioKeyboardDoubleOff.IsChecked = true;

			if (MouseDouble) radioMouseDoubleOn.IsChecked = true;
			else radioMouseDoubleOff.IsChecked = true;
		}

		/// <summary>
		/// 确认更新，则通过Data，将当前界面所有信息写回lls配置文件，然后重新读取设置
		/// </summary>
		private void Confirm(object sender, RoutedEventArgs e) {
			Data.SaveLlsSettings(Language, Theme, DarkMode, KeyboardDouble, MouseDouble, LambdaFunction);
			Data.LoadLlsSettings();
			Close();
		}

		private void Cancel(object sender, RoutedEventArgs e) {
			Data.LoadLlsSettings();
			Close();
		}

		private void TempChangeLanguage(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			Language = boxLanguage.SelectedIndex;
			Application.Current.Resources.MergedDictionaries[0].Source = new Uri("../Language/" + Data.Languages[Language] + ".xaml", UriKind.Relative);
		}

		private void TempChangeTheme(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			Theme = boxTheme.SelectedIndex;
			Application.Current.Resources.MergedDictionaries[1].Source = new Uri("../Resource/Themes/" + Data.Themes[Theme] + ".xaml", UriKind.Relative);
			Application.Current.Resources.MergedDictionaries[2].Source = new Uri("../Resource/Themes/" + (DarkMode ? "DarkMode" : "LightMode") + ".xaml", UriKind.Relative);
		}

		private void TempChangeDarkModeOn(object sender, RoutedEventArgs e) {
			DarkMode = true;
			Application.Current.Resources.MergedDictionaries[2].Source = new Uri("../Resource/Themes/DarkMode.xaml", UriKind.Relative);
		}

		private void TempChangeDarkModeOff(object sender, RoutedEventArgs e) {
			DarkMode = false;
			Application.Current.Resources.MergedDictionaries[2].Source = new Uri("../Resource/Themes/LightMode.xaml", UriKind.Relative);
		}

		private void TempChangeLambdaFunction(object sender, RoutedEventArgs e) {
			LambdaFunction = boxLambdaFunction.SelectedIndex;
		}

		private void MouseDoubleOn(object sender, RoutedEventArgs e) =>
			Data.MouseDouble = MouseDouble = true;

		private void MouseDoubleOff(object sender, RoutedEventArgs e) =>
			Data.MouseDouble = MouseDouble = false;

		private void KeyboardDoubleOn(object sender, RoutedEventArgs e) =>
			Data.KeyboardDouble = KeyboardDouble = true;

		private void KeyboardDoubleOff(object sender, RoutedEventArgs e) =>
			Data.KeyboardDouble = KeyboardDouble = false;

		private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

		private void CloseWindow(object sender, RoutedEventArgs e) => Cancel(sender, e);
	}
}