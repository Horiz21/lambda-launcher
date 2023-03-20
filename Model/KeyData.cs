using System;

namespace LambdaLauncher.Model {
    [Serializable]
    public class KeyData {
        public char Letter { get; set; }
        public int LinkType { get; set; }
        public string Title { get; set; }
        public string Command { get; set; }
        public string Icon { get; set; }
        //第二组命令、图标和标题
        //public string ViceCommand { get; set; }
        //public string ViceIcon { get; set; }
        //public string ViceTitle { get; set; }

        public string GetLlsFormatData() {
            string[] datas = new string[] { Letter.ToString(), LinkType.ToString(), Title, Command, Icon};
            return string.Join("\t", datas);
        }
    }
}
