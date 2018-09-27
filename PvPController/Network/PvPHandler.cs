﻿using Microsoft.Xna.Framework;
using PvPController.Variables;
using PvPController.Utilities;
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
            DataHandler.PlayerHurt += OnPlayerHurtted;
            DataHandler.PlayerUpdated += OnPlayerUpdated;
            DataHandler.PvPToggled += OnPvPToggled;
            DataHandler.ProjectileDestroyed += OnProjectileDestroyed;
            DataHandler.ProjectileNew += OnNewProjectile;
        }

        public void Unsubscribe() {
            DataHandler.PlayerHurt -= OnPlayerHurtted;
            DataHandler.PlayerUpdated -= OnPlayerUpdated;
            DataHandler.PvPToggled -= OnPvPToggled;
            DataHandler.ProjectileDestroyed -= OnProjectileDestroyed;
            DataHandler.ProjectileNew -= OnNewProjectile;
        }

        private void OnProjectileDestroyed(object sender, ProjectileDestroyArgs e) {
            ProjectileTracker.RemoveProjectile(e.projectileIndex);
        }

        /// <summary>
        /// Stores projectiles to an array with its originated weapon.
        /// Changes projectile types and shootspeed based off server config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnNewProjectile(object sender, ProjectileNewArgs e) {
            if (!PvPController.config.enablePlugin) return;

            bool isModified = false;
            int index = e.identity;

            if (e.attacker == null || !e.attacker.TPlayer.hostile) return;
            //Resets a minion's timer if another minion of the same type is spawned on the same index
            if (ProjectileTracker.projectiles[e.identity] != null && ProjectileTracker.projectiles[e.identity].type == e.type && MinionUtils.minionStats.ContainsKey(e.type))
                ProjectileTracker.projectiles[e.identity].timer.Dispose();

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
            projectile.identity = e.identity;
            projectile.damage = e.damage;
            projectile.active = true;
            projectile.owner = e.owner;
            projectile.velocity = e.velocity;
            projectile.position = e.position;

            e.args.Handled = isModified;
            if (isModified) NetMessage.SendData(27, -1, -1, null, index, 0.0f, 0.0f, 0.0f, 0, 0, 0);

            ProjectileTracker.InsertProjectile(e.identity, e.type, e.owner, e.weapon);
        }

        /// <summary>
        /// Calculates pvp damage and performs interactions with players, 
        /// such as buffs and other miscellaneous broken vanilla pvp mechanics.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerHurtted(object sender, PlayerHurtArgs e) {
            if (!PvPController.config.enablePlugin) return;

            e.args.Handled = true;

            if (!e.target.CanBeHit()) {
                return;
            }
            
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