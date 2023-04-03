using System;
using System.Windows;
using System.Windows.Input;

namespace LambdaLauncher {

	public partial class Setting : Window {
		private new int Language = App.Language;
		private int Theme = App.Theme;
		private bool DarkMode = App.DarkMode;
		private bool KeyboardDouble = App.KeyboardDouble;
		private bool MouseDouble = App.MouseDouble;
		private int LambdaFunction = App.LambdaFunction;
		private string Hotkey = App.Hotkey;

		public Setting() {
			InitializeComponent();

			// 在设置页面复原当前设置
			boxLanguage.SelectedIndex = Language;
			boxTheme.SelectedIndex = Theme;
			boxLambdaFunction.SelectedIndex = LambdaFunction;
			boxHotkey.Text = Hotkey;

			if (DarkMode) radioDarkModeOn.IsChecked = true;
			else radioDarkModeOff.IsChecked = true;

			if (KeyboardDouble) radioKeyboardDoubleOn.IsChecked = true;
			else radioKeyboardDoubleOff.IsChecked = true;

			if (MouseDouble) radioMouseDoubleOn.IsChecked = true;
			else radioMouseDoubleOff.IsChecked = true;
		}

		private void Cancel(object sender, RoutedEventArgs e) {
			App.ReadAndLoadSettings();
			Close();
		}

		private void TempChangeLanguage(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			Language = boxLanguage.SelectedIndex;
			App.Current.Resources.MergedDictionaries[0].Source = new Uri("../Properties/Languages/" + App.Languages[Language] + ".xaml", UriKind.Relative);
		}

		private void TempChangeTheme(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			Theme = boxTheme.SelectedIndex;
			App.Current.Resources.MergedDictionaries[1].Source = new
				("../Properties/Themes/" + App.Themes[Theme] + ".xaml", UriKind.Relative);
			App.Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/" + (DarkMode ? "DarkMode" : "LightMode") + ".xaml", UriKind.Relative);
		}

		private void TempChangeDarkModeOn(object sender, RoutedEventArgs e) {
			DarkMode = true;
			App.Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/DarkMode.xaml", UriKind.Relative);
		}

		private void TempChangeDarkModeOff(object sender, RoutedEventArgs e) {
			DarkMode = false;
			App.Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/LightMode.xaml", UriKind.Relative);
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

		private void DragWindow(object sender, MouseButtonEventArgs e) => DragMove();

		private void CloseWindow(object sender, RoutedEventArgs e) => Cancel(sender, e);

		private void CheckHotkeyAvailability(object sender, RoutedEventArgs e) {
		}

		private void hotkeyInputStart(object sender, System.Windows.Input.KeyEventArgs e) {
			Hotkey = "";
			if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)) {
				Hotkey += "Ctrl+";
			}
			if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Alt)) {
				Hotkey += "Alt+";
			}
			if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift)) {
				Hotkey += "Shift+";
			}
			if (e.Key != Key.LeftShift && e.Key != Key.RightShift
				&& e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl
				&& e.Key != Key.LeftAlt && e.Key != Key.RightAlt) {
				Hotkey += e.Key.ToString();
			}
			boxHotkey.Text = Hotkey;
		}

		/// <summary>
		/// 确认更新，则通过Data，将当前界面所有信息写回lls配置文件，然后重新读取设置
		/// </summary>
		private void Confirm(object sender, RoutedEventArgs e) {
			if (Hotkey.EndsWith('+')) {
				Hotkey = App.Hotkey;  // 快捷键没有输则会失效，因此并不修改原来的快捷键
			}
			string oldHotkey = App.Hotkey;
			App.SaveAndWriteSettings(Language, Theme, DarkMode, KeyboardDouble, MouseDouble, LambdaFunction, Hotkey);
			App.ReadAndLoadSettings();
			if (Hotkey != oldHotkey) {
				MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
				mainWindow.UnregisterHotKey(1134419766);
				mainWindow.Hotkey(sender, e);
			}
			Close();
		}
	}
}