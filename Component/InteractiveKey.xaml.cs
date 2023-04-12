using LambdaLauncher.Model;
using LambdaLauncher.Utility;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

			MakeMenu();
			Refresh();
		}

		public void MakeMenu() {
			ContextMenu contextMenu = new();

			MenuItem keySettings1 = new() { Header = (string)Application.Current.FindResource("KeySettings1") };
			keySettings1.Click += KeySettings1;
			contextMenu.Items.Add(keySettings1);

			MenuItem keySettings2 = new() { Header = (string)Application.Current.FindResource("KeySettings2") };
			keySettings2.Click += KeySettings2;
			contextMenu.Items.Add(keySettings2);

			MenuItem clearKey = new() { Header = (string)Application.Current.FindResource("ClearKey") };
			clearKey.Click += ClearKey;
			contextMenu.Items.Add(clearKey);

			keyButton.ContextMenu = contextMenu;
		}

		private void KeySettings1(object sender, RoutedEventArgs e) {
			KeySettings childWindow = new(keyData.Letter);
			childWindow.ShowDialog();
			Refresh();
		}

		private void KeySettings2(object sender, RoutedEventArgs e) {
			KeySettings childWindow = new(keyData.Letter, true);
			childWindow.ShowDialog();
			Refresh();
		}

		/// <summary>
		/// 清空一个InteractiveKey除了键盘字母外的所有内容（数据和显示）
		/// </summary>
		private void ClearKey(object sender, RoutedEventArgs e) {
			// 1. 清空实际数据
			keyData.Clear(); // 本地数据
			App.config.keyDatas[keyData.Letter - 'A'].Clear(); // 全局数据
			App.config.ModifyAndWrite(keyData); // 写回设置

			// 2. 清空显示内容
			ClearContent();
		}

		/// <summary>
		/// 保存设置的内容并刷新显示
		/// </summary>
		private void Refresh() {
			// 更新局部keyData信息
			keyData = App.config.keyDatas[keyData.Letter - 'A'];

			// 显示字母
			keyLetter.Content = keyData.Letter == '[' ? 'Λ' : keyData.Letter;

			// 为其他可调整的可显示元素赋值
			if (App.config.Vice) {
				keyTitle.Content = keyData.ViceTitle;
				keyIcon.Source = Utilities.GetImageFromPath(keyData.ViceIcon);
			}
			else {
				keyTitle.Content = keyData.Title;
				keyIcon.Source = Utilities.GetImageFromPath(keyData.Icon);
			}
		}

		public string GetCommand() => App.config.Vice ? keyData.ViceCommand : keyData.Command;

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
			if (App.config.InstantAvtice || App.config.MouseDouble == false)
				if (App.config.Vice) Utilities.RunCommand(keyData.ViceCommand);
				else Utilities.RunCommand(keyData.Command);
		}

		/// <summary>
		/// 双击鼠标执行命令
		/// </summary>
		private void DoubleClickToRunContentCommand(object sender, RoutedEventArgs e) {
			if (App.config.MouseDouble == true)
				if (App.config.Vice) Utilities.RunCommand(keyData.ViceCommand);
				else Utilities.RunCommand(keyData.Command);
		}

		public void Enable(bool isEnabled) {
			keyButton.IsEnabled = isEnabled;
		}
	}
}