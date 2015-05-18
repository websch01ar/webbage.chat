using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webbage.chat.model;

namespace webbage.chat.web.bot.helper {
    class BotMessenger {
        private static User BENDER {
            get {
                return new User {
                    Name = "ಠ_ಠ",
                    Picture = "content/img/bender.jpg"
                };
            }
        }

        public static Message INVALID_COMMAND {
            get {
                return GetMessage("Unable to validate command. Make sure you typed a valid command name");
            }
        }
        public static Message INVALID_PARSE_COMMAND {
            get {
                return GetMessage("Unable to parse this command. Make sure you typed in valid parameters for it. If you need help, type in !help {commandName}");
            }
        }
        public static Message INVALID_PARSE_ARGUMENTS { 
            get { 
                return GetMessage("Unable to parse the arguments of this command. Make sure you typed in valid parameters for it. If you need help, type in !help {commandName}"); 
            } 
        }
        public static Message INVALID_EXEC_COMMAND {
            get {
                return GetMessage("Unable to execute this command. Make sure you typed in valid parameters for it. If you need help, type in !help {commandName}");
            }
        }
        public static Message INVALID_PRIVILEGES {
            get {
                return GetMessage("You don't have permission to do that");
            }
        }
        public static Message NOT_YET_IMPLEMENTED {
            get {
                return GetMessage("This hasn't been implemented yet");
            }
        }

        public static Message INVALID_ARGUMENT_NUM {
            get {
                return GetMessage("Invalid number of arguments");
            }
        }


        public static Message GetMessage(string content) {
            return new Message {
                Sender = BENDER,
                Content = content,
                Sent = DateTime.Now.ToString(),
                IsCode = false
            };
        }

        
    }
}
