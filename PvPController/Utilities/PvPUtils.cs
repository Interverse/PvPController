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
        /// Displays the stats of a player and weapon on the right side of their screen.
        /// Stats include damage, projectile, debuffs and buffs, knockback, criticals, and defense.
        /// </summary>
        /// <param name="player"></param>
        public static void DisplayInterface(PvPPlayer player) {
            StringBuilder sb = new StringBuilder();

            PvPItem weapon = player.GetPlayerItem();
            PvPProjectile projectile = weapon.useAmmo == AmmoID.None
                ? player.GetPlayerItem().GetItemShoot()
                : player.GetFirstAvailableAmmo(weapon).GetItemShoot();

            sb.AppendLine(MiscUtils.LineBreaks(8));
            sb.AppendLine("Weapon and Armor Stats (/toggletooltip or /tt)");
            sb.AppendLine(new string('-', 40));

            if (weapon.GetPvPDamage(player) > 0)
                sb.AppendLine(weapon.name + ": " + weapon.GetPvPDamage(player) + " damage");

            if (PvPController.config.enableWeaponDebuffs)
                if (weapon.GetDebuffInfo().buffid != 0)
                    sb.AppendLine("  Inflicts {0} for {1}s."
                        .SFormat(Lang.GetBuffName(weapon.GetDebuffInfo().buffid), weapon.GetDebuffInfo().buffDuration / 60.0));

            if (PvPController.config.enableWeaponSelfBuffs)
                if (weapon.GetSelfBuffInfo().buffid != 0)
                    sb.AppendLine("  Inflicts {0} to self for {1}s."
                        .SFormat(Lang.GetBuffName(weapon.GetSelfBuffInfo().buffid), weapon.GetSelfBuffInfo().buffDuration / 60.0));

            if (projectile.type != -1) {
                int shoot = projectile.type;
                sb.AppendLine("  Shoots " + Lang.GetProjectileName(shoot).ToString());

                if (PvPController.config.enableProjectileDebuffs)
                    if (projectile.GetDebuffInfo().buffid != 0)
                        sb.AppendLine("    Inflicts {0} for {1}s."
                            .SFormat(Lang.GetBuffName(projectile.GetDebuffInfo().buffid), projectile.GetDebuffInfo().buffDuration / 60.0)); 

                if (PvPController.config.enableProjectileSelfBuffs)
                    if (projectile.GetSelfBuffInfo().buffid != 0)
                        sb.AppendLine("    Inflicts {0} to self for {1}s."
                            .SFormat(Lang.GetBuffName(projectile.GetSelfBuffInfo().buffid), projectile.GetSelfBuffInfo().buffDuration / 60.0));
            }

            for (int x = 0; x < Terraria.Player.maxBuffs; x++) {
                int buffType = player.TPlayer.buffType[x];
                ItemInfo buffInfo = Database.buffInfo[buffType];

                if (PvPController.config.enableBuffDebuff)
                    if (buffInfo.debuff.buffid != 0)
                        sb.AppendLine(MiscUtils.SeparateToLines("Buff {0} applies {1} ({2}s) to weapons."
                            .SFormat(Lang.GetBuffName(buffType), Lang.GetBuffName(buffInfo.debuff.buffid), buffInfo.debuff.buffDuration / 60.0)));

                if (PvPController.config.enableBuffSelfBuff)
                    if (buffInfo.selfBuff.buffid != 0)
                        sb.AppendLine(MiscUtils.SeparateToLines("Buff {0} applies {1} to self for {2}s on attack."
                            .SFormat(Lang.GetBuffName(buffType), Lang.GetBuffName(buffInfo.selfBuff.buffid), buffInfo.selfBuff.buffDuration / 60.0)));
            }

            if (PvPController.config.enableKnockback)
                sb.AppendLine("Knockback: " + player.GetPlayerItem().GetKnockback(player));

            if (PvPController.config.enableCriticals)
                if (player.GetCrit(weapon) > 0)
                    sb.AppendLine("Critical: " + player.GetCrit(weapon) + "%");

            sb.AppendLine("Defense: " + player.GetPlayerDefense());
            sb.AppendLine(MiscUtils.LineBreaks(50));

            player.SendData(PacketTypes.Status, sb.ToString());
        }

        /// <summary>
        /// Sends a empty string to clear the player interface on the right side of the screen.
        /// </summary>
        /// <param name="player"></param>
        public static void ClearInterface(PvPPlayer player) {
            player.SendData(PacketTypes.Status, String.Empty);
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
        /// Brings a brief text pop-up above a person displaying a message.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void PlayerTextPopup(PvPPlayer player, string message, Color color) {
            NetMessage.SendData(119, player.Index, -1, NetworkText.FromLiteral(message), (int)color.PackedValue, player.X, player.Y + 10);
        }

        /// <summary>
        /// Brings a brief text pop-up above a person displaying a message.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void PlayerTextPopup(PvPPlayer player, PvPPlayer target, string message, Color color) {
            NetMessage.SendData(119, player.Index, -1, NetworkText.FromLiteral(message), (int)color.PackedValue, target.X, target.Y + 10);
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
