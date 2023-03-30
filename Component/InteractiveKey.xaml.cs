﻿using LambdaLauncher.Model;
using LambdaLauncher.Utility;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LambdaLauncher {

	public partial class InteractiveKey : UserControl {
		private KeyData keyData;

		public InteractiveKey(KeyData keyData) {
			// 为局部变量赋值
			this.keyData = keyData;

			// 初始化窗口
			InitializeComponent();

			// 模拟键盘的“触碰式”突起标记
			if (keyData.Letter == 'F' || keyData.Letter == 'J')
				keyUnderline.Content = "_";

			Refresh();
		}

		private void MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			ContextMenu contextMenu = new();

			MenuItem modifyKeySettings = new() {
				Header = "编辑主按键策略"
			};
			modifyKeySettings.Click += ModifyKeySettings;
			contextMenu.Items.Add(modifyKeySettings);

			MenuItem modifyViceKeySettings = new() {
				Header = "编辑副按键策略"
			};
			modifyViceKeySettings.Click += ModifyViceKeySettings;
			contextMenu.Items.Add(modifyViceKeySettings);

			MenuItem clearKeySettings = new() {
				Header = "清空按键内容"
			};
			clearKeySettings.Click += ClearKeySettings;
			contextMenu.Items.Add(clearKeySettings);

			keyButton.ContextMenu = contextMenu;
		}

		private void ModifyKeySettings(object sender, RoutedEventArgs e) {
			KeySettings childWindow = new(keyData.Letter);
			childWindow.ShowDialog();
			Refresh();
		}

		private void ModifyViceKeySettings(object sender, RoutedEventArgs e) {
			KeySettings childWindow = new(keyData.Letter, true);
			childWindow.ShowDialog();
			Refresh();
		}

		/// <summary>
		/// 清空数据并清空按键显示内容
		/// </summary>
		private void ClearKeySettings(object sender, RoutedEventArgs e) => Clear();

		/// <summary>
		/// 保存设置的内容并刷新显示
		/// </summary>
		private void Refresh() {
			// 更新局部keyData信息
			keyData = App.KeyDatas[keyData.Letter - 'A'];

			// 显示字母
			keyLetter.Content = keyData.Letter == '[' ? 'Λ' : keyData.Letter;

			// 为其他可调整的可显示元素赋值
			if (App.Vice) {
				keyTitle.Content = keyData.ViceTitle;
				//if (keyData.Letter == '[')
				//	keyIcon.Source = Utilities.GetLogo();
				//else
				keyIcon.Source = Utilities.GetImageFromPath(keyData.ViceIcon);
			}
			else {
				keyTitle.Content = keyData.Title;
				//if (keyData.Letter == '[')
				//	keyIcon.Source = Utilities.GetLogo();
				//else
				keyIcon.Source = Utilities.GetImageFromPath(keyData.Icon);
			}
		}

		public string GetCommand() => App.Vice ? keyData.ViceCommand : keyData.Command;

		/// <summary>
		/// 清空一个InteractiveKey除了键盘字母外的所有内容
		/// </summary>
		public void Clear() {
			// 1. 清空实际数据
			keyData.Clear(); // 本地数据
			App.KeyDatas[keyData.Letter - 'A'].Clear(); // 全局数据
			App.ModifyAndWrite(keyData); // 写回设置

			// 2. 清空显示内容
			ClearContent();
		}

		/// <summary>
		/// 清空一个InteractiveKey的显示内容
		/// </summary>
		public void ClearContent() {
			keyTitle.Content = string.Empty;
			keyIcon.Source = Utilities.GetEmptyImage();
		}

		/// <summary>
		/// 单击鼠标执行命令
		/// </summary>
		private void SingleClickToRunContentCommand(object sender, RoutedEventArgs e) {
			if (App.InstantAvtice || App.MouseDouble == false)
				if (App.Vice) Utilities.RunCommand(keyData.ViceCommand);
				else Utilities.RunCommand(keyData.Command);
		}

		/// <summary>
		/// 双击鼠标执行命令
		/// </summary>
		private void DoubleClickToRunContentCommand(object sender, RoutedEventArgs e) {
			if (App.MouseDouble == true)
				if (App.Vice) Utilities.RunCommand(keyData.ViceCommand);
				else Utilities.RunCommand(keyData.Command);
		}

		public void Enable(bool isEnabled) {
			keyButton.IsEnabled = isEnabled;
		}
	}
}