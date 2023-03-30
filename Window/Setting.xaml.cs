using LambdaLauncher.Utility;
using System;
using System.Windows;

namespace LambdaLauncher {

	public partial class Setting : Window {
		private new int Language = App.Language;
		private int Theme = App.Theme;
		private bool DarkMode = App.DarkMode;
		private bool KeyboardDouble = App.KeyboardDouble;
		private bool MouseDouble = App.MouseDouble;
		private int LambdaFunction = App.LambdaFunction;

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
			App.SaveLlsSettings(Language, Theme, DarkMode, KeyboardDouble, MouseDouble, LambdaFunction);
			App.LoadLlsSettings();
			Close();
		}

		private void Cancel(object sender, RoutedEventArgs e) {
			App.LoadLlsSettings();
			Close();
		}

		private void TempChangeLanguage(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			Language = boxLanguage.SelectedIndex;
			Application.Current.Resources.MergedDictionaries[0].Source = new Uri("../Properties/Languages/" + App.Languages[Language] + ".xaml", UriKind.Relative);
		}

		private void TempChangeTheme(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			Theme = boxTheme.SelectedIndex;
			Application.Current.Resources.MergedDictionaries[1].Source = new Uri("../Properties/Themes/" + App.Themes[Theme] + ".xaml", UriKind.Relative);
			Application.Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/" + (DarkMode ? "DarkMode" : "LightMode") + ".xaml", UriKind.Relative);
		}

		private void TempChangeDarkModeOn(object sender, RoutedEventArgs e) {
			DarkMode = true;
			Application.Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/DarkMode.xaml", UriKind.Relative);
		}

		private void TempChangeDarkModeOff(object sender, RoutedEventArgs e) {
			DarkMode = false;
			Application.Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/LightMode.xaml", UriKind.Relative);
		}

		private void TempChangeLambdaFunction(object sender, RoutedEventArgs e) {
			LambdaFunction = boxLambdaFunction.SelectedIndex;
		}

		private void MouseDoubleOn(object sender, RoutedEventArgs e) =>
			App.MouseDouble = MouseDouble = true;

		private void MouseDoubleOff(object sender, RoutedEventArgs e) =>
			App.MouseDouble = MouseDouble = false;

		private void KeyboardDoubleOn(object sender, RoutedEventArgs e) =>
			App.KeyboardDouble = KeyboardDouble = true;

		private void KeyboardDoubleOff(object sender, RoutedEventArgs e) =>
			App.KeyboardDouble = KeyboardDouble = false;

		private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

		private void CloseWindow(object sender, RoutedEventArgs e) => Cancel(sender, e);
	}
}