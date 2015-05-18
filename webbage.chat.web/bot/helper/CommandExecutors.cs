using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webbage.chat.model;
using webbage.chat.web.hub;

namespace webbage.chat.web.bot.helper {
    class CommandExecutors {

        static CommandExecutors() {
            
        }

        #region Normal Commands
        internal static void Help(ref Command cmd) {
            cmd.Response = BotMessenger.GetMessage(Bender.GetCommandDescription(cmd.Dynamic.Cmd));
        }
        
        internal static void Insult(ref Command cmd) {
            cmd.Response = BotMessenger.GetMessage(string.Format(cmd.Dynamic.Insult, cmd.Dynamic.Recipient));
        }

        internal static void LMGTFY(ref Command cmd) {
            cmd.Response = BotMessenger.GetMessage(string.Format("http://www.lmgtfy.com/?q={0}", cmd.Dynamic.QueryString));
        }

        internal static void Google(ref Command cmd) {
            cmd.Response = BotMessenger.GetMessage(string.Format("http://www.google.com/#q={0}", cmd.Dynamic.QueryString));
        }

        internal static void Roll(ref Command cmd) {
            Random random = new Random();            
            cmd.Response = BotMessenger.GetMessage(random.Next(cmd.Dynamic.Number, cmd.Dynamic.MaxNumber).ToString());
        }

        internal static void GUID(ref Command cmd) {
            cmd.Response = BotMessenger.GetMessage(cmd.Dynamic.GUID);
        }

        internal static void Duck(ref Command cmd) {
            cmd.Response = BotMessenger.GetMessage(string.Format("http://duckduckgo.com/?q={0}", cmd.Dynamic.QueryString));
        }
        #endregion

        #region Admin Commands
        internal static void Kick(ref Command cmd) {
            if (cmd.CallerIsAdmin) {
               cmd.Response = BotMessenger.NOT_YET_IMPLEMENTED;
            } else {
                cmd.Response = BotMessenger.INVALID_PRIVILEGES;
            }
        }

        internal static void YouTube(ref Command cmd) {
            if (cmd.CallerIsAdmin) {
                cmd.Response = BotMessenger.NOT_YET_IMPLEMENTED;
            } else {
                cmd.Response = BotMessenger.INVALID_PRIVILEGES;
            }
        }

        internal static void MOAB(ref Command cmd) {
            if (cmd.CallerIsAdmin) {
                cmd.Response = BotMessenger.NOT_YET_IMPLEMENTED;
            } else {
                cmd.Response = BotMessenger.INVALID_PRIVILEGES;
            }
        }
        #endregion        
    }
}
