using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webbage.chat.web.bot.model {
    public struct CommandStruct<TDesc, TParser, TAction> {
        private TDesc desc;
        private TParser parser;
        private TAction action;

        public CommandStruct(TParser parser, TDesc desc, TAction action) {
            this.desc = desc;
            this.parser = parser;
            this.action = action;
        }

        public TDesc Desc { get { return desc; } set { desc = value; } }
        public TParser Parser { get { return parser; } set { parser = value; } }
        public TAction Action { get { return action; } set { action = value; } }
    }
}
