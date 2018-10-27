using System;
using PvPController.Variables;
using Terraria;
using TShockAPI;

namespace PvPController.Utilities {
    public class PvPUtils {

        /// <summary>
        /// Generates a death message for a person based off the weapon and type of death.
        /// </summary>
        /// <param Name="attacker">The person inflicting the hit.</param>
        /// <param Name="deadplayer">The target receiving the death message.</param>
        /// <param Name="weapon">The weapon used to hit the target.</param>
        /// <param Name="type">1 for normal hits, 2 for reflection hits such as thorns and turtle.</param>
        /// <returns>A string of the death message.</returns>
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
        /// Generates a random int between the lower and upper bounds of damage variance.
        /// </summary>
        /// <returns></returns>
        public static int GenerateDamageVariance() {
            Random random = new Random();
            return random.Next(PvPController.Config.LowerDamageVariance, PvPController.Config.UpperDamageVariance + 1);
        }

        /// <summary>
        /// Gets the damage dealt from the ammo based off the attacker's stats.
        /// </summary>
        /// <param Name="attacker">The person to calculate the ammo damage for.</param>
        /// <param Name="weapon">The weapon the ammo is used for.</param>
        /// <returns></returns>
        public static int GetAmmoDamage(PvPPlayer attacker, PvPItem weapon) {
            int ammoDamage = TerrariaUtils.GetWeaponDamage(attacker, attacker.GetFirstAvailableAmmo(weapon));
            return ammoDamage > 0 ? ammoDamage : 0;
        }

        /// <summary>
        /// Gets the vortex damage multiplier for a player.
        /// </summary>
        /// <returns></returns>
        public static int GetVortexDamage(PvPPlayer attacker, PvPItem weapon, int damage) {
            double vanillaVortexMultiplier = 1.36;

            if (weapon.ranged && attacker.TPlayer.vortexStealthActive) {
                return (int)(damage / vanillaVortexMultiplier * PvPController.Config.VortexMultiplier - damage);
            }

            return 0;
        }

        /// <summary>
        /// Generates a boolean for a critical based off the percentage.
        /// </summary>
        /// <param Name="percentage"></param>
        /// <returns></returns>
        public static bool IsCrit(int percentage) {
            Random random = new Random();
            return percentage > random.Next(0, 101);
        }

        /// <summary>
        /// Primarily plugin use. Shortcut method to allow the plugin to display a message to the server.
        /// </summary>
        /// <param Name="message"></param>
        public static void PrintDebugValue(object message) {
            TShock.Utils.Broadcast(message.ToString(), 123, 123, 123);
        }

        /// <summary>
        /// Converts a regular Terraria <see cref="Item"/> into a <see cref="PvPItem"/>.
        /// </summary>
        /// <param Name="item"></param>
        /// <returns></returns>
        public static PvPItem ConvertToPvPItem(Item item) {
            return new PvPItem(item);
        }
    }
}
