using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace LambdaLauncher {
	internal static class Utilities {
		private static readonly string[] imageExtensions = { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tif", ".ico" };//仅支持六种常见图像格式和一种图标格式

		// 判断是否是存在的图片，若是，则返回这张图片
		public static BitmapImage GetImageFromPath(string path) {
			string lowerPath = path.ToLower();
			bool isImage = imageExtensions.Any(suffix => lowerPath.EndsWith(suffix));
			bool isExist = File.Exists(path);
			if (isImage && isExist)
				return new BitmapImage(new Uri(path));
			return new BitmapImage();
		}

		// 运行一条cmd命令
		public static void RunCommand(string command) {
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = "cmd.exe";
			startInfo.Arguments = "/C " + command;
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;

			Process process = new Process();
			process.StartInfo = startInfo;
			process.Start();
		}
	}
}
