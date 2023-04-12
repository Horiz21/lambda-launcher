using LambdaLauncher.Model;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LambdaLauncher {

	public partial class Setting : Window {
		private Config oldConfig = App.config.DeepCopy();  // 暂时储存旧config

		private bool[] Modifier = new bool[4] { false, false, false, false };

		public Setting() {
			InitializeComponent();

			// 在设置页面复原当前设置
			boxLanguage.SelectedIndex = oldConfig.Language;
			boxTheme.SelectedIndex = oldConfig.Theme;
			boxLambdaFunction.SelectedIndex = oldConfig.LambdaFunction;

			// 复原日夜间、单双击设置
			if (oldConfig.DarkMode) radioDarkModeOn.IsChecked = true;
			else radioDarkModeOff.IsChecked = true;

			if (oldConfig.KeyboardDouble) radioKeyboardDoubleOn.IsChecked = true;
			else radioKeyboardDoubleOff.IsChecked = true;

			if (oldConfig.MouseDouble) radioMouseDoubleOn.IsChecked = true;
			else radioMouseDoubleOff.IsChecked = true;

			// 复原快捷键设置
			DisplayHotkey();
		}

		private void DisplayHotkey() {
			string[] parts = oldConfig.Hotkey.Split('+');
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
				radioWin.IsChecked = true;
			}
			boxHotkey.Text = parts.Last();
		}

		private void TempChangeLanguage(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			App.config.Language = boxLanguage.SelectedIndex;
			App.Current.Resources.MergedDictionaries[0].Source = new Uri("../Properties/Languages/" + Config.Languages[App.config.Language] + ".xaml", UriKind.Relative);
		}

		private void TempChangeTheme(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			App.config.Theme = boxTheme.SelectedIndex;
			App.Current.Resources.MergedDictionaries[1].Source = new
				("../Properties/Themes/" + Config.Themes[App.config.Theme] + ".xaml", UriKind.Relative);
			App.Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/" + (App.config.DarkMode ? "DarkMode" : "LightMode") + ".xaml", UriKind.Relative);
		}

		private void TempChangeDarkModeOn(object sender, RoutedEventArgs e) {
			App.config.DarkMode = true;
			App.Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/DarkMode.xaml", UriKind.Relative);
		}

		private void TempChangeDarkModeOff(object sender, RoutedEventArgs e) {
			App.config.DarkMode = false;
			App.Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/LightMode.xaml", UriKind.Relative);
		}

		private void TempChangeLambdaFunction(object sender, RoutedEventArgs e) {
			App.config.LambdaFunction = boxLambdaFunction.SelectedIndex;
		}

		private void MouseDoubleOn(object sender, RoutedEventArgs e) =>
			App.config.MouseDouble = true;

		private void MouseDoubleOff(object sender, RoutedEventArgs e) =>
			App.config.MouseDouble = false;

		private void KeyboardDoubleOn(object sender, RoutedEventArgs e) =>
			App.config.KeyboardDouble = true;

		private void KeyboardDoubleOff(object sender, RoutedEventArgs e) =>
			App.config.KeyboardDouble = false;

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

		#region 确认/取消操作

		/// <summary>
		/// 确认更新，则将App.config里所有的信息切实写回lls配置文件，然后加载新设置
		/// </summary>
		private void Confirm(object sender, RoutedEventArgs e) {
			// 当前正在运行的主界面对象
			MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

			// 若快捷键未键入，则不退出并要求键入快捷键
			if (boxHotkey.Text == string.Empty) {
				MessageBox.Show("[TODO]不允许没有快捷键实键");
				return;
			}

			// 若快捷键被修改，则尝试修改快捷键；若修改失败则要求重新键入快捷键
			App.config.Hotkey = string.Join("+", Modifier[0] ? "Ctrl" : "", Modifier[1] ? "Alt" : "", Modifier[2] ? "Shift" : "", Modifier[3] ? "Win" : "", boxHotkey.Text);
			if (App.config.Hotkey != oldConfig.Hotkey) {
				mainWindow.UnregisterHotKey(1134419766);
				if (!mainWindow.Hotkey(sender, e)) {
					MessageBox.Show((string)Application.Current.FindResource("HotkeyConflictErrorTip2"), (string)Application.Current.FindResource("HotkeyConflictError"));
					App.config.Hotkey = oldConfig.Hotkey;
					mainWindow.Hotkey(sender, e);
					DisplayHotkey();
					return;
				}
			}

			// 存储所有设置项，并立即应用到界面中
			App.ChangeSetting();

			// 修改语言
			mainWindow.ReloadLanguage();

			// 关闭窗口
			Close();
		}

		/// <summary>
		/// 确认更新，则将App.config重置为oldConfig，不改变lls文件内容并加载旧设置
		/// </summary>
		private void Cancel(object sender, RoutedEventArgs e) {
			App.RestoreSettings(oldConfig);
			Close();
		}

		#endregion 确认/取消操作

		private CultureInfo oldCultureInfo; // 记录之前的输入法，在焦点进入快捷键区时切换为英文输入法，离开时切换回旧输入法

		private void KeyboardToEnglish(object sender, KeyboardFocusChangedEventArgs e) {
			oldCultureInfo = InputLanguageManager.Current.CurrentInputLanguage;
			InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-US");
		}

		private void KeyboardBack(object sender, KeyboardFocusChangedEventArgs e) {
			InputLanguageManager.Current.CurrentInputLanguage = oldCultureInfo;
		}

		#region 配置文件导入/导出

		/// <summary>
		/// 导入配置文件，并加载其中设置
		/// </summary>
		private void Import(object sender, RoutedEventArgs e) {
			OpenFileDialog openFileDialog = new() { Filter = "LLS File|*lls" };
			if (openFileDialog.ShowDialog() == true) {
				if (Path.GetExtension(openFileDialog.FileName) == ".lls") {
					App.ImportSettings(openFileDialog.FileName);
				}
				else MessageBox.Show((string)Application.Current.FindResource("FileExtensionErrorTip"), (string)Application.Current.FindResource("FileExtensionError"));
			}
		}

		/// <summary>
		/// 导出当前配置App.config为配置文件
		/// </summary>
		private void Export(object sender, RoutedEventArgs e) {
			SaveFileDialog saveFileDialog = new() { Filter = "LLS Files|*.lls" };
			if (saveFileDialog.ShowDialog() == true) {
				string path = saveFileDialog.FileName;
				if (File.Exists(path)) {
					MessageBoxResult result = MessageBox.Show("该文件已存在，是否要替换？", "确认", MessageBoxButton.YesNo);
					if (result != MessageBoxResult.Yes) {
						return;
					}
				}
				else {
					using (File.Create(path)) {}
				}
				App.ExportSettings(path);
			}
		}

		#endregion 配置文件导入/导出
	}
}