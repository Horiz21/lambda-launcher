using LambdaLauncher.Utility;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Forms = System.Windows.Forms;

namespace LambdaLauncher {

	public partial class MainWindow : Window {
		private static InteractiveKey[] keys = new InteractiveKey[27]; // 用于存放按钮控件
		private static UniformGrid[] gridRows = new UniformGrid[3]; // 用于存放三个Grid

		private Forms.NotifyIcon notifyIcon;
		private static char currentActivedKey; // 上一次按下的字母
		private static bool isSameActive; // 二次访问标记，是否已经预先按下（致使这是第二次按下）

		//private string? menuWebsite = Application.Current.FindResource("MenuWebsite") as string;
		//private string? menuExit = Application.Current.FindResource("MenuExit") as string;
		//private string? settings = Application.Current.FindResource("Settings") as string;
		//private string? keySettings1 = Application.Current.FindResource("KeySettings1") as string;
		//private string? keySettings2 = Application.Current.FindResource("KeySettings2") as string;
		//private string? claerKey = Application.Current.FindResource("ClearKey") as string;

		public MainWindow() {
			InitializeComponent();

			// 托盘图标初始化
			notifyIcon = new Forms.NotifyIcon();
			notifyIcon.Icon = new System.Drawing.Icon("Properties/icon.ico");
			notifyIcon.Visible = true;
			notifyIcon.DoubleClick += Show;
			notifyIcon.Text = "Lambda Launcher";
			notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();

			// 将三行记录在gridRows数组中
			gridRows[0] = gridRow1;
			gridRows[1] = gridRow2;
			gridRows[2] = gridRow3;

			ReloadLanguage();
			ReloadGrid();
		}

		public static void ReloadGrid() {
			for (int i = 0; i < gridRows.Length; ++i) {
				gridRows[i].Children.Clear();
			}

			string[] rows = { "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM[" }; // 按键盘顺序存储的三行按键

			// 将每一个字母加入行中，并且把整个interactiveKey加入keys[]数组中
			for (int i = 0; i < 3; ++i) {
				foreach (char c in rows[i]) {
					keys[c - 'A'] = new InteractiveKey(App.keyDatas[c - 'A']);
					if (c == '[') keys[c - 'A'].Enable(false); // 禁止Lambda键响应鼠标
					gridRows[i].Children.Add(keys[c - 'A']);
				}
			}
		}

		private void MinimizeWindow(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

		// 键盘按键的（按下并）抬起，相当于按下了某一按钮
		private new void KeyUpEvent(object sender, KeyEventArgs e) {
			string key = e.Key.ToString(); // 获取按下的按键名称
			if (key.Length == 1) {// 键入单个符号，可能是字母
				char letter = char.Parse(key);
				// 判断是否是字母，若是字母则判断是否是……
				// 立即访问？一次按下模式的按下？二次按下模式的第二次按下？是的话，则执行命令内容
				if (App.InstantAvtice || !App.KeyboardDouble || (letter >= 'A' && letter <= 'Z' && isSameActive))
					Utilities.RunCommand(keys[letter - 'A'].GetCommand());
			}
			else if (key == "LeftShift" || key == "RightShift") {
				ActiveLambdaFunction(false);
			}
		}

		// 键盘的按下，此时将焦点聚焦在一个按钮上，并调整二次访问标记（用于判断是"对打开动作的确认"还是"新切换到一个键"）
		private new void KeyDownEvent(object sender, KeyEventArgs e) {
			string key = e.Key.ToString(); // 获取按下的按键名称
			if (key.Length == 1) {// 键入单个符号，可能是字母
				char letter = char.Parse(key);
				// 判断是否是字母，若是字母，则模拟悬浮该按钮（但不按下）的样式
				if (letter >= 'A' && letter <= 'Z') {
					Keyboard.Focus(keys[letter - 'A'].keyButton);
					isSameActive = currentActivedKey == letter;
					currentActivedKey = letter;
				}
			}
			else if (key == "LeftShift" || key == "RightShift") { // 执行Lambda功能
				ActiveLambdaFunction();
			}
		}

		private void WindowContextMenu(object sender, MouseButtonEventArgs e) {
			ContextMenu contextMenu = new();

			MenuItem settingMenuItem = new() {
				Header = "设置"
			};
			settingMenuItem.Click += LauncherSettings;
			contextMenu.Items.Add(settingMenuItem);

			ContextMenuService.SetContextMenu(this, contextMenu);
		}

		private void LauncherSettings(object sender, RoutedEventArgs e) {
			Setting childWindow = new();
			childWindow.ShowDialog();
		}

		/// <summary>
		/// 启动Lambda功能（部分功能是按下抬起才执行的，另一部分功能是长期的）
		/// </summary>
		/// <param name="start">是否是启动功能的开始阶段</param>
		private void ActiveLambdaFunction(bool start = true) {
			if (start) {
				switch (App.LambdaFunction) {
					case 3:
						// 暂切副策略组并暂开立即响应
						App.InstantAvtice = App.Vice = true;
						ReloadGrid();
						break;

					case 4:
						// 立即响应
						App.InstantAvtice = true;
						break;

					default:
						break;
				}
			}
			else {
				switch (App.LambdaFunction) {
					case 1:
						// 切换日/夜间模式
						App.SwitchDarkMode();
						break;

					case 2:
						// 切换到副策略组
						App.Vice ^= true;
						ReloadGrid();
						break;

					case 3:
						// 暂切副策略组并暂开立即响应
						App.InstantAvtice = App.Vice = false;
						ReloadGrid();
						break;

					case 4:
						// 立即响应
						App.InstantAvtice = false;
						break;

					case 5:
						// 打开设置界面
						Setting childWindow = new();
						childWindow.ShowDialog();
						break;

					default:
						break;
				}
			}
		}

		private void CloseWindow(object sender, RoutedEventArgs e) => Hide();

		private void DragWindow(object sender, MouseButtonEventArgs e) => DragMove();

		private void Menu_OpenWebsite(object sender, System.EventArgs e) => Process.Start("explorer.exe", "https://github.com/Horiz21/lambda-launcher");

		private void Menu_Exit(object sender, System.EventArgs e) {
			notifyIcon.Dispose();
			Application.Current.Shutdown();
		}

		private void Show(object sender, System.EventArgs e) => Show();

		private void ReloadLanguage() {
			notifyIcon.ContextMenuStrip.Items.Add("1", System.Drawing.Image.FromFile("Properties/Images/exit.ico"), Menu_Exit);
			notifyIcon.ContextMenuStrip.Items.Add("2", System.Drawing.Image.FromFile("Properties/Images/link.ico"), Menu_OpenWebsite);
		}
	}
}