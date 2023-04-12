using System;
using System.IO;

namespace LambdaLauncher.Model {

	/// <summary>
	/// 该类是一个配置文件类。
	/// 程序启动时，App会实例化一个“path=默认路径”的Config对象。
	/// 当进入Settings页面时，程序深拷贝一个App.config暂存，且所有操作都在旧的App.config上直接修改。
	/// 若最后点击了保存，则不用做操作，等待GC回收暂存Config对象即可；
	/// 若最后点击了取消，则需要将App.config重新赋值回刚才暂存的对象。
	/// </summary>
	public class Config {
		public static string[] Languages = new string[4] { "zh_Hans", "zh_Hant", "en", "jp" };
		public static string[] Themes = new string[8] { "bmbo", "cbpk", "dodl", "xwhs", "sfsr", "dstr", "aztl", "cnlt" };

		private readonly string path;
		public string[] llsFile = new string[28];  // 用于存放1个初始行和27个字母信息（单行形式）
		public KeyData[] keyDatas = new KeyData[27];  // 用于存放27个字母信息（对象形式）
		public int Language { get; set; }
		public int Theme { get; set; }
		public bool DarkMode { get; set; }
		public bool KeyboardDouble { get; set; }
		public bool MouseDouble { get; set; }
		public int LambdaFunction { get; set; }
		public string? Hotkey { get; set; }
		public bool InstantAvtice { get; set; } = false;  // 如果为真，则忽略二次访问设置，直接改为一次访问
		public bool Vice { get; set; } = false;  // 如果为真，则刷新成第二键设置

		public Config(string path) {
			this.path = path;
			llsFile = File.ReadAllLines(path); // 读取所有数据（包括按键设置和全局设置）
			ReadAndLoadSettings(isInit: true);  // 加载全局设置
			ReadAndLoadKeys();  // 加载按键设置
		}

		/// <summary>
		/// 已经读取了lls文件，加载lls文件中的全局设置
		/// </summary>
		public void ReadAndLoadSettings(bool isInit = false) {
			// 如果是初次启动，则读取设置项的内容（Read）
			if (isInit) {
				string[] settings = llsFile[27].Split('\t');
				Language = int.Parse(settings[0]);
				Theme = int.Parse(settings[1]);
				DarkMode = settings[2] == "1";
				KeyboardDouble = settings[3] == "1";
				MouseDouble = settings[4] == "1";
				LambdaFunction = int.Parse(settings[5]);
				Hotkey = settings[6];
			}

			// 加载所使用的资源文件（Load）
			App.Current.Resources.MergedDictionaries[0].Source = new Uri("../Properties/Languages/" + Languages[Language] + ".xaml", UriKind.Relative);
			App.Current.Resources.MergedDictionaries[1].Source = new Uri("../Properties/Themes/" + Themes[Theme] + ".xaml", UriKind.Relative);
			App.Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/" + (DarkMode ? "DarkMode" : "LightMode") + ".xaml", UriKind.Relative);
		}

		/// <summary>
		/// 已经读取了lls文件，加载lls文件中的按键设置
		/// </summary>
		private void ReadAndLoadKeys() {
			for (int i = 0; i < 27; ++i) {  // 写入字母的相关信息
				string[] strs = llsFile[i].Split('\t');  // 根据制表符，分割出四个子串
				keyDatas[i] = new KeyData(
					(char)('A' + i),
					int.Parse(strs[0]), strs[1], strs[2], strs[3],
					int.Parse(strs[4]), strs[5], strs[6], strs[7]
				);
			}
		}

		/// <summary>
		/// 储存所有当前设置项到变量和文件"path"里
		/// </summary>
		public void SaveAndWriteSettings(string path) {
			// 保存到变量里（Save）并修改字符串
			llsFile[27] = string.Join("\t", Language, Theme, DarkMode ? "1" : "0", KeyboardDouble ? "1" : "0", MouseDouble ? "1" : "0", LambdaFunction, Hotkey);

			// 存储到文件中（Write）
			File.WriteAllLines(path, llsFile);
		}

		/// <summary>
		/// 修改某一个字母对应的单行信息，并写回固定位置的.csv文件
		/// </summary>
		/// <param name="keyData">含修改后信息的KeyData对象</param>
		public void ModifyAndWrite(KeyData keyData) {
			llsFile[keyData.Letter - 'A'] = keyData.GetLlsFormatData();
			File.WriteAllLines(path, llsFile);
		}

		/// <summary>
		/// 切换当前显示模式（日间/夜间）为另一个（夜间/日间），并且将此更改存储为文件
		/// </summary>
		public void SwitchDarkMode() {
			DarkMode ^= true;  // 交换日间/夜间模式
			App.Current.Resources.MergedDictionaries[2].Source = new Uri("../Properties/Themes/" + (DarkMode ? "Dark" : "Light") + "Mode.xaml", UriKind.Relative);
			llsFile[27] = string.Join("\t", Language, Theme, DarkMode ? "1" : "0", KeyboardDouble ? "1" : "0", MouseDouble ? "1" : "0", LambdaFunction, Hotkey);
			File.WriteAllLines(path, llsFile);
		}

		// 深拷贝一个内容相同的Config对象
		public Config DeepCopy() => new Config(path);
	}
}