
namespace webbage.chat.bot {
    public class Command {
        public string Text { get; set; }
        public string[] Args { get; set; }
        public string Response { get; set; }

        public Command(string commandText) {
            this.Text = commandText;
        }
    }
}
