using Microsoft.Xna.Framework;
using PvPController.PvPVariables;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;

namespace PvPController.Utilities {
    public class PvPUtils {
        public static string GetPvPDeathMessage(PvPPlayer attacker, PvPPlayer deadplayer, PvPItem weapon, int type = 1) {
            Random random = new Random();
            string deathmessage = "";

            if (type == 1)
                deathmessage = PvPController.config.normalDeathMessages[random.Next(PvPController.config.normalDeathMessages.Count)];
            else if (type == 2)
                deathmessage = PvPController.config.reflectedDeathMessages[random.Next(PvPController.config.reflectedDeathMessages.Count)];

            string tag = PvPController.config.deathItemTag;
            if (PvPController.config.deathItemTag == "weapon" && type == 1) tag = "[i/p{0}:{1}] ".SFormat(weapon.prefix, weapon.netID);
            else if (PvPController.config.deathItemTag == "weapon" && type == 2) tag = "[i:1150] ";

            return tag + deadplayer.Name + deathmessage + attacker.Name + "'s " + weapon.name + ".";
        }

        public static int GetDamageVariance() {
            Random random = new Random();
            return random.Next(PvPController.config.lowerDamageVariance, PvPController.config.upperDamageVariance + 1);
        }

        public static int GetAmmoDamage(PvPPlayer attacker, PvPItem weapon) {
            int ammoDamage = TerrariaUtils.GetWeaponDamage(attacker, attacker.GetFirstAvailableAmmo(weapon));
            return ammoDamage > 0 ? ammoDamage : 0;
        }

        public static int GetVortexDamage(PvPPlayer attacker, PvPItem weapon, int damage) {
            double vanillaVortexMultiplier = 1.36;

            if (weapon.ranged && attacker.TPlayer.vortexStealthActive) {
                return (int)(((double)damage / vanillaVortexMultiplier * PvPController.config.vortexMultiplier) - damage);
            }

            return 0;
        }

        public static bool isCrit(int percentage) {
            Random random = new Random();
            return percentage > random.Next(0, 101);
        }

        public static void PlayerTextPopup(PvPPlayer player, string message, Color color) {
            NetMessage.SendData(119, player.Index, -1, NetworkText.FromLiteral(message), (int)color.PackedValue, player.X, player.Y + 10);
        }

        public static void PlayerTextPopup(PvPPlayer player, PvPPlayer target, string message, Color color) {
            NetMessage.SendData(119, player.Index, -1, NetworkText.FromLiteral(message), (int)color.PackedValue, target.X, target.Y + 10);
        }

        public static void PrintDebugValue(object message) {
            TShock.Utils.Broadcast(message.ToString(), 123, 123, 123);
        }

        public static PvPItem ConvertToPvPItem(Item item) {
            return new PvPItem(item);
        }
    }
}
