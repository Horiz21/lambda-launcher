using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaLauncher.Model {
    [Serializable]
    public class KeyData {
        public char Letter { get; set; }
        public string Command { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        //第二组命令、图标和标题
        //public string ViceCommand { get; set; }
        //public string ViceIcon { get; set; }
        //public string ViceTitle { get; set; }
    }
}
