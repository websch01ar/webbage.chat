using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webbage.chat.web.bot.helper {
    class CommandDescriptions {
        // "<span class=\"pomegranate-font\">Syntax:</span> <b>[[[ COMMAND NAME HERE ]]]</b> <em>[[[ PARAMETERS HERE ]]]</em>. <span class=\"emerald-font\">Purpose:</span> [[[ DESCRIPTION HERE ]]]";
        public const string HELP = "<span class=\"pomegranate-font\">Syntax:</span> <b>!help</b> <em>[commandName]</em>. <span class=\"emerald-font\">Purpose:</span> Lists help out for the specified command. For a list of valid commands visit https://github.com/webbage/webbage.chat/wiki/Using-the-Bot";
        public const string INSULT = "<span class=\"pomegranate-font\">Syntax:</span> <b>!insult</b> <em>{userName}</em>. <span class=\"emerald-font\">Purpose:</span> Throws a random insult at the specified user. ";
        public const string LMGTFY = "<span class=\"pomegranate-font\">Syntax:</span> <b>!lmgtfy</b> <em>{queryString}</em>. <span class=\"emerald-font\">Purpose:</span> Returns an http://www.lmgtfy.com search link.";
        public const string MOAB = "<span class=\"pomegranate-font\">Syntax:</span> <b>!moab</b>. <span class=\"emerald-font\">Purpose:</span> <b>This is an admin only command.</b> It kicks all currently connected users out of the room. Used for publishing purposes and killing all active connections.";
        public const string KICK = "<span class=\"pomegranate-font\">Syntax:</span> <b>!!kick</b> <em>\"{userName}\" [-ban]</em>. <span class=\"emerald-font\">Purpose:</span> <b>This is an admin only command.</b> It disconnects the specified user. If the optional -ban flag is specified it also bans the user and prevents them from logging back in.";
        public const string GUID = "<span class=\"pomegranate-font\">Syntax:</span> <b>!guid</b>. <span class=\"emerald-font\">Purpose:</span> Returns a random new GUID. We are programmers after all.";
        public const string GOOGLE = "<span class=\"pomegranate-font\">Syntax:</span> <b>!google</b> <em>{queryString}</em>. <span class=\"emerald-font\">Purpose:</span> Returns an http://www.google.com search link.";
        public const string YOUTUBE = "<span class=\"pomegranate-font\">Syntax:</span> <b>!youtube</b> <em>\"{videoId}\" [-force]</em>. <span class=\"emerald-font\">Purpose:</span> Plays the specified video from YouTube or puts it in the queue to be played if one is already playing. If the optional -force flag is specified it immediately starts playing, even if another is already playing.";
        public const string DUCK = "<span class=\"pomegranate-font\">Syntax:</span> <b>!duck</b> <em>{queryString}</em>. <span class=\"emerald-font\">Purpose:</span> Returns an http://www.duckduckgo.com search link.";
        public const string ROLL = "<span class=\"pomegranate-font\">Syntax:</span> <b>!roll</b> <em>[minNumber] [maxNumber]</em>. <span class=\"emerald-font\">Purpose:</span> Returns a random number between two numbers. If no numbers are specified 1 and 100 are used. If one number is specified 1 and that number are used. If 2 numbers are specified it uses them.";
    }
}
