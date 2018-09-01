using Microsoft.Xna.Framework;
using PvPController.PvPVariables;
using System;
using System.Text;
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

            if (projectile.type != 0) {
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
                ItemInfo buffInfo = PvPController.database.buffInfo[buffType];
                
                if (PvPController.config.enableBuffDebuff)
                    if (buffInfo.debuff.buffid != 0)
                        sb.AppendLine(MiscUtils.SeparateToLines("Buff {0} applies {1} to weapons for {2}s."
                            .SFormat(Lang.GetBuffName(buffType), Lang.GetBuffName(buffInfo.debuff.buffid), buffInfo.debuff.buffDuration / 60.0)));

                if (PvPController.config.enableBuffSelfBuff)
                    if (buffInfo.selfBuff.buffid != 0)
                        sb.AppendLine(MiscUtils.SeparateToLines("Buff {0} applies {1} to self for {2}s on attack."
                            .SFormat(Lang.GetBuffName(buffType), Lang.GetBuffName(buffInfo.selfBuff.buffid), buffInfo.selfBuff.buffDuration / 60.0)));
            }

            if (PvPController.config.enableKnockback)
                sb.AppendLine("Knockback: " + player.GetPlayerItem().GetKnockback(player));
            sb.AppendLine("Defense: " + player.GetPlayerDefense());
            sb.AppendLine(MiscUtils.LineBreaks(50));

            player.SendData(PacketTypes.Status, sb.ToString());
        }

        public static void ClearInterface(PvPPlayer player) {
            player.SendData(PacketTypes.Status, String.Empty);
        }

        public static bool IsCrit(int percentage) {
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
