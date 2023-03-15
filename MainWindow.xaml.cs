using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace LambdaLauncher {
	struct KeyData {
		public char Character;
		public string keyTitle;
		public string contentCommand;
		public string ImgPath;
	}

	public partial class MainWindow : Window {
		private Dictionary<char, KeyData> keyDataDictionary = new Dictionary<char, KeyData>();
		private void ReadCsvData(string filePath) {
			using (var reader = new StreamReader(filePath)) {
				reader.ReadLine(); //跳过首行
				while (!reader.EndOfStream) {
					var line = reader.ReadLine();
					var values = line.Split(',');

					var keyData = new KeyData {
						Character = char.Parse(values[0]),
						keyTitle = values[1],
						contentCommand = values[2],
						ImgPath = values[3]
					};
					keyDataDictionary.Add(keyData.Character, keyData);
				}
			}
		}
		public MainWindow() {
			string row1 = "QWERTYUIOP";
			string row2 = "ASDFGHJKL";
			string row3 = "ZXCVBNM^";
			ReadCsvData("C:/Users/Frankie/source/repos/LambdaLauncher/resource/testfile.csv");

			InitializeComponent();

			foreach (char c in row1) {
				InteractiveKey interactiveKey = new InteractiveKey(c, keyDataDictionary[c].keyTitle, keyDataDictionary[c].contentCommand, keyDataDictionary[c].ImgPath);
				gridRow1.Children.Add(interactiveKey);
			}
			foreach (char c in row2) {
				InteractiveKey interactiveKey = new InteractiveKey(c, keyDataDictionary[c].keyTitle, keyDataDictionary[c].contentCommand, keyDataDictionary[c].ImgPath);
				gridRow2.Children.Add(interactiveKey);
			}
			foreach (char c in row3) {
				InteractiveKey interactiveKey = new InteractiveKey(c, keyDataDictionary[c].keyTitle, keyDataDictionary[c].contentCommand, keyDataDictionary[c].ImgPath);
				gridRow3.Children.Add(interactiveKey);
			}
		}

		private void CloseWindow(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			this.DragMove();
		}

		private void MinimizeWindow(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}
	}
}
