using Microsoft.Xna.Framework;
using PvPController.PvPVariables;
using PvPController.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace PvPController.PacketHandling {
    public class PvPHandler {
        public PvPHandler() {
            DataHandler.PlayerHurtted += OnPlayerHurtted;
            DataHandler.PlayerUpdated += OnPlayerUpdated;
            DataHandler.ProjectileDestroyed += OnProjectileDestroyed;
            DataHandler.PvPToggled += OnPvPToggled;
            GetDataHandlers.NewProjectile += OnNewProjectile;
        }

        public void Unsubscribe() {
            DataHandler.PlayerHurtted -= OnPlayerHurtted;
            DataHandler.PlayerUpdated -= OnPlayerUpdated;
            DataHandler.ProjectileDestroyed -= OnProjectileDestroyed;
            DataHandler.PvPToggled -= OnPvPToggled;
            GetDataHandlers.NewProjectile -= OnNewProjectile;
        }

        /// <summary>
        /// Stores projectiles to an array with its originated weapon.
        /// Changes projectile types and shootspeed based off server config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnNewProjectile(object sender, GetDataHandlers.NewProjectileEventArgs args) {
            if (!PvPController.config.enablePlugin) return;

            PvPPlayer player = new PvPPlayer(args.Owner);
            Vector2 velocity = args.Velocity;
            int type = args.Type;

            if (player == null || !player.TPlayer.hostile) return;
            //Resets a minion's timer if another minion of the same type is spawned on the same index
            if (PvPController.projectiles[args.Identity] != null && PvPController.projectiles[args.Identity].type == args.Type && MiscData.minionStats.ContainsKey(args.Type))
                PvPController.projectiles[args.Identity].timer.Dispose();

            PvPItem weapon;
            PvPItem ammo = new PvPItem();
            if (MiscData.presetProjDamage.ContainsKey(args.Type)) {
                weapon = new PvPItem();
                weapon.damage = MiscData.presetProjDamage[args.Type];
                weapon.name = Lang.GetProjectileName(args.Type).ToString();
            } else if (MiscData.fromWhatItem.ContainsKey(args.Type)) {
                weapon = player.FindPlayerItem(MiscData.fromWhatItem[args.Type]);
            } else {
                weapon = player.GetPlayerItem();
            }

            if (player.GetFirstAvailableAmmo(weapon).netID > 0) {
                ammo = player.GetFirstAvailableAmmo(weapon);
            }

            if (Database.itemInfo[weapon.netID].shoot > -1 || Database.itemInfo[weapon.netID].shootSpeed > -1) {
                args.Handled = true;
                if (Database.itemInfo[weapon.netID].shoot > 0)
                    type = Database.itemInfo[weapon.netID].shoot;
                if (Database.itemInfo[weapon.netID].shootSpeed > 0)
                    velocity = Vector2.Normalize(args.Velocity) * Database.itemInfo[weapon.netID].shootSpeed;
                int number = Projectile.NewProjectile(args.Position.X, args.Position.Y, velocity.X, velocity.Y, type, args.Damage, 1f, args.Owner, 0.0f, 0.0f);
                NetMessage.SendData(27, -1, -1, null, args.Identity, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            }

            PvPController.projectiles[args.Identity] = new PvPProjectile(type, args.Identity);
            PvPController.projectiles[args.Identity].SetOwner(args.Owner);
            PvPController.projectiles[args.Identity].SetOriginatedItem(weapon);
            PvPController.projectiles[args.Identity].PerformProjectileAction();
        }

        /// <summary>
        /// Calculates pvp damage and interactions with players, such as buffs and other
        /// miscellaneous broken vanilla pvp mechanics.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerHurtted(object sender, PlayerHurtArgs e) {
            if (!PvPController.config.enablePlugin) return;

            if (!e.target.CanBeHit()) {
                e.args.Handled = true;
                return;
            }
            
            e.args.Handled = true;

            e.target.DamagePlayer(e.attacker, e.weapon, e.inflictedDamage, e.knockback, PvPUtils.IsCrit(e.crit));
            e.target.ApplyPvPEffects(e.attacker, e.weapon, e.projectile, e.inflictedDamage);

            if (PvPController.config.enableProjectileDebuffs)
                e.target.SetBuff(Database.projectileInfo[e.playerHitReason.SourceProjectileType].debuff);

            if (PvPController.config.enableProjectileSelfBuffs)
                e.attacker.SetBuff(Database.projectileInfo[e.playerHitReason.SourceProjectileType].selfBuff);

            if (PvPController.config.enableWeaponDebuffs)
                e.target.SetBuff(Database.itemInfo[e.attacker.GetPlayerItem().netID].debuff);

            if (PvPController.config.enableWeaponSelfBuffs)
                e.attacker.SetBuff(Database.itemInfo[e.attacker.GetPlayerItem().netID].selfBuff);

            if (PvPController.config.enableBuffDebuff)
                e.target.ApplyBuffDebuffs(e.attacker, e.weapon);

            if (PvPController.config.enableBuffSelfBuff)
                e.attacker.ApplyBuffSelfBuff();
        }

        /// <summary>
        /// Updates a player's vortex status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerUpdated(object sender, PlayerUpdateArgs e) {
            e.player.TPlayer.vortexStealthActive = (e.pulley & 8) == 8;

            if (!PvPController.config.enablePlugin) return;

            if (e.player.previousSelectedItem != e.selectedSlot) {
                e.player.previousSelectedItem = e.selectedSlot;
            }
        }

        /// <summary>
        /// Resets a projectile to null after it has disappeared.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnProjectileDestroyed(object sender, ProjectileDestroyArgs e) {
            PvPController.projectiles[e.projectileID] = null;
        }

        /// <summary>
        /// Clears a person's interface when they toggle off pvp.
        /// This only activates if a person turns off pvp, not when
        /// they turn it on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPvPToggled(object sender, TogglePvPArgs e) {
            PvPUtils.ClearInterface(e.player);
        }
    }
}
