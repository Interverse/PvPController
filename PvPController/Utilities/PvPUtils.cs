using Microsoft.Xna.Framework;
using PvPController.Variables;
using System;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;

namespace PvPController.Utilities {
    public class PvPUtils {

        /// <summary>
        /// Generates a death message for a person based off the weapon and type of death.
        /// </summary>
        /// <param name="attacker">The person inflicting the hit.</param>
        /// <param name="deadplayer">The target receiving the death message.</param>
        /// <param name="weapon">The weapon used to hit the target.</param>
        /// <param name="type">1 for normal hits, 2 for reflection hits such as thorns and turtle.</param>
        /// <returns>A string of the death message.</returns>
        public static string GetPvPDeathMessage(PvPPlayer attacker, PvPPlayer deadplayer, PvPItem weapon, int type = 1) {
            Random random = new Random();
            string deathmessage = "";

            if (type == 1)
                deathmessage = PvPController.config.normalDeathMessages[random.Next(PvPController.config.normalDeathMessages.Count)];
            else if (type == 2)
                deathmessage = PvPController.config.reflectedDeathMessages[random.Next(PvPController.config.reflectedDeathMessages.Count)];

            string tag = PvPController.config.deathItemTag;
            if (PvPController.config.deathItemTag == "weapon" && type == 1) tag = weapon.netID != 0 ? "[i/p{0}:{1}] ".SFormat(weapon.prefix, weapon.netID) : "";
            else if (PvPController.config.deathItemTag == "weapon" && type == 2) tag = "[i:1150] ";

            return tag + deadplayer.Name + deathmessage + attacker.Name + "'s " + weapon.name + ".";
        }

        /// <summary>
        /// Generates a random int between the lower and upper bounds of damage variance.
        /// </summary>
        /// <returns></returns>
        public static int GenerateDamageVariance() {
            Random random = new Random();
            return random.Next(PvPController.config.lowerDamageVariance, PvPController.config.upperDamageVariance + 1);
        }

        /// <summary>
        /// Gets the damage dealt from the ammo based off the attacker's stats.
        /// </summary>
        /// <param name="attacker">The person to calculate the ammo damage for.</param>
        /// <param name="weapon">The weapon the ammo is used for.</param>
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
                return (int)(((double)damage / vanillaVortexMultiplier * PvPController.config.vortexMultiplier) - damage);
            }

            return 0;
        }

        /// <summary>
        /// Generates a boolean for a critical based off the percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static bool IsCrit(int percentage) {
            Random random = new Random();
            return percentage > random.Next(0, 101);
        }

        /// <summary>
        /// Primarily plugin use. Shortcut method to allow the plugin to display a message to the server.
        /// </summary>
        /// <param name="message"></param>
        public static void PrintDebugValue(object message) {
            TShock.Utils.Broadcast(message.ToString(), 123, 123, 123);
        }

        /// <summary>
        /// Converts a regular Terraria <see cref="Item"/> into a <see cref="PvPItem"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static PvPItem ConvertToPvPItem(Item item) {
            return new PvPItem(item);
        }
    }
}
