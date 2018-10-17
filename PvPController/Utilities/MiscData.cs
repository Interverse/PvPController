using System.Collections.Generic;
using System.IO;
using TShockAPI;

namespace PvPController.Variables {
    class MiscData {
        //Sets a debuff and its duration to a flask buff
        public static Dictionary<int, BuffDuration> flaskDebuffs = new Dictionary<int, BuffDuration> {
            //Weapon Imbue Venom
            { 71, new BuffDuration(70, 210) },

            //Weapon Imbue Cursed Flames
            { 73, new BuffDuration(39, 150) },

            //Weapon Imbue Fire
            { 74, new BuffDuration(24, 150) },
                            
            //Weapon Imbue Gold
            { 75,  new BuffDuration(72, 300) },
                         
            //Weapon Imbue Ichor
            { 76, new BuffDuration(69, 300) },
                       
            //Weapon Imbue Nanites
            { 77, new BuffDuration(31, 90) },
                    
            //Weapon Imbue Poison
            { 79, new BuffDuration(20, 300) },
        };

        //Death Message List
        public static List<string> normalDeathMessages = new List<string> {
            " was slain by ",
            " was eviscerated by ",
            " was murdered by ",
            "'s face was torn off by ",
            "'s entrails were ripped out by ",
            " was destroyed by ",
            "'s skull was crushed by ",
            " got massacred by ",
            " got impaled by ",
            " lost his/her head by ",
            " was torn in half by ",
            " was decapitated by ",
            " let their arms get torn off by ",
            " watched their innards become outards by ",
            " was brutally dissected by ",
            "'s extremities were detached by ",
            "'s body was mangled by ",
            "'s vital organs were ruptured by ",
            " was turned into a pile of flesh by ",
            " was removed from " + Terraria.Main.worldName + " by ",
            " got snapped in half by ",
            " was cut down the middle by ",
            " was chopped up by ",
            "'s plead for death was answered by ",
            "'s meat was ripped off the bone by ",
            "'s flailing about was finally stopped by ",
            " had their head removed by ",
            " was annihilated to shreds by ",
            "'s body was roasted in a high flame by ",
            "'s hand was nailed to the wall by ",
            " was burnt to a crisp by ",
            " failed to maneuver around ",
            " was tossed off a cliff by ",
            " pointed their face directly at ",
            " exploded into tiny pieces by ",
            "'s heart was pierced by ",
            " was faked out by ",
            "'s soul was diced into slices by ",
            " was left in the heat by ",
            " fell unconscious from ",
            "'s internal lungs were filled up by ",
            " suddenly collapsed from ",
            " was slammed directly into the ground by ",
            " was sent into outer space by ",
            " met the Moon Lord with the aid of ",
            " absorbed too much impact from ",
            " life force was absorbed into "
        };

        //Death Message List for thorn-type attacks
        public static List<string> reflectedDeathMessages = new List<string> {
            " was punctured to death by ",
            " was pricked by ",
            " got their weapon reflected by ",
            " unfortunately came across ",
            " didn't expect ",
            " threw their body right into ",
            " played ping-pong with ",
            " sat on top of "
        };
    }
}
