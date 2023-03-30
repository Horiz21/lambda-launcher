using LambdaLauncher.Model;
using System;
using System.IO;
using System.Windows;

namespace LambdaLauncher {

	public partial class App : Application {

		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);
			LoadData();  // 读取设置项和按键功能信息
		}

		public static string[] Languages = new string[3] { "zh_Hans", "zh_Hant", "en" };
		public static string[] Themes = new string[3] { "bmbo", "cbpk", "dodl" };
		private static readonly string LlsPath = "../../../Settings/setting.lls";  // 设置文件（.lls)
		private static string[] keyLlsDatas = new string[28];  // 用于存放1个初始行和27个字母信息（单行形式）
		public static KeyData[] keyDatas = new KeyData[27];  // 用于存放27个字母信息（对象形式）

		public static int Language { get; set; }
		public static int Theme { get; set; }
		public static bool DarkMode { get; set; }
		public static bool KeyboardDouble { get; set; }
		public static bool MouseDouble { get; set; }
		public static int LambdaFunction { get; set; }
		public static bool InstantAvtice { get; set; } = false;  // 如果为真，则忽略二次访问设置，直接改为一次访问
		public static bool Vice { get; set; } = false;  // 如果为真，则刷新成第二键设置

		/// <summary>
		/// 从固定的位置读取数据，产生单行信息和对象信息
		/// </summary>
		public static void LoadData() {
			keyLlsDatas = File.ReadAllLines(LlsPath);

			// 读出设置相关信息
			string[] settings = keyLlsDatas[27].Split('\t');
			Language = int.Parse(settings[0]);
			Theme = int.Parse(settings[1]);
			DarkMode = settings[2] == "1";
			KeyboardDouble = settings[3] == "1";
			MouseDouble = settings[4] == "1";
			LambdaFunction = int.Parse(settings[5]);

			LoadLlsSettings();

			// 写入字母的相关信息
			for (int i = 0; i < 27; ++i) {
				// 根据制表符，分割出四个子串
				string[] strs = keyLlsDatas[i].Split('\t');

				// 根据子串新建keyData
				char letter = (char)('A' + i);
				keyDatas[letter - 'A'] = new KeyData(
					letter,
					int.Parse(strs[0]), strs[1], strs[2], strs[3],
					int.Parse(strs[4]), strs[5], strs[6], strs[7]
				);
			}
		}

		/// <summary>
		/// 加载.lls文件中的设置项
		/// </summary>
		public static void LoadLlsSettings() {
			string head = @"pack://application:,,,/";
			Current.Resources.MergedDictionaries[0].Source = new Uri("../Properties/Languages/" + Languages[Language] + ".xaml", UriKind.Relative);
			Current.Resources.MergedDictionaries[1].Source = new Uri("../Properties/Themes/" + Themes[Theme] + ".xaml", UriKind.Relative);
			Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/" + (DarkMode ? "DarkMode" : "LightMode") + ".xaml", UriKind.Relative);
		}

		/// <summary>
		/// 储存由Settings类给出的新设置项到文件里
		/// </summary>
		public static void SaveLlsSettings(int language, int theme, bool darkMode, bool keyboardDouble, bool mouseDouble, int lambdaFunction) {
			// 更新本地数据
			Language = language;
			Theme = theme;
			DarkMode = darkMode;
			KeyboardDouble = keyboardDouble;
			MouseDouble = mouseDouble;
			LambdaFunction = lambdaFunction;

			// 修改字符串
			keyLlsDatas[27] = string.Join("\t", new string[] { language.ToString(), theme.ToString(), darkMode ? "1" : "0", keyboardDouble ? "1" : "0", mouseDouble ? "1" : "0", lambdaFunction.ToString() });

			// 存储到文件中
			File.WriteAllLines(LlsPath, keyLlsDatas);
		}

		/// <summary>
		/// 修改某一个字母对应的单行信息，并写回固定位置的.csv文件
		/// </summary>
		/// <param name="keyData">含修改后信息的KeyData对象</param>
		public static void ModifyAndWrite(KeyData keyData) {
			keyLlsDatas[keyData.Letter - 'A'] = keyData.GetLlsFormatData();
			File.WriteAllLines(LlsPath, keyLlsDatas);
		}

		/// <summary>
		/// 切换当前显示模式（日间/夜间）为另一个（夜间/日间）
		/// </summary>
		public static void SwitchDarkMode() {
			if (DarkMode) {
				DarkMode = false;
				Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/LightMode.xaml", UriKind.Relative);
			}
			else {
				DarkMode = true;
				Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/DarkMode.xaml", UriKind.Relative);
			}
			keyLlsDatas[27] = Language + "\t" + Theme + "\t" + (DarkMode ? "1" : "0") + "\t" + (KeyboardDouble ? "1" : "0") + "\t" + (MouseDouble ? "1" : "0") + "\t" + LambdaFunction;
			File.WriteAllLines(LlsPath, keyLlsDatas);
		}
	}
}