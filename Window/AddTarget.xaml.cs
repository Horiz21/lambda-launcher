using LambdaLauncher.Model;
using LambdaLauncher.Utility;
using System.Windows;

namespace LambdaLauncher {
	public partial class AddTarget : Window {
		private InteractiveKey interactiveKey;
		private KeyData keyData; // 本地KeyData变量
		public AddTarget(KeyData keyData) {
			// 为局部变量赋值
			this.keyData = keyData;

			// 初始化窗口并添加预览窗口，并且禁止与该预览窗口交互
			InitializeComponent();
			interactiveKey = new InteractiveKey(keyData);
			interactiveKey.keyButton.IsEnabled = false;
			gridInteractiveKey.Children.Add(interactiveKey);

			// 向输入框插入目前已有信息
			textTitle.Text = keyData.Title;
			textIcon.Text = keyData.Icon;
			textTarget.Text = keyData.Command;
		}

		private void CloseWindow(object sender, RoutedEventArgs e) => Close();

		private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

		// 如果textTitle的内容更新，则实时更新预览窗口的keyTitle
		private void UpdateTitle(object sender, System.Windows.Controls.TextChangedEventArgs e) {
			interactiveKey.keyTitle.Content = this.textTitle.Text;
		}

		// 如果textIcon的内容更新，则尝试更新显示图标
		private void UpdateIcon(object sender, System.Windows.Controls.TextChangedEventArgs e) {
			keyData.Icon = textIcon.Text;
			interactiveKey.keyIcon.Source = Utilities.GetImageFromPath(keyData.Icon);
		}

		private void ButtonSelectIcon(object sender, RoutedEventArgs e) {
			var openFileDialog = new Microsoft.Win32.OpenFileDialog() {
				Filter = "Image File|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif"
			};
			if (openFileDialog.ShowDialog() == true) { //需要显式转换为bool类型
				keyData.Icon = openFileDialog.FileName; // 若存在则赋值给局部变量
				interactiveKey.keyIcon.Source = Utilities.GetImageFromPath(keyData.Icon);
			}
		}

		private void ButtonSelectTarget(object sender, RoutedEventArgs e) {
			var openFileDialog = new Microsoft.Win32.OpenFileDialog() {
				Filter = "Target|*.*"
			};
			if (openFileDialog.ShowDialog() == true) { //需要显式转换为bool类型
				keyData.Command = openFileDialog.FileName; // 若存在则赋值给局部变量
			}
		}

		private void ButtonClear(object sender, RoutedEventArgs e) {
			 
		}

		private void ButtonConfirm(object sender, RoutedEventArgs e) {

		}
	}
}
