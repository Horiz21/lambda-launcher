using LambdaLauncher.Model;
using LambdaLauncher.Utility;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LambdaLauncher {
	public partial class InteractiveKey : UserControl {
		private KeyData keyData;
		public InteractiveKey(KeyData keyData) {
			// 为局部变量赋值：特判，如果是'['（即ASCII码为'Z'+1的）就转换成符号Λ
			this.keyData = keyData;
			if (this.keyData.Letter == '[')
				this.keyData.Letter = 'Λ';

			// 初始化窗口
			InitializeComponent();

			// 模拟键盘的“触碰式”突起标记
			if (keyData.Letter == 'F' || keyData.Letter == 'J')
				keyUnderline.Content = "_";

			// 为所有可调整的可显示元素赋值
			keyLetter.Content = keyData.Letter;
			keyTitle.Content = keyData.Title;
			keyIcon.Source = Utilities.GetImageFromPath(keyData.Icon);
		}

		private void RuncontentCommand(object sender, RoutedEventArgs e) => Utilities.RunCommand(keyData.Command);

		private void keyButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			ContextMenu contextMenu = new ContextMenu();

			MenuItem addProgramMenuItem = new MenuItem();
			addProgramMenuItem.Header = "编辑启动程序";
			addProgramMenuItem.Click += AddTarget;
			contextMenu.Items.Add(addProgramMenuItem);

			MenuItem addcontentCommandMenuItem = new MenuItem();
			addcontentCommandMenuItem.Header = "编辑执行命令";
			addcontentCommandMenuItem.Click += AddcontentCommandMenuItem_Click;
			contextMenu.Items.Add(addcontentCommandMenuItem);

			keyButton.ContextMenu = contextMenu;
		}

		private void AddTarget(object sender, RoutedEventArgs e) {
			AddTarget childWindow = new AddTarget(keyData);
			childWindow.ShowDialog();
		}

		private void AddcontentCommandMenuItem_Click(object sender, RoutedEventArgs e) {
			//AddcontentCommandWindow addcontentCommandWindow = new AddcontentCommandWindow();
			//addcontentCommandWindow.ShowDialog();
		}

		public string GetCommand() => keyData.Command;
	}
}
