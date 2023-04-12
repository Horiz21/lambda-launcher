using LambdaLauncher.Model;
using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace LambdaLauncher {

	public partial class App : Application {
		private Mutex _mutex;

		public static Uri LlsPath = new Uri(Path.Combine(Directory.GetCurrentDirectory(), @"Properties/Settings/setting.lls"));
		public static Config config;

		protected override void OnStartup(StartupEventArgs e) {
			_mutex = new Mutex(true, "LambdaLauncher", out var createdNew);
			if (createdNew) {
				base.OnStartup(e);
				config = new Config(LlsPath.LocalPath);
			}
			else {
				// 应用程序实例已经存在，退出新实例
				MessageBox.Show((string)Current.FindResource("MultipleInstanceErrorTip"), (string)Current.FindResource("MultipleInstanceError"), MessageBoxButton.OK, MessageBoxImage.Error);
				Current.Shutdown();
				return;
			}
		}

		protected override void OnExit(ExitEventArgs e) {
			// 释放互斥锁
			_mutex.ReleaseMutex();
			_mutex.Dispose();

			base.OnExit(e);
		}

		public static void ImportSettings(string path) {
			config = new Config(path);
			config.ReadAndLoadSettings();
		}

		public static void RestoreSettings(Config oldConfig) {
			config = oldConfig;
			config.ReadAndLoadSettings();
		}

		public static void ExportSettings(string path) {
			config.SaveAndWriteSettings(path);
		}

		public static void ChangeSetting() {
			config.SaveAndWriteSettings(LlsPath.LocalPath);
			config.ReadAndLoadSettings();
		}
	}
}