using Microsoft.Xna.Framework;
using PvPController.Variables;
using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;

namespace PvPController {
    public class Interface {
        /// <summary>
        /// Displays the stats of a player and weapon on the right side of their screen.
        /// Stats include damage, projectile, debuffs and buffs, knockback, criticals, and defense.
        /// </summary>
        /// <param name="player"></param>
        public static void DisplayInterface(PvPPlayer player) { //TODO: Fix Interface
            StringBuilder sb = new StringBuilder();

            PvPItem weapon = player.GetPlayerItem();
            PvPProjectile projectile = weapon.useAmmo == AmmoID.None
                ? player.GetPlayerItem().GetItemShoot()
                : weapon.GetItemShoot().type > 0
                    ? weapon.GetItemShoot()
                    : player.GetFirstAvailableAmmo(weapon).GetItemShoot();

            sb.AppendLine(MiscUtils.LineBreaks(8));
            sb.AppendLine("Weapon and Armor Stats (/toggletooltip or /tt)");
            sb.AppendLine(new string('-', 40));

            if (weapon.GetPvPDamage(player) > 0 && weapon.netID != 0)
                sb.AppendLine(weapon.name + ": " + weapon.GetPvPDamage(player) + " damage");

            if (PvPController.config.enableWeaponDebuffs)
                if (weapon.GetDebuffInfo().buffid != 0)
                    sb.AppendLine("  Inflicts {0} for {1}s."
                        .SFormat(Lang.GetBuffName(weapon.GetDebuffInfo().buffid), weapon.GetDebuffInfo().buffDuration / 60.0));

            if (PvPController.config.enableWeaponSelfBuffs)
                if (weapon.GetSelfBuffInfo().buffid != 0)
                    sb.AppendLine("  Inflicts {0} to self for {1}s."
                        .SFormat(Lang.GetBuffName(weapon.GetSelfBuffInfo().buffid), weapon.GetSelfBuffInfo().buffDuration / 60.0));

            if (projectile.type > 0) {
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
                var debuffInfo = Database.GetBuffDuration(DBConsts.BuffTable, x, true);
                var selfBuffInfo = Database.GetBuffDuration(DBConsts.BuffTable, x, false);

                if (PvPController.config.enableBuffDebuff)
                    if (debuffInfo.buffid != 0)
                        sb.AppendLine(MiscUtils.SeparateToLines("Buff {0} applies {1} ({2}s) to weapons."
                            .SFormat(Lang.GetBuffName(buffType), Lang.GetBuffName(debuffInfo.buffid), debuffInfo.buffDuration / 60.0)));

                if (PvPController.config.enableBuffSelfBuff)
                    if (selfBuffInfo.buffid != 0)
                        sb.AppendLine(MiscUtils.SeparateToLines("Buff {0} applies {1} to self for {2}s on attack."
                            .SFormat(Lang.GetBuffName(buffType), Lang.GetBuffName(selfBuffInfo.buffid), selfBuffInfo.buffDuration / 60.0)));
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
    }
}
