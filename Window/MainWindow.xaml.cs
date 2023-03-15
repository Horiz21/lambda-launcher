using LambdaLauncher.Model;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LambdaLauncher {
	public partial class MainWindow : Window {
		private KeyData[] keyDatas = new KeyData[27]; // 用于存放27个字母信息
		private InteractiveKey[] keys = new InteractiveKey[27]; // 用于存放按钮控件
		private UniformGrid[] gridRows = new UniformGrid[3]; // 用于存放三个Grid

		private char currentActivedKey; // 上一次按下的字母
		private bool isSameActive; // 二次访问标记，是否已经预先按下（致使这是第二次按下）

		private void ReadCsvData(string filePath) {
			using (var reader = new StreamReader(filePath)) {
				reader.ReadLine(); //跳过首行（首行是用于控制格式）
				while (!reader.EndOfStream) {
					// 根据逗号，分割出四个子串
					string[] line = reader.ReadLine().Split(',');

					// 根据子串新建keyData
					char letter = char.Parse(line[0]);
					keyDatas[letter - 'A'] = new KeyData {
						Letter = char.Parse(line[0]),
						Title = line[1],
						Command = line[2],
						Icon = line[3]
					};
				}
			}
		}
		public MainWindow() {
			string[] rows = { "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM[" }; // 按键盘顺序存储的三行按键
			ReadCsvData("C:/Users/Frankie/source/repos/LambdaLauncher/resource/testfile.csv");

			InitializeComponent();

			// 将三行记录在gridRows数组中
			gridRows[0] = gridRow1;
			gridRows[1] = gridRow2;
			gridRows[2] = gridRow3;

			// 将每一个字母加入行中，并且把整个interactiveKey加入keys[]数组中
			for (int i = 0; i < 3; ++i) {
				foreach (char c in rows[i]) {
					keys[c - 'A'] = new InteractiveKey(keyDatas[c - 'A']);
					gridRows[i].Children.Add(keys[c - 'A']);
				}
			}
		}

		private void CloseWindow(object sender, RoutedEventArgs e) => Close();

		private void DragWindow(object sender, MouseButtonEventArgs e) => DragMove();

		private void MinimizeWindow(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

		// 键盘按键的（按下并）抬起，相当于按下了某一按钮
		private new void KeyUpEvent(object sender, KeyEventArgs e) {
			string key = e.Key.ToString(); // 获取按下的按键名称
			if (key.Length == 1) {// 键入单个符号，可能是字母
				char letter = char.Parse(key);
				// 判断是否是字母，若是字母则判断是否是第二次按下（确认），若是则执行命令内容
				if (letter >= 'A' && letter <= 'Z' && isSameActive) {
					Utilities.RunCommand(keys[letter - 'A'].GetCommand());
				}
			}
		}

		// 键盘的按下，此时将焦点聚焦在一个按钮上，并调整二次访问标记（用于判断是"对打开动作的确认"还是"新切换到一个键"）
		private new void KeyDownEvent(object sender, KeyEventArgs e) {
			string key = e.Key.ToString(); // 获取按下的按键名称
			if (key.Length == 1) {// 键入单个符号，可能是字母
				char letter = char.Parse(key);
				// 判断是否是字母，若是字母，则模拟悬浮该按钮（但不按下）的样式
				if (letter >= 'A' && letter <= 'Z') {
					Keyboard.Focus(keys[letter - 'A'].keyButton);
					isSameActive = currentActivedKey == letter;
					currentActivedKey = letter;
				}
			}
		}
	}
}
