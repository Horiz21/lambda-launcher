using System;

namespace LambdaLauncher.Model {
	[Serializable]
	public class KeyData {
		// 字母
		public char Letter { get; set; }

		// 第一组命令、图标和标题
		public int LinkType { get; set; } = 0;
		public string Title { get; set; }
		public string Command { get; set; }
		public string Icon { get; set; }

		// 第二组命令、图标和标题
		public int ViceLinkType { get; set; } = 0;
		public string ViceTitle { get; set; }
		public string ViceCommand { get; set; }
		public string ViceIcon { get; set; }

		/// <summary>
		/// 含参构造函数
		/// </summary>
		public KeyData(char Letter, int LinkType, string Title, string Command, string Icon,
			int ViceLinkType, string ViceTitle, string ViceCommand, string ViceIcon) {
			this.Letter = Letter;
			this.LinkType = LinkType;
			this.Title = Title;
			this.Command = Command;
			this.Icon = Icon;
			this.ViceLinkType = ViceLinkType;
			this.ViceTitle = ViceTitle;
			this.ViceCommand = ViceCommand;
			this.ViceIcon = ViceIcon;
		}

		/// <summary>
		/// 转化为Lambda Launcher Settings的一行格式
		/// </summary>
		/// <returns>以制表符分割的字符串，记录了一个Key的元素内容</returns>
		public string GetLlsFormatData() {
			string[] datas = new string[] { LinkType.ToString(), Title, Command, Icon, ViceLinkType.ToString(), ViceTitle, ViceCommand,ViceIcon };
			return string.Join("\t", datas);
		}

		/// <summary>
		/// 清空一个KeyData除了字母以外的所有内容
		/// </summary>
		public void Clear() {
			LinkType = 0;
			ViceTitle = Title = ViceCommand = Command = ViceIcon = Icon = string.Empty;
		}

		/// <summary>
		/// 深拷贝
		/// </summary>
		/// <returns>一个完全和当前内容相同的深拷贝新对象</returns>
		public KeyData DeepCopy() => new KeyData(Letter, LinkType, Title, Command, Icon, ViceLinkType, ViceTitle, ViceCommand,ViceIcon);
	}
}
