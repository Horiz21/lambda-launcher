using System.Xml.Serialization;
using System.IO;
using LambdaLauncher.Model;
using System.Linq;
using System.Windows;
using System;

namespace LambdaLauncher.Utility {
	public static class Data {
		private static readonly string csvpath = @"C:\Users\Frankie\source\repos\LambdaLauncher\Resource\testfile.csv";
		private static string[] keyCsvDatas = new string[28]; // 用于存放1个初始行和27个字母信息（单行形式）
		public static KeyData[] keyDatas = new KeyData[27]; // 用于存放27个字母信息（对象形式）

		/// <summary>
		/// 从固定的位置读取数据，产生单行信息和对象信息
		/// </summary>
		public static void Read() {
			keyCsvDatas = File.ReadAllLines(csvpath);
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
		/// 修改某一个字母对应的单行信息，并写回固定位置的.csv文件
		/// </summary>
		/// <param name="keyData">含修改后信息的KeyData对象</param>
		public static void ModifyAndWrite(KeyData keyData) {
			keyCsvDatas[keyData.Letter - 'A' + 1] = keyData.GetCsvFormatData();
			File.WriteAllLines(csvpath, keyCsvDatas);
		}
	}
}
