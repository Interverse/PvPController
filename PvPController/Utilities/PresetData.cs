using System.Collections.Generic;
using PvPController.Variables;

namespace PvPController.Utilities {
    class PresetData {
        //Sets a debuff and its duration to a flask buff
        public static Dictionary<int, BuffInfo> FlaskDebuffs = new Dictionary<int, BuffInfo> {
            //Weapon Imbue Venom
            { 71, new BuffInfo(70, 210) },

            //Weapon Imbue Cursed Flames
            { 73, new BuffInfo(39, 150) },

            //Weapon Imbue Fire
            { 74, new BuffInfo(24, 150) },
                            
            //Weapon Imbue Gold
            { 75,  new BuffInfo(72, 300) },
                         
            //Weapon Imbue Ichor
            { 76, new BuffInfo(69, 300) },
                       
            //Weapon Imbue Nanites
            { 77, new BuffInfo(31, 90) },
                    
            //Weapon Imbue Poison
            { 79, new BuffInfo(20, 300) },
        };

        //Minion IDs
        public static Dictionary<int, int> MinionItem = new Dictionary<int, int> {
            { 373, 2364 }, //Hornet Staff
            { 375, 2365 }, //Imp Staff
            { 377, 2366 }, //Queen Spider Staff
            { 379, 2366 }, //Queen Spider Staff (Baby Spider)
            { 387, 2535 }, //Optic Staff (Retanimini)
            { 388, 2535 }, //Optic Staff (Spazmamini)
            { 390, 2551 }, //Spider Staff
            { 391, 2551 }, //Spider Staff
            { 392, 2551 }, //Spider Staff
            { 393, 2584 }, //Pirate Staff
            { 394, 2584 }, //Pirate Staff
            { 395, 2584 }, //Pirate Staff
            { 191, 1157 }, //Pygmy Staff
            { 192, 1157 }, //Pygmy Staff
            { 193, 1157 }, //Pygmy Staff
            { 194, 1157 }, //Pygmy Staff
            { 423, 2749 }, //Xeno Staff
            { 317, 1802 }, //Raven Staff
            { 407, 2621 }, //Tempest Staff
            { 408, 2621 }, //Tempest Staff
            { 533, 3249 }, //Deadly Sphere Staff
            { 625, 3531 }, //Stardust Dragon Staff
            { 626, 3531 }, //Stardust Dragon Staff
            { 627, 3531 }, //Stardust Dragon Staff
            { 628, 3531 }, //Stardust Dragon Staff
            { 613, 3474 }, //Stardust Cell Staff
            { 614, 3474 }, //Stardust Cell Staff (Mini)
            { 308, 1572 }, //Staff of the Frost Hydra
            { 309, 1572 }, //Staff of the Frost Hydra
            { 641, 3569 }, //Lunar Portal Staff
            { 642, 3569 }, //Lunar Portal Staff
            { 643, 3571 }, //Rainbow Crystal Staff
            { 644, 3571 }, //Rainbow Crystal Staff
            { 663, 3818 }, //Flameburst Rod
			{ 664, 3818 }, //Flameburst Rod
            { 665, 3819 }, //Flameburst Cane
            { 666, 3819 }, //Flameburst Cane
            { 667, 3820 }, //Flameburst Staff
            { 668, 3820 }, //Flameburst Staff
            { 677, 3824 }, //Ballista Rod
            { 678, 3825 }, //Ballista Cane
            { 679, 3826 }, //Ballista Staff
            { 688, 3829 }, //Lightning Aura Rod
            { 689, 3830 }, //Lightning Aura Cane
            { 690, 3831 }, //Lightning Aura Staff
            { 691, 3823 }, //Explosive Trap Rod
            { 692, 3824 }, //Explosive Trap Cane
            { 693, 3825 }, //Explosive Trap Staff
            { 694, 3823 }, //Explosive Trap Rod
            { 695, 3824 }, //Explosive Trap Cane
            { 696, 3825 }, //Explosive Trap Staff
        };

        //Minion Data
        public static Dictionary<int, MinionProjectile> MinionStats = new Dictionary<int, MinionProjectile> {
            { 191, new MinionProjectile(195, 0.5, 40, 12) }, //Pygmy
            { 192, new MinionProjectile(195, 0.5, 40, 12) }, //Pygmy
            { 193, new MinionProjectile(195, 0.5, 40, 12) }, //Pygmy
            { 194, new MinionProjectile(195, 0.5, 40, 12) }, //Pygmy
            { 407, new MinionProjectile(408, 0.6, 50, 14, 0 ,9) }, //Tempest
            { 377, new MinionProjectile(378, 1.0, 30, 9, 30, 10) }, //Spider
            { 308, new MinionProjectile(309, 1.0, 50, 6, 40, 45) }, //Hydra
            { 641, new MinionProjectile(642, 2.0, 30, 0) }, //Lunar Portal
            { 643, new MinionProjectile(644, 1.5, 30, 0) }, //Rainbow
            { 373, new MinionProjectile(374, 0.8, 50, 10, 0, 3) }, //Hornet
            { 375, new MinionProjectile(376, 2.5, 50, 11) }, //Imp
            { 387, new MinionProjectile(389, 0.8, 50, 8) }, //Retanimini
            { 388, new MinionProjectile(389, 0.8, 50, 8) }, //Spazmamini
            { 623, new MinionProjectile(624, 1.2, 6, 0) }, //Stardust Guardian
            { 663, new MinionProjectile(664, 1.7, 50, 12, 0, 18) }, //Flameburst Rod
            { 665, new MinionProjectile(666, 1.7, 50, 15, 0, 18) }, //Flameburst Cane
            { 667, new MinionProjectile(668, 1.7, 50, 18, 0, 18) }, //Flameburst Staff
            { 677, new MinionProjectile(680, 3.5, 50, 16, 0, 16) }, //Ballista Rod
            { 678, new MinionProjectile(680, 3.5, 50, 16, 0, 16) }, //Ballista Cane
            { 679, new MinionProjectile(680, 3.5, 50, 16, 0, 16) }, //Ballista Staff
            { 691, new MinionProjectile(694, 1.0, 5, 0, 7, -24) }, //Explosive Trap Rod
            { 692, new MinionProjectile(695, 1.0, 5, 0, 7, -24) }, //Explosive Trap Cane
            { 693, new MinionProjectile(696, 1.0, 5, 0, 7, -24) },  //Explosive Trap Staff
        };

        //Defines a weapon to a projectile that spawns from another projectile
        public static Dictionary<int, int> FromWhatItem = new Dictionary<int, int> {
            { 19, 119 }, //Flamarang
            { 33, 191 }, //Thorn Chakram
            { 52, 284 }, //Wooden Boomerang
            { 90, 515 }, //Crystal Bullet
            { 92, 516 }, //Holy Arrow
            { 113, 670 }, //Ice Boomerang
            { 150, 788 }, //Nettle Burst
            { 151, 788 }, //Nettle Burst
            { 152, 788 }, //Nettle Burst
            { 182, 1122 }, //Possessed Hatchet
            { 239, 1244 }, //Nimbus Rod
            { 249, 1258 }, //Stynger
            { 250, 1260 }, //Rainbow Gun
            { 251, 1260 }, //Rainbow Gun
            { 264, 1244 }, //Nimbus Rod
            { 272, 1324 }, //Bananarang
            { 296, 1445 }, //Inferno Fork
            { 301, 1513 }, //Paladin's Hammer
            { 307, 1571 }, //Scourge of the Corruptor
            { 320, 1825 }, //Bloody Machete
            { 321, 1826 }, //The Horseman's Blade
            { 344, 1947 }, //North Pole
            { 400, 2590 }, //Molotov Cocktail
            { 401, 2590 }, //Molotov Cocktail
            { 402, 2590 }, //Molotov Cocktail
            { 405, 2611 }, //Flairon
            { 491, 3030 }, //Flying Knife
            { 493, 3051 }, //Crystal Vile Shard
            { 494, 3051 }, //Crystal Vile Shard
            { 511, 3105 }, //Toxic Flask
            { 512, 3105 }, //Toxic Flask
            { 513, 3105 }, //Toxic Flask
            { 522, 3209 }, //Crystal Serpent
            { 604, 3389 }, //Terrarian
            { 617, 3476 }, //Nebula Arcanum
            { 619, 3476 }, //Nebula Arcanum
            { 620, 3476 }, //Nebula Arcanum
            { 640, 3568 }, //Luminite Arrow
            { 480, 3010 } //Cursed Darts
        };

        //Links a hook to its respective item
        public static Dictionary<int, int> ProjHooks = new Dictionary<int, int> {
            { 3, 84 }, //Grappling Hook
            { 32, 185 }, //Ivy Whip
            { 73, 437 }, //Dual Hook (Blue)
            { 74, 437 }, //Dual Hook (Red)
            { 165, 939 }, //Web Slinger
            { 230, 1236 }, //Amethyst Hook
            { 231, 1237 }, //Topaz Hook
            { 232, 1238 }, //Sapphire Hook
            { 233, 1239 }, //Emerald Hook
            { 234, 1240 }, //Ruby Hook
            { 235, 1241 }, //Diamond Hook
            { 256, 1273 }, //Skeletron Hand
            { 315, 1800 }, //Bat Hook
            { 322, 1829 }, //Spooky Hook
            { 331, 1915 }, //Candy Cane Hook
            { 332, 1916 }, //Christmas Hook
            { 372, 2360 }, //Fish Hook
            { 396, 2585 }, //Slime Hook
            { 403, 0 }, //Minecart Hook
            { 446, 2800 }, //Anti-Gravity Hook
            { 646, 3572 }, //Lunar Hook (Solar)
            { 647, 3572 }, //Lunar Hook (Vortex)
            { 648, 3572 }, //Lunar Hook (Nebula)
            { 649, 3572 } //Lunar Hook (Stardust)
        };

        //Accessory and Armor created projectiles
        public static Dictionary<int, int> PresetProjDamage = new Dictionary<int, int> {
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
            { 561, 60 },
            { 624, 70 }
        };

        //Sets a debuff and its duration to a projectile id
        public static Dictionary<int, BuffInfo> ProjectileDebuffs = new Dictionary<int, BuffInfo> {
            //Flaming Arrow
            { 2, new BuffInfo(24, 90) },

            //Flamarang, Sunfury
            { 19, new BuffInfo(24, 90) },
            { 35, new BuffInfo(24, 90) },

            //Thorn Chakram
            { 33, new BuffInfo(20, 210) },

            //Poisoned Knife
            { 54, new BuffInfo(20, 300) },

            //Dao of Pow
            { 63, new BuffInfo(31, 60) },

            //Cursed Flames
            { 95, new BuffInfo(39, 210) },

            //Cursed Arrow
            { 103, new BuffInfo(39, 210) },

            //Cursed Bullet
            { 104, new BuffInfo(39, 210) },

            //Frostburn Arrow
            { 172, new BuffInfo(44, 120) },

            //Poison Dart
            { 184, new BuffInfo(20, 450) },

            //Flower of Frost
            { 253, new BuffInfo(44, 120) },

            //Poison Staff
            { 265, new BuffInfo(20, 210) },

            //Poison Dart
            { 267, new BuffInfo(20, 450) },

            //Ichor Arrow
            { 278, new BuffInfo(69, 450) },

            //Ichor Bullet
            { 279, new BuffInfo(69, 450) },

            //Golden Shower
            { 280, new BuffInfo(69, 300) },

            //Venom Arrow
            { 282, new BuffInfo(70, 150) },

            //Venom Bullet
            { 283, new BuffInfo(70, 150) },

            //Nano Bullet
            { 285, new BuffInfo(31, 60) },

            //Golden Bullet
            { 287, new BuffInfo(72, 300) },

            //Inferno Fork Fireball
            { 295, new BuffInfo(24, 360) },

            //Inferno Fork Blast
            { 296, new BuffInfo(24, 360) },

            //Baby Spider
            { 379, new BuffInfo(70, 150) },

            //Venom Staff
            { 355, new BuffInfo(70, 150) },

            //Molotov Cocktail/Fires
            { 399, new BuffInfo(24, 150) },
            { 400, new BuffInfo(24, 150) },
            { 401, new BuffInfo(24, 150) },
            { 402, new BuffInfo(24, 150) },

            //Life Drain
            { 476, new BuffInfo(151, 30) },

            //Cursed Dart
            { 478, new BuffInfo(39, 210) },
            { 480, new BuffInfo(39, 210) },

            //Ichor Dart
            { 479, new BuffInfo(69, 150) },

            //Clinger Staff
            { 482, new BuffInfo(39, 210) },

            //Shadowflame Arrow
            { 495, new BuffInfo(153, 150) },

            //Shadowflame Hex Doll
            { 496, new BuffInfo(153, 240) },

            //Shadowflame Knife
            { 497, new BuffInfo(153, 90) },

            //Wand of Sparking
            { 504, new BuffInfo(24, 90) },

            //Butcher's Chainsaw
            { 509, new BuffInfo(24, 90) },

            //Toxikarp
            { 523, new BuffInfo(20, 300) },

            //Medusa Head
            { 535, new BuffInfo(156, 30) },

            //Cascade
            { 545, new BuffInfo(24, 90) },

            //Amarok
            { 552, new BuffInfo(44, 120) },

            //Hel-Fire
            { 553, new BuffInfo(24, 180) },

            //Spore Sac
            { 567, new BuffInfo(20, 210) },
            { 568, new BuffInfo(20, 210) },
            { 569, new BuffInfo(20, 210) },
            { 570, new BuffInfo(20, 210) },
            { 571, new BuffInfo(20, 210) },

            //Betsy's Wrath
            { 711, new BuffInfo(203, 300) }
        };

        //Death Message List
        public static List<string> NormalDeathMessages = new List<string> {
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
        public static List<string> ReflectedDeathMessages = new List<string> {
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
