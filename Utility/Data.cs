using System.Xml.Serialization;
using System.IO;
using LambdaLauncher.Model;
using System.Linq;
using System.Windows;
using System;
using System.Diagnostics;

namespace LambdaLauncher.Utility {
	public static class Data {
		private static readonly string csvpath = @"..\..\..\Settings\setting.lls";
		// 按键信息
		private static string[] keyCsvDatas = new string[28]; // 用于存放1个初始行和27个字母信息（单行形式）
		public static KeyData[] keyDatas = new KeyData[27]; // 用于存放27个字母信息（对象形式）

		// 设置信息
		public static string Language { get; set; }
		public static string Theme { get; set; }
		public static bool DarkMode { get; set; }
		public static bool KeyboardDouble { get; set; }
		public static bool MouseDouble { get; set; }
		public static int LambdaFunction { get; set; }

		/// <summary>
		/// 从固定的位置读取数据，产生单行信息和对象信息
		/// </summary>
		public static void LoadData() {
			keyCsvDatas = File.ReadAllLines(csvpath);

			// 写入设置相关信息
			string[] settings = keyCsvDatas[0].Split(new char[] { ',', '&' });
			Language = settings[0];
			Theme = settings[1];
			DarkMode = settings[2] == "1";
			KeyboardDouble = settings[3] == "1";
			MouseDouble = settings[4] == "1";
			LambdaFunction = int.Parse(settings[5]);

			LoadLlsSettings();

			// 写入字母的相关信息
			for (int i = 1; i < 28; ++i) {
				// 根据逗号，分割出四个子串
				string[] strs = keyCsvDatas[i].Split(',');

				// 根据子串新建keyData
				char letter = char.Parse(strs[0]);
				keyDatas[letter - 'A'] = new KeyData {
					Letter = letter,
					Title = strs[1],
					Command = strs[2],
					Icon = strs[3]
				};
			}
		}

		/// <summary>
		/// 加载.lls文件中的设置项
		/// </summary>
		public static void LoadLlsSettings() {
			string head = @"pack://application:,,,/";
			App.Current.Resources.MergedDictionaries[0].Source = new Uri(head + "Language/" + Data.Language + ".xaml");
			App.Current.Resources.MergedDictionaries[1].Source = new Uri(head + "Resource/Themes/" + Data.Theme + ".xaml");
		}

		/// <summary>
		/// 修改某一个字母对应的单行信息，并写回固定位置的.csv文件
		/// </summary>
		/// <param name="keyData">含修改后信息的KeyData对象</param>
		public static void ModifyAndWrite(KeyData keyData) {
			keyCsvDatas[keyData.Letter - 'A' + 1] = keyData.GetCsvFormatData();
			File.WriteAllLines(csvpath, keyCsvDatas);
		}
	}
}
