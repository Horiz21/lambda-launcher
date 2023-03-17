using System.Xml.Serialization;
using System.IO;
using LambdaLauncher.Model;
using System.Linq;
using System.Windows;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace LambdaLauncher.Utility {
	public static class Data {
		// 语言字典
		public static Dictionary<string, string> LanguageDictionary = new Dictionary<string, string> {
			{"中文","zh_cn"},
			{"English","en_us"},
		};

		// 主题字典
		public static Dictionary<string, string> ThemeDictionary = new Dictionary<string, string> {
			{"Bamboo","bamboo"},
			{"Cyberpunk","cyberpunk"},
		};

		// 设置文件（.lls)
		private static readonly string LlsPath = @"..\..\..\Settings\setting.lls";

		// 按键信息
		private static string[] keyLlsDatas = new string[28]; // 用于存放1个初始行和27个字母信息（单行形式）
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
			keyLlsDatas = File.ReadAllLines(LlsPath);

			// 读出设置相关信息
			string[] settings = keyLlsDatas[27].Split('\t');
			Language = settings[0];
			Theme = settings[1];
			DarkMode = settings[2] == "1";
			KeyboardDouble = settings[3] == "1";
			MouseDouble = settings[4] == "1";
			LambdaFunction = int.Parse(settings[5]);

			LoadLlsSettings();

			// 写入字母的相关信息
			for (int i = 0; i < 27; ++i) {
				// 根据逗号，分割出四个子串
				string[] strs = keyLlsDatas[i].Split('\t');

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
			Application.Current.Resources.MergedDictionaries[0].Source = new Uri(head + "Language/" + Language + ".xaml");
			Application.Current.Resources.MergedDictionaries[1].Source = new Uri(head + "Resource/Themes/" + Theme + ".xaml");
			Application.Current.Resources.MergedDictionaries[2].Source = new Uri(head + "Resource/Themes/" + (DarkMode ? "DarkMode" : "LightMode") + ".xaml");
		}

		public static void SaveLlsSettings(string Language, string Theme, bool DarkMode, bool KeyboardDouble, bool MouseDouble, int LambdaFunction) {
			// 更新本地数据
			if (Language != null) Data.Language = LanguageDictionary[Language];
			if (Theme != null) Data.Theme = ThemeDictionary[Theme];
			Data.DarkMode = DarkMode;
			Data.KeyboardDouble = KeyboardDouble;
			Data.MouseDouble = MouseDouble;
			Data.LambdaFunction = LambdaFunction;
			keyLlsDatas[27] = Data.Language + "\t" + Data.Theme + "\t" + (DarkMode ? "0" : "1") + "\t" + (KeyboardDouble ? "0" : "1") + "\t" + (MouseDouble ? "0" : "1") + "\t" + LambdaFunction;
			File.WriteAllLines(LlsPath, keyLlsDatas);
		}

		/// <summary>
		/// 修改某一个字母对应的单行信息，并写回固定位置的.csv文件
		/// </summary>
		/// <param name="keyData">含修改后信息的KeyData对象</param>
		public static void ModifyAndWrite(KeyData keyData) {
			keyLlsDatas[keyData.Letter - 'A' + 1] = keyData.GetLlsFormatData();
			File.WriteAllLines(LlsPath, keyLlsDatas);
		}
	}
}
