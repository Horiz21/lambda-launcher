using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LambdaLauncher {
	public partial class InteractiveKey : UserControl {
		private char letter;
		private string title;
		public string command; // 命令可以被外部获取
		private string iconSource;
		public InteractiveKey(char letter, string title = "", string command = "", string iconSource = "") {
			// 为局部变量赋值：特判，如果是'['（即ASCII码为'Z'+1的）就转换成符号Λ
			this.letter = letter == '['? 'Λ':letter;
			this.title = title;
			this.command = command;
			this.iconSource = iconSource;

			// 初始化窗口
			InitializeComponent();

			// 模拟键盘的“触碰式”突起标记
			if (letter == 'F' || letter == 'J')
				keyUnderline.Content = "_";

			// 为所有可调整的可显示元素赋值
			keyName.Content = letter;
			keyTitle.Content = title;
			keyIcon.Source = Functions.GetImageFromPath(iconSource);
		}

		private void RuncontentCommand(object sender, RoutedEventArgs e) => Functions.RunCommand(command);

		private void keyButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			ContextMenu contextMenu = new ContextMenu();

			MenuItem addProgramMenuItem = new MenuItem();
			addProgramMenuItem.Header = "编辑启动程序";
			addProgramMenuItem.Click += AddProgramMenuItem_Click;
			contextMenu.Items.Add(addProgramMenuItem);

			MenuItem addcontentCommandMenuItem = new MenuItem();
			addcontentCommandMenuItem.Header = "编辑执行命令";
			addcontentCommandMenuItem.Click += AddcontentCommandMenuItem_Click;
			contextMenu.Items.Add(addcontentCommandMenuItem);

			keyButton.ContextMenu = contextMenu;
		}

		private void AddProgramMenuItem_Click(object sender, RoutedEventArgs e) {
			AddTarget childWindow = new AddTarget(letter, title, command, iconSource);
			childWindow.ShowDialog();
		}

		private void AddcontentCommandMenuItem_Click(object sender, RoutedEventArgs e) {
			//AddcontentCommandWindow addcontentCommandWindow = new AddcontentCommandWindow();
			//addcontentCommandWindow.ShowDialog();
		}
	}
}
