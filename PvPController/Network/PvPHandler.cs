using Microsoft.Xna.Framework;
using PvPController.Network.PacketArgs;
using PvPController.Variables;
using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace PvPController.Network {
    public class PvPHandler {
        public PvPHandler() {
            DataHandler.PlayerHurt += OnPlayerHurt;
            DataHandler.PlayerUpdated += OnPlayerUpdated;
            DataHandler.PvPToggled += OnPvPToggled;
            DataHandler.ProjectileNew += OnNewProjectile;
        }

        public void Unsubscribe() {
            DataHandler.PlayerHurt -= OnPlayerHurt;
            DataHandler.PlayerUpdated -= OnPlayerUpdated;
            DataHandler.PvPToggled -= OnPvPToggled;
            DataHandler.ProjectileNew -= OnNewProjectile;
        }

        /// <summary>
        /// Stores projectiles to an array with its originated weapon.
        /// Changes projectile types and shootspeed based off server config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnNewProjectile(object sender, ProjectileNewArgs e) {
            if (!PvPController.config.enablePlugin) return;

            if (Main.projectile[e.identity].active && Main.projectile[e.identity].type == e.type) return;

            bool isModified = false;
            int index = ProjectileUtils.FindFreeIndex();

            if (e.attacker == null || !e.attacker.TPlayer.hostile) return;
            
            if (Database.GetData<int>(DBConsts.ItemTable, e.weapon.netID, DBConsts.Shoot) > 0 && Database.GetData<bool>(DBConsts.ItemTable, e.weapon.netID, DBConsts.IsShootModded)) {
                e.type = Database.GetData<int>(DBConsts.ItemTable, e.weapon.netID, DBConsts.Shoot);
                isModified = true;
            }
            
            if (Database.GetData<float>(DBConsts.ItemTable, e.weapon.netID, DBConsts.ShootSpeed) > 0) {
                e.velocity = Vector2.Normalize(e.velocity) * Database.GetData<float>(DBConsts.ItemTable, e.weapon.netID, DBConsts.ShootSpeed);
                isModified = true;
            }

            if (isModified) {
                e.args.Handled = true;

                var projectile = Main.projectile[index];
                projectile.SetDefaults(e.type);
                projectile.identity = index;
                projectile.damage = e.damage;
                projectile.active = true;
                projectile.owner = e.owner;
                projectile.velocity = e.velocity;
                projectile.position = e.position;

                NetMessage.SendData(27, -1, -1, null, index);
            }

            e.attacker.projTracker.InsertProjectile(index, e.type, e.owner, e.weapon);
            e.attacker.projTracker.projectiles[e.type].PerformProjectileAction();
        }

        /// <summary>
        /// Calculates pvp damage and performs interactions with players, 
        /// such as buffs and other miscellaneous broken vanilla pvp mechanics.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerHurt(object sender, PlayerHurtArgs e) {
            if (!PvPController.config.enablePlugin) return;

            if (!e.isPvPDamage) return;

            e.args.Handled = true;

            if (!e.target.CanBeHit()) return;

            if (PvPController.config.enableKnockback) {
                float knockback = e.weapon.GetKnockback(e.attacker);
                if (knockback >= PvPController.config.knockbackMinimum) {
                    e.target.KnockBack(e.weapon.GetKnockback(e.attacker), e.attacker.GetAngleFrom(e.target.TPlayer.position), e.target.IsLeftFrom(e.attacker.TPlayer.position) ? -e.hitDirection : e.hitDirection);
                    e.hitDirection = 0;
                }
            }
            e.target.DamagePlayer(e.attacker, e.weapon, e.inflictedDamage, e.hitDirection, PvPController.config.enableCriticals ? PvPUtils.IsCrit(e.crit) : false);

            e.target.ApplyPvPEffects(e.attacker, e.weapon, e.projectile, e.inflictedDamage);

            if (PvPController.config.enableProjectileDebuffs)
                e.target.SetBuff(Database.GetBuffDuration(DBConsts.ProjectileTable, e.playerHitReason.SourceProjectileType, true));

            if (PvPController.config.enableProjectileSelfBuffs)
                e.attacker.SetBuff(Database.GetBuffDuration(DBConsts.ProjectileTable, e.playerHitReason.SourceProjectileType, false));

            if (PvPController.config.enableWeaponDebuffs)
                e.target.SetBuff(Database.GetBuffDuration(DBConsts.ItemTable, e.attacker.GetPlayerItem().netID, true));

            if (PvPController.config.enableWeaponSelfBuffs)
                e.attacker.SetBuff(Database.GetBuffDuration(DBConsts.ItemTable, e.attacker.GetPlayerItem().netID, false));

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
        /// Clears a person's interface when they toggle off pvp.
        /// This only activates if a person turns off pvp, not when
        /// they turn it on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPvPToggled(object sender, TogglePvPArgs e) {
            Interface.ClearInterface(e.player);
        }
    }
}
