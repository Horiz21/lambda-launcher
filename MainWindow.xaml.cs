using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LambdaLauncher {
	public partial class MainWindow : Window {
		struct KeyData {
			public char letter;
			public string title;
			public string command;
			public string icon;
		}

		private KeyData[] keyDatas = new KeyData[27]; // 用于存放字母完全信息
		private InteractiveKey[] keys = new InteractiveKey[27]; // 用于存放按钮空间
		private UniformGrid[] gridRows = new UniformGrid[3]; // 用于存放三个Grid

		private char last_active_key; // 上一次按下的字母
		private bool last_active_same; // 是否已经预先按下（致使这是第二次按下）

		private void ReadCsvData(string filePath) {
			using (var reader = new StreamReader(filePath)) {
				reader.ReadLine(); //跳过首行（首行是用于控制格式）
				while (!reader.EndOfStream) {
					// 根据逗号，分割出四个子串
					string[] line = reader.ReadLine().Split(',');

					// 根据子串新建keyData
					char letter = char.Parse(line[0]);
					keyDatas[letter - 'A'] = new KeyData {
						letter = char.Parse(line[0]),
						title = line[1],
						command = line[2],
						icon = line[3]
					};
				}
			}
		}
		public MainWindow() {
			string[] rows = { "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM[" };
			ReadCsvData("C:/Users/Frankie/source/repos/LambdaLauncher/resource/testfile.csv");

			InitializeComponent();

			// 将三行记录在gridRows数组中
			gridRows[0] = gridRow1;
			gridRows[1] = gridRow2;
			gridRows[2] = gridRow3;

			// 将每一个字母加入行中，并且把整个interactiveKey加入keys[]数组中
			for (int i = 0; i < 3; ++i) {
				foreach (char c in rows[i]) {
					keys[c - 'A'] = new InteractiveKey(c, keyDatas[c - 'A'].title, keyDatas[c - 'A'].command, keyDatas[c - 'A'].icon);
					gridRows[i].Children.Add(keys[c - 'A']);
				}
			}
		}

		private void CloseWindow(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void DragWindow(object sender, MouseButtonEventArgs e) {
			this.DragMove();
		}

		private void MinimizeWindow(object sender, RoutedEventArgs e) {
			this.WindowState = WindowState.Minimized;
		}

		// 键盘按键的（按下并）抬起，相当于按下了某一按钮
		private new void KeyUpEvent(object sender, KeyEventArgs e) {
			string key = e.Key.ToString(); // 获取按下的按键名称
			if (key.Length == 1) {// 键入单个符号，可能是字母
				char letter = char.Parse(key);
				// 判断是否是字母，若是字母则判断是否是第二次按下（确认），若是则执行命令内容
				if (letter >= 'A' && letter <= 'Z' && last_active_same) {
					Functions.RunCommand(keys[letter - 'A'].contentCommand);
				}
			}
		}

		private new void KeyDownEvent(object sender, KeyEventArgs e) {
			string key = e.Key.ToString(); // 获取按下的按键名称
			if (key.Length == 1) {// 键入单个符号，可能是字母
				char letter = char.Parse(key);
				// 判断是否是字母，若是字母，则模拟悬浮该按钮（但不按下）的样式
				if (letter >= 'A' && letter <= 'Z') {
					Keyboard.Focus(keys[letter - 'A'].keyButton);
					last_active_same = last_active_key == letter;
					last_active_key = letter;
				}
			}
		}
	}
}
