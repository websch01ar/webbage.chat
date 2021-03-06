﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webbage.chat.model;

namespace webbage.chat.model {
    public class Command {
        public string Name { get; set; }
        public string Text { get; set; }
        public string[] Args { get; set; }
        public Message Response { get; set; }
        public dynamic Dynamic { get; set; }
        public bool CallerIsAdmin { get; set; }

        public Command(Message message) {

            int firstSpace = message.Content.IndexOf(' ');
            if (firstSpace != -1) {
                this.Name = message.Content.Substring(0, firstSpace).ToLower();
                this.Text = message.Content.Substring(firstSpace + 1);
            } else {
                this.Name = message.Content.ToLower();
                this.Text = string.Empty;
            }

            this.CallerIsAdmin = message.Sender.IsAdmin;
        }
    }
}
