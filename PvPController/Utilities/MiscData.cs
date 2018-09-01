using System.Collections.Generic;
using System.IO;
using TShockAPI;

namespace PvPController.Utilities {
    class MiscData {
        //Defines a weapon to a projectile that spawns from another projectile
        public static Dictionary<int, int> fromWhatWeapon = new Dictionary<int, int> {
            { 19, 119 },
            { 33, 191 },
            { 52, 284 },
            { 90, 515 },
            { 92, 516 },
            { 113, 670 },
            { 150, 788 },
            { 151, 788 },
            { 152, 788 },
            { 182, 1122 },
            { 239, 1244 },
            { 249, 1258 },
            { 250, 1260 },
            { 251, 1260 },
            { 264, 1244 },
            { 272, 1324 },
            { 296, 1445 },
            { 301, 1513 },
            { 307, 1571 },
            { 320, 1825 },
            { 321, 1826 },
            { 344, 1947 },
            { 400, 2590 },
            { 401, 2590 },
            { 402, 2590 },
            { 405, 2611 },
            { 443, 2796 },
            { 491, 3030 },
            { 493, 3051 },
            { 494, 3051 },
            { 511, 3105 },
            { 512, 3105 },
            { 513, 3105 },
            { 522, 3209 },
            { 604, 3389 },
            { 617, 3476 },
            { 619, 3476 },
            { 620, 3476 }
        };

        //Minion IDs
        public static List<int> minionids = new List<int> {
            226, 373, 375, 377, 379, 387, 388, 393, 394, 395, 191, 192, 193, 194,
            423, 317, 407, 533, 625, 626, 627, 628, 613, 614, 337, 308, 641, 643
        };

        //Accessory and Armor created projectiles
        public static Dictionary<int, int> accessoryOrArmorProjectiles = new Dictionary<int, int> {
            { 566, 15 },
            { 567, 30 },
            { 568, 30 },
            { 569, 30 },
            { 570, 30 },
            { 571, 30 },
            { 221, 30 },
            { 227, 100 },
            { 608, 50 },
            { 656, 20 },
            { 556, 60 },
            { 557, 60 },
            { 559, 60 },
            { 560, 60 },
            { 561, 60 }
        };

        //Sets a debuff and its duration to a projectile id
        public static Dictionary<int, BuffDuration> projectileDebuffs = new Dictionary<int, BuffDuration> {
            //Flaming Arrow
            { 2, new BuffDuration(24, 90) },

            //Flamarang, Sunfury
            { 19, new BuffDuration(24, 90) },
            { 35, new BuffDuration(24, 90) },

            //Thorn Chakram
            { 33, new BuffDuration(20, 210) },

            //Poisoned Knife
            { 54, new BuffDuration(20, 300) },

            //Dao of Pow
            { 63, new BuffDuration(31, 60) },

            //Cursed Flames
            { 95, new BuffDuration(39, 210) },

            //Cursed Arrow
            { 103, new BuffDuration(39, 210) },

            //Cursed Bullet
            { 104, new BuffDuration(39, 210) },

            //Frostburn Arrow
            { 172, new BuffDuration(44, 120) },

            //Poison Dart
            { 184, new BuffDuration(20, 450) },

            //Flower of Frost
            { 253, new BuffDuration(44, 120) },

            //Poison Staff
            { 265, new BuffDuration(20, 210) },

            //Poison Dart
            { 267, new BuffDuration(20, 450) },

            //Ichor Arrow
            { 278, new BuffDuration(69, 450) },

            //Ichor Bullet
            { 279, new BuffDuration(69, 450) },

            //Golden Shower
            { 280, new BuffDuration(69, 300) },

            //Venom Arrow
            { 282, new BuffDuration(70, 150) },

            //Venom Bullet
            { 283, new BuffDuration(70, 150) },

            //Nano Bullet
            { 285, new BuffDuration(31, 60) },

            //Golden Bullet
            { 287, new BuffDuration(72, 300) },

            //Inferno Fork Fireball
            { 295, new BuffDuration(24, 360) },

            //Inferno Fork Blast
            { 296, new BuffDuration(24, 360) },

            //Baby Spider
            { 379, new BuffDuration(70, 150) },

            //Venom Staff
            { 355, new BuffDuration(70, 150) },

            //Molotov Cocktail/Fires
            { 399, new BuffDuration(24, 150) },
            { 400, new BuffDuration(24, 150) },
            { 401, new BuffDuration(24, 150) },
            { 402, new BuffDuration(24, 150) },

            //Life Drain
            { 476, new BuffDuration(151, 30) },

            //Cursed Dart
            { 478, new BuffDuration(39, 210) },
            { 480, new BuffDuration(39, 210) },

            //Ichor Dart
            { 479, new BuffDuration(69, 150) },

            //Clinger Staff
            { 482, new BuffDuration(39, 210) },

            //Shadowflame Arrow
            { 495, new BuffDuration(153, 150) },

            //Shadowflame Hex Doll
            { 496, new BuffDuration(153, 240) },

            //Shadowflame Knife
            { 497, new BuffDuration(153, 90) },

            //Wand of Sparking
            { 504, new BuffDuration(24, 90) },

            //Butcher's Chainsaw
            { 509, new BuffDuration(24, 90) },

            //Toxikarp
            { 523, new BuffDuration(20, 300) },

            //Medusa Head
            { 535, new BuffDuration(156, 30) },

            //Cascade
            { 545, new BuffDuration(24, 90) },

            //Amarok
            { 552, new BuffDuration(44, 120) },

            //Hel-Fire
            { 553, new BuffDuration(24, 180) },

            //Spore Sac
            { 567, new BuffDuration(20, 210) },
            { 568, new BuffDuration(20, 210) },
            { 569, new BuffDuration(20, 210) },
            { 570, new BuffDuration(20, 210) },
            { 571, new BuffDuration(20, 210) },

            //Betsy's Wrath
            { 711, new BuffDuration(203, 300) }
        };

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
