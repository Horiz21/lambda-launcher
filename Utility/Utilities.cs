using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace LambdaLauncher.Utility {

	internal static class Utilities {
		private static readonly string[] imageExtensions = { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tif", ".ico" };//仅支持六种常见图像格式和一种图标格式

		/// <summary>
		/// 判断是否是存在的图片，若是，则返回这张图片
		/// </summary>
		/// <param name="path">图片的物理地址</param>
		/// <returns>如果图片物理地址存在，则返回该图片；否则返回一张空图片</returns>
		public static BitmapImage GetImageFromPath(string path) {
			string lowerPath = path.ToLower();
			bool isImage = imageExtensions.Any(suffix => lowerPath.EndsWith(suffix));
			bool isExist = File.Exists(path);
			if (isImage && isExist)
				return new BitmapImage(new Uri(path));
			else if (path != string.Empty) {
				return new BitmapImage(new Uri("../Properties/Images/noimg.png", UriKind.Relative));
			}
			return GetEmptyImage();
		}

		/// <summary>
		/// 直接获取一张空图片
		/// </summary>
		/// <returns>空白的图片</returns>
		public static BitmapImage GetEmptyImage() {
			return new BitmapImage();
		}

		/// <summary>
		/// 获取Logo
		/// </summary>
		public static BitmapImage GetLogo() {
			return new BitmapImage(new Uri("../Properties/Images/icon.png", UriKind.Relative));
		}

		/// <summary>
		/// 运行一条cmd命令，然后隐藏主程序界面
		/// </summary>
		/// <param name="target">目标指令</param>
		public static void RunCommand(string target) {
			ProcessStartInfo startInfo = new() {
				FileName = "cmd.exe",
				Arguments = @"/C start """" """ + target + @"""",
				CreateNoWindow = true,
				UseShellExecute = false
			};

			if (App.LambdaFunction == 3) {
				App.InstantAvtice = App.Vice = false;
				MainWindow.ReloadGrid();
			}
			else if (App.LambdaFunction == 4) {
				App.InstantAvtice = false;
				MainWindow.ReloadGrid();
			}

			Process process = new() {
				StartInfo = startInfo
			};
			if (process.Start())
				App.Current.MainWindow.Hide(); // 若运行一条指令，则隐藏程序界面
		}
	}
}