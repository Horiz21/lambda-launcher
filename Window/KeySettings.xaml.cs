using LambdaLauncher.Model;
using LambdaLauncher.Utility;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace LambdaLauncher {
	public partial class KeySettings : Window {
		private InteractiveKey interactiveKey;
		private KeyData keyData; // 本地KeyData变量
		private char Letter;
		private int localLinkType = 0;
		private string localLink = string.Empty; // 暂时记录原始的链接，使得标签切换走、切换回来该链接仍然暂存着
		private bool viceMode; // 是否是一个副策略组的实例

		public KeySettings(char Letter, bool viceMode = false) {
			// 为局部变量赋值
			this.Letter = Letter;
			keyData = Data.keyDatas[Letter - 'A'].DeepCopy();
			this.viceMode = viceMode;
			if (viceMode) {
				localLink = keyData.ViceCommand;
				localLinkType = keyData.ViceLinkType;
			}
			else {
				localLink = keyData.Command;
				localLinkType = keyData.LinkType;
			}

			// 初始化窗口并添加预览窗口，并且禁止与该预览窗口交互
			InitializeComponent();

			interactiveKey = new InteractiveKey(keyData);
			interactiveKey.ClearContent();
			interactiveKey.Enable(false);
			gridInteractiveKey.Children.Add(interactiveKey);

			// 向输入框插入目前已有信息
			if (viceMode) {
				textIcon.Text = keyData.ViceIcon;
				textLink.Text = keyData.ViceCommand;
				textTitle.Text = keyData.ViceTitle;
			}
			else {
				textIcon.Text = keyData.Icon;
				textLink.Text = keyData.Command;
				textTitle.Text = keyData.Title;
			}

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
		private void UpdateTitle(object sender, TextChangedEventArgs e) {
			interactiveKey.keyTitle.Content = textTitle.Text;
		}

		// 如果textIcon的内容更新，则尝试更新显示图标
		private void UpdateIcon(object sender, TextChangedEventArgs e) {
			keyData.Icon = textIcon.Text;
			interactiveKey.keyIcon.Source = Utilities.GetImageFromPath(keyData.Icon);
		}

		private void ButtonSelectIcon(object sender, RoutedEventArgs e) {
			OpenFileDialog? openFileDialog = new OpenFileDialog() {
				Filter = "Image File|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif"
			};
			if (openFileDialog.ShowDialog() == true) { //需要显式转换为bool类型
				textIcon.Text = openFileDialog.FileName; // 若存在则赋值给局部变量
				interactiveKey.keyIcon.Source = Utilities.GetImageFromPath(keyData.Icon);
			}
		}

		private void ButtonSelectTarget(object sender, RoutedEventArgs e) {
			if (AddTarget.IsChecked == true) {
				OpenFileDialog? openFileDialog = new OpenFileDialog() {
					Filter = "Target|*.*"
				};
				if (openFileDialog.ShowDialog() == true) { // 需要显式转换为bool类型
					textLink.Text = openFileDialog.FileName; // 若存在则赋值给局部变量
				}
			}
			else if (AddFolder.IsChecked == true) { // 此处使用WinForm内容
				var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
				if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					textLink.Text = folderBrowserDialog.SelectedPath;
				}
			}
		}

		private void ButtonClear(object sender, RoutedEventArgs e) {
			// 清空本地keyData的信息
			keyData.Clear();

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
			interactiveKey.ClearContent();
		}

		private void ButtonConfirm(object sender, RoutedEventArgs e) {
			// 根据填充项修改信息
			if (viceMode) {
				keyData.ViceLinkType = localLinkType;
				keyData.ViceTitle = textTitle.Text;
				keyData.ViceCommand = textLink.Text;
				keyData.ViceIcon = textIcon.Text;
			}
			else {
				keyData.LinkType = localLinkType;
				keyData.Title = textTitle.Text;
				keyData.Command = textLink.Text;
				keyData.Icon = textIcon.Text;
			}

			// 将自己修改后的信息保存回csv文件
			Data.ModifyAndWrite(keyData);
			Data.keyDatas[Letter - 'A'] = keyData.DeepCopy();

			// 关闭当前页面
			Close();
		}

		private void CheckedAddTarget(object sender, RoutedEventArgs e) {
			ChangeTextLinkAttribute("AddTarget", 1, true);
		}

		private void CheckedAddFolder(object sender, RoutedEventArgs e) {
			ChangeTextLinkAttribute("AddFolder", 2, true);
		}

		private void CheckedAddWebsite(object sender, RoutedEventArgs e) {
			ChangeTextLinkAttribute("AddWebsite", 3,false);
		}

		private void CheckedPureCommand(object sender, RoutedEventArgs e) {
			ChangeTextLinkAttribute("PureCommand", 4,false);
		}

		private void ChangeTextLinkAttribute(string reference, int linkType, bool isLongTextBox) {
			Debug.WriteLine("检测到选项卡切换到" + linkType.ToString());
			labelLink.SetResourceReference(ContentProperty, reference);
			if (isLongTextBox) {
				textLink.SetValue(Grid.ColumnSpanProperty, 1);
				linkButton.Visibility = Visibility.Visible;
			}
			else {
				textLink.SetValue(Grid.ColumnSpanProperty, 2);
				linkButton.Visibility = Visibility.Collapsed;
			}
			// 如果目前的类型和历史记录类型一致，则恢复该历史记录
			if (linkType == keyData.LinkType)
				textLink.Text = localLink;
			else
				textLink.Text = string.Empty;
			localLinkType = linkType;
			NowInfoWriteLine();
		}
		
		private void NowInfoWriteLine() {
			Debug.WriteLine("现在是选项卡" + localLinkType.ToString() + "，有记录的是选项卡"+keyData.LinkType.ToString()+"，内容为："+localLink);
		}
	}
}
