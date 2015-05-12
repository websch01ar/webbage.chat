using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webbage.chat.model.bot {
    public struct CommandStruct<T, TDesc, TParser, TAction> where T : class {
        private T type;
        private TDesc desc;
        private TParser parser;
        private TAction action;

        public CommandStruct(T type, TParser parser, TDesc desc, TAction action) {
            this.type = type;
            this.desc = desc;
            this.parser = parser;
            this.action = action;
        }

        public T Type { get { return type; } set { type = value; } }
        public TDesc Desc { get { return desc; } set { desc = value; } }
        public TParser Parser { get { return parser; } set { parser = value; } }
        public TAction Action { get { return action; } set { action = value; } }
    }
}
