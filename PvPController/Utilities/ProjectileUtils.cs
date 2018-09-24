using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace PvPController.Utilities {
    public class ProjectileUtils {
        public static PvPItem GetProjectileWeapon(PvPPlayer owner, int type) {
            PvPItem weapon;
            if (ProjectileUtils.presetProjDamage.ContainsKey(type)) {
                weapon = new PvPItem();
                weapon.damage = ProjectileUtils.presetProjDamage[type];
                weapon.name = Lang.GetProjectileName(type).ToString();
            } else if (ProjectileUtils.fromWhatItem.ContainsKey(type)) {
                weapon = owner.FindPlayerItem(ProjectileUtils.fromWhatItem[type]);
            } else if (MinionUtils.minionItem.ContainsKey(type)) {
                weapon = owner.FindPlayerItem(MinionUtils.minionItem[type]);
            } else  {
                weapon = owner.GetPlayerItem();
            }
            return weapon;
        }

        //Defines a weapon to a projectile that spawns from another projectile
        public static Dictionary<int, int> fromWhatItem = new Dictionary<int, int> {
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
            { 443, 2796 }, //Electrosphere Launcher
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
            { 649, 3572 }, //Lunar Hook (Stardust)
            { 640, 3568 }, //Luminite Arrow
            { 480, 3010 } //Cursed Darts
        };
        
        //Accessory and Armor created projectiles
        public static Dictionary<int, int> presetProjDamage = new Dictionary<int, int> {
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
    }
}
