using LambdaLauncher.Utility;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using Forms = System.Windows.Forms;

namespace LambdaLauncher {

	public partial class MainWindow : Window {
		private static InteractiveKey[] keys = new InteractiveKey[27]; // 用于存放按钮控件
		private static UniformGrid[] gridRows = new UniformGrid[3]; // 用于存放三个Grid

		private Forms.NotifyIcon notifyIcon;
		private static char currentActivedKey; // 上一次按下的字母
		private static bool isSameActive; // 二次访问标记，是否已经预先按下（致使这是第二次按下）
		private HwndSource source;

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

			ReloadLanguage();  // 加载语言文件
			ReloadGrid();  // 加载布局

			Loaded += HotkeyAfterLoad;  // 注册热键
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
					gridRows[i].Children.Add(keys[c - 'A']);
				}
			}

			keys[26].Enable(false); // 禁止Lambda键响应鼠标
		}

		#region 键盘事件（按键响应、Lambda功能）

		/// <summary>
		/// 键盘的按下，此时将焦点聚焦在一个按钮上，并调整二次访问标记（用于判断是"对打开动作的确认"还是"新切换到一个键"）
		/// </summary>
		private new void KeyDownEvent(object sender, KeyEventArgs e) {
			string key = e.Key.ToString(); // 获取按下的按键名称
			if (key.Length == 1) {// 键入单个符号，可能是字母
				char letter = char.Parse(key);
				// 判断是否是字母，若是字母，则模拟悬浮该按钮（但不按下）的样式
				if (letter >= 'A' && letter <= 'Z') {
					keys[letter - 'A'].keyButton.Focus();
					isSameActive = currentActivedKey == letter;
					currentActivedKey = letter;
				}
			}
			else if (key == "LeftShift" || key == "RightShift") { // 执行Lambda功能
				ActiveLambdaFunction();
			}
		}

		/// <summary>
		/// 键盘按键的（按下并）抬起，相当于按下了某一按钮
		/// </summary>
		private new void KeyUpEvent(object sender, KeyEventArgs e) {
			string key = e.Key.ToString(); // 获取按下的按键名称
			if (key.Length == 1) {// 键入单个符号，可能是字母
				char letter = char.Parse(key);
				// 判断是否是字母，若是字母则判断是否是……
				// 立即访问？一次按下模式的按下？二次按下模式的第二次按下？是的话，则执行命令内容
				if (App.InstantAvtice || !App.KeyboardDouble || (letter >= 'A' && letter <= 'Z' && isSameActive)) {
					currentActivedKey = '\0';
					isSameActive = false; // 删除激活信息，这样下次打开界面的时候就不会有残留信息
					Utilities.RunCommand(keys[letter - 'A'].GetCommand());
				}
			}
			else if (key == "LeftShift" || key == "RightShift") {
				ActiveLambdaFunction(false);
			}
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

		#endregion 键盘事件（按键响应、Lambda功能）

		#region 窗口显示/隐藏/拖动/最小化

		private void Show(object sender, EventArgs e) => Show();

		private void Hide(object sender, RoutedEventArgs e) => Hide();

		private void DragWindow(object sender, MouseButtonEventArgs e) => DragMove();

		private void MinimizeWindow(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

		private void Activate(object sender, EventArgs e) => ReloadGrid();  // HACK

		#endregion 窗口显示/隐藏/拖动/最小化

		#region 上下文菜单语言设置

		private void MenuWebsite(object sender, EventArgs e) {
			Process.Start("explorer.exe", "https://github.com/Horiz21/lambda-launcher");
		}

		private void MenuExit(object sender, EventArgs e) {
			notifyIcon.Dispose();
			UnregisterHotKey(1134419766);
			App.Current.Shutdown();
		}

		private void LauncherSettings(object sender, RoutedEventArgs e) {
			Setting childWindow = new();
			childWindow.ShowDialog();
		}

		#endregion 上下文菜单语言设置

		#region 语言设置

		public void ReloadLanguage() {
			// 1. 任务栏语言设置
			notifyIcon.ContextMenuStrip.Items.Clear();
			notifyIcon.ContextMenuStrip.Items.Add((string)Application.Current.FindResource("MenuWebsite"), System.Drawing.Image.FromFile("Properties/Images/link.ico"), MenuWebsite);
			notifyIcon.ContextMenuStrip.Items.Add((string)Application.Current.FindResource("MenuExit"), System.Drawing.Image.FromFile("Properties/Images/exit.ico"), MenuExit);

			// 2. 主界面右键菜单语言设置
			ContextMenu contextMenu = new();
			MenuItem settingMenuItem = new() { Header = (string)Application.Current.FindResource("Settings") };
			settingMenuItem.Click += LauncherSettings;
			contextMenu.Items.Add(settingMenuItem);
			this.ContextMenu = contextMenu;

			// 3. 按键右键菜单语言设置
			foreach (var key in keys) {
				if (key != null) key.MakeMenu();
			}
		}

		#endregion 语言设置

		#region 热键设置

		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		public void HotkeyAfterLoad(object sender, RoutedEventArgs e) {
			if (!Hotkey(sender, e)) { // 快捷键被占用时抛出提示
				MessageBox.Show((string)Application.Current.FindResource("HotkeyConflictErrorTip1"), (string)Application.Current.FindResource("HotkeyConflictError"));
			}
		}

		public bool Hotkey(object sender, RoutedEventArgs e) {
			string[] parts = App.Hotkey.Split('+');  // 以“+”为界分割快捷键的每个部分
			ModifierKeys modifier = ModifierKeys.None;
			if (parts.Contains("Ctrl")) modifier |= ModifierKeys.Control;
			if (parts.Contains("Alt")) modifier |= ModifierKeys.Alt;
			if (parts.Contains("Shift")) modifier |= ModifierKeys.Shift;
			if (parts.Contains("Win")) modifier |= ModifierKeys.Windows;
			string actualKey = parts.Last();  // 排除修饰键以外的就是实键

			// 注册热键 (热键ID,修饰键,实键)
			bool success = RegisterHotKey(1134419766, (Key)Enum.Parse(typeof(Key), actualKey), modifier);

			// 获取窗口句柄，创建HwndSource实例
			source = HwndSource.FromHwnd(GetHandle(this));
			source.AddHook(new HwndSourceHook(WndProc));

			// 返回是否成功注册
			return success;
		}

		// 获取窗口句柄
		public static IntPtr GetHandle(Window window) => new WindowInteropHelper(window).Handle;

		// 注册热键
		public bool RegisterHotKey(int id, Key key, ModifierKeys modifiers) {
			// 将Key转换为虚拟键码
			uint vk = (uint)KeyInterop.VirtualKeyFromKey(key);

			// 将ModifierKeys转换为辅助键的值
			uint fsModifiers = 0;
			if ((modifiers & ModifierKeys.Alt) != 0) fsModifiers |= 0x0001;
			if ((modifiers & ModifierKeys.Control) != 0) fsModifiers |= 0x0002;
			if ((modifiers & ModifierKeys.Shift) != 0) fsModifiers |= 0x0004;
			if ((modifiers & ModifierKeys.Windows) != 0) fsModifiers |= 0x0008;

			// 注册热键
			return RegisterHotKey(GetHandle(this), id, fsModifiers, vk);
		}

		// 注销热键
		public void UnregisterHotKey(int id) => UnregisterHotKey(GetHandle(this), id);

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			switch (msg) {
				case 0x0312:  // WM_HOTKEY
					if (wParam.ToInt32() == 1134419766) {
						if (IsVisible) Hide();
						else Show();
					}
					handled = true;
					break;
			}
			return IntPtr.Zero;
		}

		#endregion 热键设置


	}
}