using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.Utilities {
    /// <summary>
    /// Mapped database table/column names into constants.
    /// </summary>
    public static class DbConsts {
        public const string ItemTable = "Items";
        public const string ProjectileTable = "Projectiles";
        public const string BuffTable = "Buffs";

        public const string Id = "ID";
        public const string Name = "Name";
        public const string Damage = "Damage";
        public const string Shoot = "Shoot";
        public const string IsShootModded = "IsShootModded";
        public const string ShootSpeed = "ShootSpeed";
        public const string VelocityMultiplier = "VelocityMultiplier";
        public const string Knockback = "Knockback";
        public const string Defense = "Defense";
        public const string InflictBuffId = "InflictBuffID";
        public const string InflictBuffDuration = "InflictBuffDuration";
        public const string ReceiveBuffId = "ReceiveBuffID";
        public const string ReceiveBuffDuration = "ReceiveBuffDuration";
        public const string Wrath = "Wrath"; //%damage multiplier
        public const string Endurance = "Endurance"; //%damage reduction multiplier
        public const string Titan = "Titan"; //%knockback multiplier
    }
    
    public class StringConsts {
        /// <summary>
        /// Gets the table name from a string.
        /// </summary>
        public static bool TryGetTableFromString(string input, out string table) {
            switch (input.ToLower()) {
                case "items":
                case "item":
                case "i":
                    table = DbConsts.ItemTable;
                    break;

                case "projectiles":
                case "projectile":
                case "proj":
                case "p":
                    table = DbConsts.ProjectileTable;
                    break;

                case "buffs":
                case "buff":
                case "b":
                    table = DbConsts.BuffTable;
                    break;

                default:
                    table = input;
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets an attribute's sql column name from a string.
        /// </summary>
        public static bool TryGetAttributeFromString(string input, out string attribute) {
            switch (input.ToLower()) {
                case "name":
                case "n":
                    attribute = DbConsts.Name;
                    break;

                case "damage":
                case "dmg":
                case "d":
                    attribute = DbConsts.Damage;
                    break;

                case "shoot":
                case "s":
                    attribute = DbConsts.Shoot;
                    break;

                case "isshootmodded":
                case "ism":
                    attribute = DbConsts.IsShootModded;
                    break;

                case "shootspeed":
                case "ss":
                    attribute = DbConsts.ShootSpeed;
                    break;

                case "velocitymultiplier":
                case "vm":
                    attribute = DbConsts.VelocityMultiplier;
                    break;

                case "knockback":
                case "kb":
                    attribute = DbConsts.Knockback;
                    break;

                case "defense":
                case "def":
                    attribute = DbConsts.Defense;
                    break;

                case "inflictbuffid":
                case "ibid":
                case "ibi":
                    attribute = DbConsts.InflictBuffId;
                    break;

                case "inflictbuffduration":
                case "ibd":
                    attribute = DbConsts.InflictBuffDuration;
                    break;

                case "receivebuffid":
                case "rbid":
                case "rbi":
                    attribute = DbConsts.ReceiveBuffId;
                    break;

                case "receivebuffduration":
                case "rbd":
                    attribute = DbConsts.ReceiveBuffDuration;
                    break;

                case "titan":
                case "t":
                    attribute = DbConsts.Titan;
                    break;

                case "endurance":
                case "e":
                    attribute = DbConsts.Endurance;
                    break;

                case "wrath":
                case "w":
                    attribute = DbConsts.Wrath;
                    break;

                default:
                    attribute = input;
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Get's a config value's name from an input
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetConfigAttributeFromString(string val) {
            switch (val) {
                case "enableplugin":
                case "ep":
                    return "EnablePlugin";

                case "enabledamagechanges":
                case "edc":
                case "ed":
                case "d":
                    return "EnableDamageChanges";

                case "enablecriticals":
                case "ec":
                case "c":
                    return "EnableCriticals";

                case "enableknockback":
                case "ek":
                case "k":
                    return "EnableKnockback";

                case "enableminions":
                case "em":
                case "m":
                    return "EnableMinions";

                case "enabletooltip":
                case "et":
                    return "EnableTooltip";

                case "enableprojectiledebuffs":
                case "epd":
                case "pd":
                    return "EnableProjectileDebuffs";

                case "enableprojectileselfbuffs":
                case "epsb":
                case "psb":
                    return "EnableProjectileSelfBuffs";

                case "enableweapondebuffs":
                case "ewd":
                case "wd":
                    return "EnableWeaponDebuffs";

                case "enableweaponselfbuffs":
                case "ewsb":
                case "wsb":
                    return "EnableWeaponSelfBuffs";

                case "healthbasedbuffduration":
                case "hbbd":
                case "hbd":
                    return "HealthBasedBuffDuration";

                case "enableturtle":
                case "eturtle":
                case "turtle":
                    return "EnableTurtle";

                case "turtlemultiplier":
                case "turtlem":
                    return "TurtleMultiplier";

                case "enablethorns":
                case "ethorns":
                case "thorns":
                    return "EnableThorns";

                case "thornmultiplier":
                case "thornm":
                    return "ThornMultiplier";

                case "enablenebula":
                case "nebula":
                case "en":
                case "n":
                    return "EnableNebula";

                case "nebulatier3duration":
                case "nt3d":
                case "n3":
                    return "NebulaTier3Duration";

                case "nebulatier2duration":
                case "nt2d":
                case "n2":
                    return "NebulaTier2Duration";

                case "nebulatier1duration":
                case "nt1d":
                case "n1":
                    return "NebulaTier1Duration";

                case "enablefrost":
                case "efrost":
                case "frost":
                case "f":
                    return "EnableFrost";

                case "frostduration":
                case "fd":
                    return "FrostDuration";

                case "vortexmultiplier":
                case "vm":
                    return "VortexMultiplier";

                case "enablebuffdebuff":
                case "ebd":
                    return "EnableBuffDebuff";

                case "enablebuffselfbuff":
                case "ebsb":
                    return "EnableBuffSelfBuff";

                case "knockbackminimum":
                case "km":
                    return "KnockbackMinimum";

                case "iframetime":
                case "ift":
                    return "IframeTime";

                case "lowerdamagevariance":
                case "ldv":
                    return "LowerDamageVariance";

                case "upperdamagevariance":
                case "udv":
                    return "UpperDamageVariance";

                case "deathitemtag":
                case "dit":
                    return "DeathItemTag";

                case "parrytime":
                case "pt":
                    return "ParryTime";

                case "lowermagicdamagepercentage":
                case "lmdp":
                case "lm":
                    return "LowerMagicDamagePercentage";

                case "uppermagicdamagepercentage":
                case "umdp":
                case "um":
                    return "UpperMagicDamagePercentage";

                default:
                    return val;
            }
        }
    }
}
