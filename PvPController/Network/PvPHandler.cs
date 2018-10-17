using Microsoft.Xna.Framework;
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

            if (Database.itemInfo[e.weapon.netID].shoot > 0 && Database.itemInfo[e.weapon.netID].isShootModded) {
                e.type = Database.itemInfo[e.weapon.netID].shoot;
                isModified = true;
            }
            
            if (Database.itemInfo[e.weapon.netID].shootSpeed > 0) {
                e.velocity = Vector2.Normalize(e.velocity) * Database.itemInfo[e.weapon.netID].shootSpeed;
                isModified = true;
            }
            
            var projectile = Main.projectile[index];
            projectile.SetDefaults(e.type);
            projectile.identity = index;
            projectile.damage = e.damage;
            projectile.active = true;
            projectile.owner = e.owner;
            projectile.velocity = e.velocity;
            projectile.position = e.position;

            e.attacker.projTracker.InsertProjectile(index, e.type, e.owner, e.weapon);

            e.args.Handled = isModified;
            if (isModified) NetMessage.SendData(27, -1, -1, null, index, 0.0f, 0.0f, 0.0f, 0, 0, 0);
        }

        /// <summary>
        /// Calculates pvp damage and performs interactions with players, 
        /// such as buffs and other miscellaneous broken vanilla pvp mechanics.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerHurt(object sender, PlayerHurtArgs e) {
            if (!PvPController.config.enablePlugin) return;

            if (e.target == null || !e.target.ConnectionAlive || !e.target.Active) return;

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
