using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webbage.chat.model {
    public struct CommandStruct<TDesc, TParser, TArgParser, TAction> {
        private TDesc desc;
        private TParser parser;
        private TArgParser argParser;
        private TAction action;

        public CommandStruct(TDesc desc, TParser parser, TArgParser argParser, TAction action) {
            this.desc = desc;
            this.parser = parser;
            this.argParser = argParser;
            this.action = action;
        }

        public TDesc Desc { get { return desc; } set { desc = value; } }
        public TParser Parser { get { return parser; } set { parser = value; } }
        public TArgParser ArgParser { get { return argParser; } set { argParser = value; } }
        public TAction Action { get { return action; } set { action = value; } }
    }
}
