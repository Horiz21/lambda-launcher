using System;

namespace LambdaLauncher.Model {
	[Serializable]
	public class KeyData {
		public char Letter { get; set; }
		public int LinkType { get; set; } = 0;
		public string Title { get; set; }
		public string Command { get; set; }
		public string Icon { get; set; }
		//第二组命令、图标和标题
		//public int ViceLinkType { get; set; }
		//public string ViceCommand { get; set; }
		//public string ViceIcon { get; set; }
		//public string ViceTitle { get; set; }

		public KeyData(char Letter, int LinkType, string Title, string Command, string Icon) {
			this.Letter = Letter;
			this.LinkType = LinkType;
			this.Title = Title;
			this.Command = Command;
			this.Icon = Icon;
		}

		public string GetLlsFormatData() {
			string[] datas = new string[] { Letter.ToString(), LinkType.ToString(), Title, Command, Icon };
			return string.Join("\t", datas);
		}

		/// <summary>
		/// 清空一个KeyData除了字母以外的所有内容
		/// </summary>
		public void Clear() {
			LinkType = 0;
			Title = string.Empty;
			Command = string.Empty;
			Icon = string.Empty;
		}

		public KeyData DeepCopy() {
			KeyData copyData = new KeyData(Letter, LinkType, Title, Command, Icon);
			return copyData;
		}
	}
}
