using LambdaLauncher.Model;
using LambdaLauncher.Utility;
using System.Diagnostics;
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
			ContextMenu contextMenu = new ContextMenu();

			MenuItem modifyKeySettings = new MenuItem();
			modifyKeySettings.Header = "编辑启动程序";
			modifyKeySettings.Click += ModifyKeySettings;
			contextMenu.Items.Add(modifyKeySettings);

			MenuItem clearKeySettings = new MenuItem();
			clearKeySettings.Header = "清空按键内容";
			clearKeySettings.Click += ClearKeySettings;
			contextMenu.Items.Add(clearKeySettings);

			keyButton.ContextMenu = contextMenu;
		}

		private void ModifyKeySettings(object sender, RoutedEventArgs e) {
			KeySettings childWindow = new KeySettings(keyData.Letter);
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
			keyData = Data.keyDatas[keyData.Letter - 'A'];

			// 为所有可调整的可显示元素赋值
			keyLetter.Content = keyData.Letter == '[' ? 'Λ' : keyData.Letter;
			keyTitle.Content = keyData.Title;
			keyIcon.Source = Utilities.GetImageFromPath(keyData.Icon);
		}

		public string GetCommand() => keyData.Command;

		/// <summary>
		/// 清空一个InteractiveKey除了键盘字母外的所有内容
		/// </summary>
		public void Clear() {
			// 1. 清空实际数据   
			keyData.Clear(); // 本地数据
			Data.keyDatas[keyData.Letter - 'A'].Clear(); // 全局数据
			Data.ModifyAndWrite(keyData); // 写回设置

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
			if (Data.InstantAvtice || Data.MouseDouble == false)
				Utilities.RunCommand(keyData.Command);
		}

		/// <summary>
		/// 双击鼠标执行命令
		/// </summary>
		private void DoubleClickToRunContentCommand(object sender, RoutedEventArgs e) {
			if (Data.MouseDouble == true)
				Utilities.RunCommand(keyData.Command);
		}

		public void Enable(bool isEnabled) {
			keyButton.IsEnabled = isEnabled;
		}
	}
}
