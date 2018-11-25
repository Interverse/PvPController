using Microsoft.Xna.Framework;
using PvPController.Variables;
using System;
using System.Text;
using PvPController.Utilities;
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
        public static void DisplayInterface(PvPPlayer player) {
            StringBuilder sb = new StringBuilder();

            PvPItem weapon = player.HeldItem;
            PvPProjectile projectile = weapon.useAmmo == AmmoID.None
                ? player.HeldItem.Shoot
                : weapon.Shoot.type > 0
                    ? weapon.Shoot
                    : player.GetFirstAvailableAmmo(weapon).Shoot;

            sb.AppendLine(MiscUtils.LineBreaks(8));
            sb.AppendLine("Weapon and Armor Stats (/toggletooltip or /tt)");
            sb.AppendLine(new string('-', 40));
            
            if (weapon.GetPvPDamage(player) > 0 && weapon.netID != 0)
                sb.AppendLine(weapon.Name + ": " + PvPUtils.GetPvPDamage(player, weapon, projectile) + " damage");

            if (PvPController.Config.EnableWeaponDebuffs)
                if (weapon.Debuff.BuffId != 0)
                    sb.AppendLine("  Inflicts {0} for {1}s."
                        .SFormat(Lang.GetBuffName(weapon.Debuff.BuffId), weapon.Debuff.BuffDuration / 60.0));

            if (PvPController.Config.EnableWeaponSelfBuffs)
                if (weapon.SelfBuff.BuffId != 0)
                    sb.AppendLine("  Inflicts {0} to self for {1}s."
                        .SFormat(Lang.GetBuffName(weapon.SelfBuff.BuffId), weapon.SelfBuff.BuffDuration / 60.0));

            if (projectile.type > 0) {
                int shoot = projectile.type;
                sb.AppendLine("  Shoots " + Lang.GetProjectileName(shoot));

                if (PvPController.Config.EnableProjectileDebuffs)
                    if (projectile.Debuff.BuffId != 0)
                        sb.AppendLine("    Inflicts {0} for {1}s."
                            .SFormat(Lang.GetBuffName(projectile.Debuff.BuffId), projectile.Debuff.BuffDuration / 60.0));

                if (PvPController.Config.EnableProjectileSelfBuffs)
                    if (projectile.SelfBuff.BuffId != 0)
                        sb.AppendLine("    Inflicts {0} to self for {1}s."
                            .SFormat(Lang.GetBuffName(projectile.SelfBuff.BuffId), projectile.SelfBuff.BuffDuration / 60.0));
            }

            for (int x = 0; x < Player.maxBuffs; x++) {
                int buffType = player.TPlayer.buffType[x];
                var debuffInfo = Database.GetBuffInfo(DbConsts.BuffTable, x, true);
                var selfBuffInfo = Database.GetBuffInfo(DbConsts.BuffTable, x, false);

                if (PvPController.Config.EnableBuffDebuff)
                    if (debuffInfo.BuffId != 0)
                        sb.AppendLine("Buff {0} applies {1} ({2}s) to weapons."
                            .SFormat(Lang.GetBuffName(buffType), Lang.GetBuffName(debuffInfo.BuffId), debuffInfo.BuffDuration / 60.0).SeparateToLines());

                if (PvPController.Config.EnableBuffSelfBuff)
                    if (selfBuffInfo.BuffId != 0)
                        sb.AppendLine("Buff {0} applies {1} to self for {2}s on attack."
                            .SFormat(Lang.GetBuffName(buffType), Lang.GetBuffName(selfBuffInfo.BuffId), selfBuffInfo.BuffDuration / 60.0).SeparateToLines());
            }

            if (PvPController.Config.EnableKnockback)
                sb.AppendLine("Knockback: " + player.GetHeldWeaponKnockback);

            if (PvPController.Config.EnableCriticals)
                if (player.GetCrit(weapon) > 0)
                    sb.AppendLine("Critical: " + player.GetCrit(weapon) + "%");

            sb.AppendLine($"Defense: {player.Defense} + {player.DamageReduction * 100}% Damage Reduction");
            sb.AppendLine(MiscUtils.LineBreaks(50));

            player.SendData(PacketTypes.Status, sb.ToString());
        }

        /// <summary>
        /// Sends a empty string to clear the player interface on the right side of the screen.
        /// </summary>
        public static void ClearInterface(PvPPlayer player) {
            player.SendData(PacketTypes.Status, String.Empty);
        }

        /// <summary>
        /// Brings a brief text pop-up above a person displaying a message.
        /// </summary>
        public static void PlayerTextPopup(PvPPlayer player, string message, Color color) {
            NetMessage.SendData(119, player.Index, -1, NetworkText.FromLiteral(message), (int)color.PackedValue, player.X, player.Y + 10);
        }

        /// <summary>
        /// Brings a brief text pop-up above a person displaying a message.
        /// </summary>
        public static void PlayerTextPopup(PvPPlayer player, PvPPlayer target, string message, Color color) {
            NetMessage.SendData(119, player.Index, -1, NetworkText.FromLiteral(message), (int)color.PackedValue, target.X, target.Y + 10);
        }
    }
}
