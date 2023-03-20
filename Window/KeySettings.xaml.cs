using LambdaLauncher.Model;
using LambdaLauncher.Utility;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LambdaLauncher {
	public partial class KeySettings : Window {
		private InteractiveKey interactiveKey;
		private KeyData keyData; // 本地KeyData变量
		private int localLinkType = 0;
		public KeySettings(char Letter) {
			// 为局部变量赋值
			this.keyData = Data.keyDatas[Letter - 'A'];

			// 初始化窗口并添加预览窗口，并且禁止与该预览窗口交互
			InitializeComponent();
			interactiveKey = new InteractiveKey(keyData);
			interactiveKey.keyButton.IsEnabled = false;
			gridInteractiveKey.Children.Add(interactiveKey);

			// 向输入框插入目前已有信息
			textTitle.Text = keyData.Title;
			textIcon.Text = keyData.Icon;
			textLink.Text = keyData.Command;
			localLinkType = keyData.LinkType;

			if (localLinkType == 1) {
				AddTarget.IsChecked = true;
			}
			else if (localLinkType == 2) {
				AddFolder.IsChecked = true;
			}
			else if (localLinkType == 3) {
				AddWebsite.IsChecked = true;
			}
			else if (localLinkType == 4) {
				PureCommand.IsChecked = true;
			}
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
				textIcon.Text = openFileDialog.FileName; // 若存在则赋值给局部变量
				interactiveKey.keyIcon.Source = Utilities.GetImageFromPath(keyData.Icon);
			}
		}

		private void ButtonSelectTarget(object sender, RoutedEventArgs e) {
			var openFileDialog = new Microsoft.Win32.OpenFileDialog() {
				Filter = "Target|*.*"
			};
			if (openFileDialog.ShowDialog() == true) { //需要显式转换为bool类型
				textLink.Text = openFileDialog.FileName; // 若存在则赋值给局部变量
			}
		}

		private void ButtonClear(object sender, RoutedEventArgs e) {
			// 清空信息
			textTitle.Clear();
			textIcon.Clear();
			textLink.Clear();

			// 取消选择
			AddTarget.IsChecked = false;
			AddFolder.IsChecked = false;
			AddWebsite.IsChecked = false;
			PureCommand.IsChecked = false;
			localLinkType = 0;

			// 清空左侧显示区
			interactiveKey.Clear();
		}

		private void ButtonConfirm(object sender, RoutedEventArgs e) {
			// 根据填充项修改信息
			keyData.LinkType = localLinkType;
			keyData.Title = textTitle.Text;
			keyData.Command = textLink.Text;
			keyData.Icon = textIcon.Text;

			// 将自己修改后的信息保存回csv文件
			Data.ModifyAndWrite(keyData);

			// 关闭当前页面
			Close();
		}

		private void CheckedAddTarget(object sender, RoutedEventArgs e) {
			labelLink.SetResourceReference(ContentProperty, "AddTarget");
			textLink.SetValue(Grid.ColumnSpanProperty, 1);
			linkButton.Visibility = Visibility.Visible;
			localLinkType = 1;
		}

		private void CheckedAddFolder(object sender, RoutedEventArgs e) {
			labelLink.SetResourceReference(ContentProperty, "AddFolder");
			textLink.SetValue(Grid.ColumnSpanProperty, 1);
			linkButton.Visibility = Visibility.Visible;
			localLinkType = 2;
		}

		private void CheckedAddWebsite(object sender, RoutedEventArgs e) {
			labelLink.SetResourceReference(ContentProperty, "AddWebsite");
			textLink.SetValue(Grid.ColumnSpanProperty, 2);
			linkButton.Visibility = Visibility.Collapsed;
			localLinkType = 3;
		}

		private void CheckedPureCommand(object sender, RoutedEventArgs e) {
			labelLink.SetResourceReference(ContentProperty, "PureCommand");
			textLink.SetValue(Grid.ColumnSpanProperty, 2);
			linkButton.Visibility = Visibility.Collapsed;
			localLinkType = 4;
		}
	}
}
