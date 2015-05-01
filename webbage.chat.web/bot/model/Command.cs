using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webbage.chat.web.bot.model {
    public class Command {
        public string Name { get; set; }
        public string Text { get; set; }
        public string[] Args { get; set; }
        public string Response { get; set; }

        public Command(string commandText) {
            int firstSpace = commandText.IndexOf(' ');
            if (firstSpace != -1) {
                this.Name = commandText.Substring(0, firstSpace);
                this.Text = commandText.Substring(firstSpace + 1);
            } else {
                this.Name = commandText;
                this.Text = "";
            }
        }
    }
}
