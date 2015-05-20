using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webbage.chat.bot {
    public class Insulter {
        private static Random random;
        private static List<string> insults;

        static Insulter() {
            random = new Random();
            insults = new List<string>() {
                "@{0}, your mom is like a race car driver; she burns a lot of rubbers",
                "Your mom is like a doorknob @{0}; everybody gets a turn",
                "@{0}, your mom is like an ice cream cone; everyone gets a lick",
                "Your mom is like a bowling ball @{0}; you can fit three fingers in",
                "@{0}, your mom is like a bowling ball; she always winds up in the gutter",
                "Your mom is like a bowling ball @{0}; she always comes back for more",
                "@{0}, your mom is like McDonalds; Billions and Billions served",
                "Your mom is like Denny's @{0}; open 24 hours",         
                "Your mom is like a railroad track @{0}; she gets laid all over the country.",        
                "If I had change for a buck I could have been your dad @{0}!",
                "When you pass away and people ask me what the cause of death was, @{0}, I'll say it was your stupidity.",
                "When you get run over by a car, @{0}, it shouldn't be listed under accidents.",
                "@{0}, you  were  born  because  your  mother  didn't believe in abortion; now she believes in infanticide.",
                "@{0}, I heard your parents took you to a dog show and you won.",
                "You must have been born on a highway because that's where most accidents happen, @{0}",
                "You are proof that God has a sense of humor, @{0}",
                "@{0}, your birth certificate is an apology letter from the condom factory.",
                "I'd like to see things from your point of view, @{0}, but I can't seem to get my head that far up my ass.",
                "What are you doing here @{0}? Did someone leave your cage open?",
                "Shut up @{0}, you'll never be the man your mother is.",
                "Well, @{0}, I could agree with you, but then we'd both be wrong.",
                "So, a thought crossed your mind, @{0}? Must have been a long and lonely journey.",
                "Everyone who ever loved you was wrong, @{0}.",
                "Learn from your parents' mistakes @{0} - use birth control!",
                "@{0}, why don't you slip into something more comfortable -- like a coma."
            };
        }

        public static string GetInsult() {
            return insults[random.Next(0, insults.Count())];
        }
    }
}
