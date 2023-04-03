using System;
using System.Linq;
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
		private bool[] Modifier = new bool[4] { false, false, false, false };

		public Setting() {
			InitializeComponent();

			// 在设置页面复原当前设置
			boxLanguage.SelectedIndex = Language;
			boxTheme.SelectedIndex = Theme;
			boxLambdaFunction.SelectedIndex = LambdaFunction;

			// 复原日夜间、单双击设置
			if (DarkMode) radioDarkModeOn.IsChecked = true;
			else radioDarkModeOff.IsChecked = true;

			if (KeyboardDouble) radioKeyboardDoubleOn.IsChecked = true;
			else radioKeyboardDoubleOff.IsChecked = true;

			if (MouseDouble) radioMouseDoubleOn.IsChecked = true;
			else radioMouseDoubleOff.IsChecked = true;

			// 复原快捷键设置
			string[] parts = App.Hotkey.Split('+');
			if (parts.Contains("Ctrl")) {
				Modifier[0] = true;
				radioCtrl.IsChecked = true;
			}
			if (parts.Contains("Alt")) {
				Modifier[1] = true;
				radioAlt.IsChecked = true;
			}
			if (parts.Contains("Shift")) {
				Modifier[2] = true;
				radioShift.IsChecked = true;
			}
			if (parts.Contains("Win")) {
				Modifier[3] = true;
				radioShift.IsChecked = true;
			}
			boxHotkey.Text = parts.Last();
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

		/* 热键相关，包含四个辅助按键的选定以及实键的输入 */

		private void HotkeyCtrlChecked(object sender, RoutedEventArgs e) => Modifier[0] = true;

		private void HotkeyCtrlUnchecked(object sender, RoutedEventArgs e) => Modifier[0] = false;

		private void HotkeyAltChecked(object sender, RoutedEventArgs e) => Modifier[1] = true;

		private void HotkeyAltUnchecked(object sender, RoutedEventArgs e) => Modifier[1] = false;

		private void HotkeyShiftChecked(object sender, RoutedEventArgs e) => Modifier[2] = true;

		private void HotkeyShiftUnchecked(object sender, RoutedEventArgs e) => Modifier[2] = false;

		private void HotkeyWinChecked(object sender, RoutedEventArgs e) => Modifier[3] = true;

		private void HotkeyWinUnchecked(object sender, RoutedEventArgs e) => Modifier[3] = false;

		private void hotkeyInputStart(object sender, KeyEventArgs e) {
			e.Handled = true; // 取消事件的默认操作
			if (Keyboard.Modifiers == ModifierKeys.None && (e.Key < Key.LeftCtrl || e.Key > Key.RightAlt) && e.Key != Key.LWin && e.Key != Key.RWin) boxHotkey.Text = e.Key.ToString();
			else boxHotkey.Clear();
		}

		/// <summary>
		/// 确认更新，则通过Data，将当前界面所有信息写回lls配置文件，然后重新读取设置
		/// </summary>
		private void Confirm(object sender, RoutedEventArgs e) {
			if (boxHotkey.Text == string.Empty) { // 快捷键没有输则会失效，因此并不修改原来的快捷键
				Hotkey = App.Hotkey;
			}
			string oldHotkey = App.Hotkey;
			Hotkey = (Modifier[0] ? "Ctrl+" : "") + (Modifier[1] ? "Alt+" : "") + (Modifier[2] ? "Shift+" : "") + (Modifier[3] ? "Windows+" : "") + boxHotkey.Text;
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