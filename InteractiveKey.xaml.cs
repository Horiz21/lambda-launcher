using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LambdaLauncher {
	public partial class InteractiveKey : UserControl {
		private char character;
		private string contentTitle;
		private string contentCommand;
		private string iconSource;
		public InteractiveKey(char character, string contentTitle = "", string contentCommand = "", string iconSource = "") {
			// 为局部变量赋值
			if (character == '^')
				this.character = 'Λ';
			else
				this.character = character;
			this.contentTitle = contentTitle;
			this.contentCommand = contentCommand;
			this.iconSource = iconSource;

			// 初始化窗口
			InitializeComponent();

			// 模拟键盘的“触碰式”突起标记
			if (character == 'F' || character == 'J')
				keyUnderline.Content = "_";

			// 为所有可调整的可显示元素赋值
			keyName.Content = character;
			keyTitle.Content = contentTitle;
			keyIcon.Source = Functions.GetImageFromPath(iconSource);
		}

		private void RuncontentCommand(object sender, RoutedEventArgs e) {
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = "cmd.exe";
			startInfo.Arguments = "/C " + contentCommand;
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;

			Process process = new Process();
			process.StartInfo = startInfo;
			process.Start();
		}

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
			AddProgramWindow addProgramWindow = new AddProgramWindow(character, contentTitle, contentCommand, iconSource);
			addProgramWindow.ShowDialog();
		}

		private void AddcontentCommandMenuItem_Click(object sender, RoutedEventArgs e) {
			//AddcontentCommandWindow addcontentCommandWindow = new AddcontentCommandWindow();
			//addcontentCommandWindow.ShowDialog();
		}
	}
}
