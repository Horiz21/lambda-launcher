using System.Windows;

namespace LambdaLauncher {
	public partial class AddTarget : Window {
		private InteractiveKey interactiveKey;
		private string title;
		private string command;
		private string iconSource;
		public AddTarget(char letter, string title, string command, string iconSource) {
			// 为局部变量赋值
			this.title = title;
			this.command = command;
			this.iconSource = iconSource;

			// 初始化窗口并添加预览窗口，并且禁止与该预览窗口交互
			InitializeComponent();
			interactiveKey = new InteractiveKey(letter, title, command, iconSource);
			gridInteractiveKey.Children.Add(interactiveKey);
			textTitle.Text = title;
			textIcon.Text = iconSource;
			textTarget.Text = command;
		}

		private void CloseWindow(object sender, RoutedEventArgs e) => Close();

		private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

		// 如果textTitle的内容更新，则实时更新预览窗口的keyTitle
		private void UpdateTitle(object sender, System.Windows.Controls.TextChangedEventArgs e) {
			interactiveKey.keyTitle.Content = this.textTitle.Text;
		}

		// 如果textIcon的内容更新，则尝试更新显示图标
		private void UpdateIcon(object sender, System.Windows.Controls.TextChangedEventArgs e) {
			iconSource=textIcon.Text;
			interactiveKey.keyIcon.Source = Functions.GetImageFromPath(iconSource);
		}

		private void ButtonSelectIcon(object sender, RoutedEventArgs e) {
			var openFileDialog = new Microsoft.Win32.OpenFileDialog() {
				Filter = "Image File|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif"
			};
			if (openFileDialog.ShowDialog() == true) { //需要显式转换为bool类型
				iconSource = openFileDialog.FileName; // 若存在则赋值给局部变量
				interactiveKey.keyIcon.Source = Functions.GetImageFromPath(iconSource);
			}
		}

		private void ButtonSelectTarget(object sender, RoutedEventArgs e) {
			var openFileDialog = new Microsoft.Win32.OpenFileDialog() {
				Filter = "Target|*.*"
			};
			if (openFileDialog.ShowDialog() == true) { //需要显式转换为bool类型
				iconSource = openFileDialog.FileName; // 若存在则赋值给局部变量
			}
		}

		private void ButtonClear(object sender, RoutedEventArgs e) {

		}

		private void ButtonConfirm(object sender, RoutedEventArgs e) {

		}
	}
}
