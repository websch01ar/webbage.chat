using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace webbage.chat.bot {
    public class RoomCommandInterpreter {

        public void DoWork(Command cmd) {
            if (!DoCommand(cmd))
                cmd.Response = "invalid command";
        }
        private bool DoCommand(Command cmd) {

            return true;
        }
    }
}
