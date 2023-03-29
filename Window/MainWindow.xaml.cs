﻿using LambdaLauncher.Model;
using LambdaLauncher.Utility;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LambdaLauncher {
	public partial class MainWindow : Window {
		private static InteractiveKey[] keys = new InteractiveKey[27]; // 用于存放按钮控件
		private static UniformGrid[] gridRows = new UniformGrid[3]; // 用于存放三个Grid

		private static char currentActivedKey; // 上一次按下的字母
		private static bool isSameActive; // 二次访问标记，是否已经预先按下（致使这是第二次按下）

		public MainWindow() {
			Data.LoadData();

			InitializeComponent();

			// 将三行记录在gridRows数组中
			gridRows[0] = gridRow1;
			gridRows[1] = gridRow2;
			gridRows[2] = gridRow3;

			Refresh();
		}

		public static void Refresh() {
			for (int i = 0; i < gridRows.Length; ++i) {
				gridRows[i].Children.Clear();
			}

			string[] rows = { "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM[" }; // 按键盘顺序存储的三行按键

			// 将每一个字母加入行中，并且把整个interactiveKey加入keys[]数组中
			for (int i = 0; i < 3; ++i) {
				foreach (char c in rows[i]) {
					keys[c - 'A'] = new InteractiveKey(Data.keyDatas[c - 'A']);
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
				if (Data.InstantAvtice || !Data.KeyboardDouble || (letter >= 'A' && letter <= 'Z' && isSameActive))
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
				switch (Data.LambdaFunction) {
					case 3:
						// 暂切副策略组并暂开立即响应
						Data.InstantAvtice = Data.Vice = true;
						Refresh();
						break;
					case 4:
						// 立即响应
						Data.InstantAvtice = true;
						break;
					default:
						break;
				}
			}
			else {
				switch (Data.LambdaFunction) {
					case 1:
						// 切换日/夜间模式
						Data.SwitchDarkMode();
						break;
					case 2:
						// 切换到副策略组
						Data.Vice ^= true;
						Refresh();
						break;
					case 3:
						// 暂切副策略组并暂开立即响应
						Data.InstantAvtice = Data.Vice = false;
						Refresh();
						break;
					case 4:
						// 立即响应
						Data.InstantAvtice = false;
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

		private void CloseWindow(object sender, RoutedEventArgs e) => Close();

		private void DragWindow(object sender, MouseButtonEventArgs e) => DragMove();
	}
}