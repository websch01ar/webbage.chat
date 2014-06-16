using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webbage.chat.bot {
    public class BotCommandInterpreter {

        private CommandHelper helper;
        private InsultGrabber insult;
        public Dictionary<string, CommandStruct<string, Func<string, string[]>, Func<Command, bool>>> commands;

        public BotCommandInterpreter() {
            helper = new CommandHelper(this);
            insult = new InsultGrabber();

            ///     Master Command List
            ///     String:         Command name [key]
            ///     CommandStruct:  [value]
            ///         String:     !help <commandName> command description
            ///         Delegate1:  Parser
            ///         Delegate2:  Command action
            commands = new Dictionary<string, CommandStruct<string, Func<string, string[]>, Func<Command, bool>>>() {
                {
                    "!help",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, bool>>() {
                        Desc = "Usage: !help {commandName} :: lists available commands (if {commandName} is omitted), if {commandName} is included gets information specific to that command.",
                        Parser = new Func<string, string[]>(helper.ParseNormal),
                        Action = new Func<Command, bool>(helper.Help)
                    }
                },
                {
                    "!insult",
                    new CommandStruct<string, Func<string, string[]>, Func<Command, bool>>() {
                        Desc = "Usage: !insult [userName] :: hurls a random insult at the specified userName.",
                        Parser = new Func<string, string[]>(helper.ParseNormal),
                        Action = new Func<Command, bool>(helper.Insult)
                    }
                }
            };
        }

        public bool DoCommand(Command cmd) {
            if (!(validateCommand(cmd.Text.Split(' ')[0])))
                return false;

            if (!parseCommand(cmd))
                return false;

            if (!executeCommand(cmd))
                return false;

            return true;
        }
        // validate that this is a command contained within commands
        private bool validateCommand(string command) {
            if (!commands.Keys.Contains(command))
                return false;

            return true;
        }
        // parse the command passed in [CommandStruct.Delegate1]
        private bool parseCommand(Command cmd) {
            try {
                cmd.Args = (string[])commands[cmd.Text.Split(' ')[0]].Parser.DynamicInvoke(new object[] { (object)cmd.Text });
                return true;
            } catch {
                return false;
            }
        }
        // run the method for this command [CommandStruct.Delegate2]
        private bool executeCommand(Command cmd) {
            return (bool)commands[cmd.Args[0]].Action.DynamicInvoke(new object[] { (object)cmd });
        }

        private class CommandHelper {
            private BotCommandInterpreter parent;
            public CommandHelper(BotCommandInterpreter parent) {
                this.parent = parent;
            }

            #region [!command] Parsers
            public string[] ParseNormal(string command) {
                return command.Split(' ');
            }
            public string[] ParseQuotes(string command) {
                return command.Split('"').Select(p => p.Trim()).ToArray();
            }
            #endregion

            #region [!command] Delegates
            public bool Help(Command cmd) {
                if (cmd.Args.Count() > 1)
                    return helpHelper(cmd);

                cmd.Response = "Valid commands are: ";
                foreach (string key in parent.commands.Keys) {
                    cmd.Response += string.Format("[{0}], ", key);
                }
                cmd.Response = cmd.Response.Replace(',', ' ').Trim();
                cmd.Response += ". To get specific information for each command type '!help {commandName}'. Parameters listed in {} are optional. Parameters listed in [] are required.";

                return true;
            }
            public bool Insult(Command cmd) {
                if (cmd.Args.Count() < 2)
                    return false;

                cmd.Response = string.Format(parent.insult.GetNew(), cmd.Args[1]);
                return true;
            }
            #endregion

            #region [!command] Helpers
            private bool helpHelper(Command cmd) {
                if (!cmd.Args[1].StartsWith("!"))
                    cmd.Args[1] = "!" + cmd.Args[1];

                if (!(parent.validateCommand(cmd.Args[1])))
                    return false;

                cmd.Response = parent.commands[cmd.Args[1]].Desc;
                return true;
            }
            #endregion

        }
    }
}
