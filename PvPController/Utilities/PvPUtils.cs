using System;
using PvPController.Variables;
using Terraria;
using TShockAPI;

namespace PvPController.Utilities {
    public class PvPUtils {
        /// <summary>
        /// Generates a death message for a person based off the weapon and type of death.
        /// </summary>
        /// <param Name="type">1 for normal hits, 2 for reflection hits such as thorns and turtle.</param>
        public static string GetPvPDeathMessage(PvPPlayer attacker, PvPPlayer deadplayer, PvPItem weapon, int type = 1) {
            Random random = new Random();
            string deathmessage = "";

            if (type == 1)
                deathmessage = PvPController.Config.NormalDeathMessages[random.Next(PvPController.Config.NormalDeathMessages.Count)];
            else if (type == 2)
                deathmessage = PvPController.Config.ReflectedDeathMessages[random.Next(PvPController.Config.ReflectedDeathMessages.Count)];

            string tag = PvPController.Config.DeathItemTag;
            if (PvPController.Config.DeathItemTag == "weapon" && type == 1) tag = weapon.netID != 0 ? "[i/p{0}:{1}] ".SFormat(weapon.prefix, weapon.netID) : "";
            else if (PvPController.Config.DeathItemTag == "weapon" && type == 2) tag = "[i:1150] ";

            return tag + deadplayer.Name + deathmessage + attacker.Name + "'s " + weapon.Name + ".";
        }
        
        /// <summary>
        /// Generates a boolean for a critical based off the percentage.
        /// </summary>
        public static bool IsCrit(int percentage) => percentage > Main.rand.Next(0, 101);

        /// <summary>
        /// Converts a regular Terraria <see cref="Item"/> into a <see cref="PvPItem"/>.
        /// </summary>
        public static PvPItem ConvertToPvPItem(Item item) => new PvPItem(item);

        /// <summary>
        /// Generates a random int between the lower and upper bounds of damage variance.
        /// </summary>
        public static int GenerateDamageVariance() =>
            new Random().Next(PvPController.Config.LowerDamageVariance, PvPController.Config.UpperDamageVariance + 1);

        /// <summary>
        /// Gets the pvp damage value with modifications from the current database.
        /// </summary>
        public static int GetPvPDamage(PvPPlayer attacker, PvPItem weapon, PvPProjectile projectile = null) {
            int damage = (projectile == null || projectile.ModdedDamage < 1)
                ? weapon.GetPvPDamage(attacker)
                : projectile.ModdedDamage;

            damage += GetAmmoDamage(attacker, weapon);
            damage += GetVortexDamage(attacker, weapon, damage);
            damage += attacker.GetIntBuffArmorIncrease(DbConsts.Damage);
            damage = (int)(damage * (projectile?.Wrath ?? 1 * weapon.Wrath).Replace(0, 1));
            damage = (int)(damage * attacker.GetFloatBuffArmorIncrease(DbConsts.Wrath));
            damage = (int)(damage * GetManaDamagePercentage(attacker, weapon));
            return damage;
        }

        /// <summary>
        /// Gets the damage dealt from the ammo based off the attacker's stats.
        /// </summary>
        public static int GetAmmoDamage(PvPPlayer attacker, PvPItem weapon) {
            int ammoDamage = TerrariaUtils.GetWeaponDamage(attacker, attacker.GetFirstAvailableAmmo(weapon));
            return ammoDamage > 0 ? ammoDamage : 0;
        }

        /// <summary>
        /// Gets a percentage multiplier based off the percentage of mana the player 
        /// currently has and the configuration's lower and upper magic percentage values.
        /// </summary>
        public static double GetManaDamagePercentage(PvPPlayer player, PvPItem weapon) =>
            weapon.magic ? (PvPController.Config.UpperMagicDamagePercentage - PvPController.Config.LowerMagicDamagePercentage) *
                            player.ManaPercentage + PvPController.Config.LowerMagicDamagePercentage
                         : 1.0;

        /// <summary>
        /// Gets the vortex damage multiplier for a player.
        /// </summary>
        public static int GetVortexDamage(PvPPlayer attacker, PvPItem weapon, int damage) {
            double vanillaVortexMultiplier = 1.36;

            if (weapon.ranged && attacker.TPlayer.vortexStealthActive) {
                return (int)(damage / vanillaVortexMultiplier * PvPController.Config.VortexMultiplier - damage);
            }

            return 0;
        }
    }
}
