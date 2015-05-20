using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webbage.chat.bot.helper {
    class CommandDescriptions {
        const string template = "<span class=\"pomegranate-font\">Syntax:</span> <b>{0}</b> <em>{1}</em>. <span class=\"emerald-font\">Purpose:</span> {2}.";

        public static string HELP = string.Format(template, "<b>!help</b>", "<em>[commandName]</em>", "Lists help out for the specified command. For a list of valid commands visit https://github.com/webbage/webbage.chat/wiki/Using-the-Bot");
        public static string INSULT = string.Format(template, "<b>!insult</b>", "<em>{userName}</em>", "Throws a random insult at the specified user. ");
        public static string LMGTFY = string.Format(template, "<b>!lmgtfy</b>", "<em>{queryString}</em>", "Returns an http://www.lmgtfy.com search link.");
        public static string MOAB = string.Format(template, "<b>!moab</b>", "", "<b>This is an admin only command.</b> It kicks all currently connected users out of the room. Used for publishing purposes and killing all active connections.");
        public static string KICK = string.Format(template, "<b>!!kick</b>", "<em>\"{userName}\" [-ban]</em>", "<b>This is an admin only command.</b> It disconnects the specified user. If the optional -ban flag is specified it also bans the user and prevents them from logging back in.");
        public static string GUID = string.Format(template, "<b>!guid</b>", "", "Returns a random new GUID. We are programmers after all.");
        public static string GOOGLE = string.Format(template, "<b>!google</b>", "<em>{queryString}</em>", "Returns an http://www.google.com search link.");
        public static string YOUTUBE = string.Format(template, "<b>!youtube</b>", "<em>\"{videoId}\" [-force]</em>", "Plays the specified video from YouTube or puts it in the queue to be played if one is already playing. If the optional -force flag is specified it immediately starts playing, even if another is already playing.");
        public static string DUCK = string.Format(template, "<b>!duck</b>", "<em>{queryString}</em>", "Returns an http://www.duckduckgo.com search link.");
        public static string ROLL = string.Format(template, "<b>!roll</b>", "<em>[minNumber] [maxNumber]</em>", "Returns a random number between two numbers. If no numbers are specified 1 and 100 are used. If one number is specified 1 and that number are used. If 2 numbers are specified it uses them.");
    }
}
