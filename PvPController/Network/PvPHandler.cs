using System;
using Microsoft.Xna.Framework;
using PvPController.Network.PacketArgs;
using PvPController.Utilities;
using Terraria;

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
        /// <param Name="sender"></param>
        /// <param Name="args"></param>
        private void OnNewProjectile(object sender, ProjectileNewArgs e) {
            if (!PvPController.Config.EnablePlugin) return;
            if (e.Attacker == null || !e.Attacker.TPlayer.hostile || !e.Attacker.ConnectionAlive) return;
            if (PresetData.ProjectileDummy.Contains(e.Type)) return;
            if (Main.projectile[e.Identity].active && Main.projectile[e.Identity].type == e.Type) return;

            var isModified = false;

            if (e.Weapon.Shoot.type > -1 && e.Weapon.IsShootModded) {
                //In case the player lags, it tracks the yet-to-be modified projectile to the current weapon weapon
                e.Attacker.ProjTracker.InsertProjectile(e.Identity, e.Type, e.Owner, e.Weapon);
                e.Type = e.Weapon.Shoot.type;
                isModified = true;
            }

            //If the player is dead and attempts to throw a projectile, the projectile is deleted
            if (e.Attacker.Dead) {
                e.Type = 0;
                isModified = true;
            }

            if (e.Weapon.ShootSpeed > 0) {
                e.Velocity = Vector2.Normalize(e.Velocity) * e.Weapon.ShootSpeed;
                isModified = true;
            }

            if (e.Weapon.VelocityMultiplier != 1 || e.Proj.VelocityMultiplier != 1) {
                e.Velocity = e.Velocity * e.Weapon.VelocityMultiplier * e.Proj.VelocityMultiplier;
                isModified = true;
            }

            if (isModified) {
                e.Args.Handled = true;

                var projectile = Main.projectile[e.Identity];
                projectile.SetDefaults(e.Type);
                projectile.identity = e.Identity;
                projectile.damage = e.Damage;
                projectile.active = true;
                projectile.owner = e.Owner;
                projectile.velocity = e.Velocity;
                projectile.position = e.Position;

                NetMessage.SendData(27, -1, -1, null, e.Identity);
            }

            e.Attacker.ProjTracker.InsertProjectile(e.Identity, e.Type, e.Owner, e.Weapon);
            e.Attacker.ProjTracker.Projectiles[e.Type].PerformProjectileAction();
        }

        /// <summary>
        /// Calculates pvp damage and performs interactions with players,
        /// such as buffs and other miscellaneous broken vanilla pvp mechanics.
        /// </summary>
        /// <param Name="sender"></param>
        /// <param Name="e"></param>
        private void OnPlayerHurt(object sender, PlayerHurtArgs e) {
            if (!PvPController.Config.EnablePlugin) return;
            if (!e.IsPvPDamage) return;

            e.Args.Handled = true;
            
            if (e.Attacker.TPlayer.immune || !e.Target.CanBeHit()) return;

            if (PvPController.Config.EnableKnockback) {
                float knockback = e.Weapon.GetKnockback(e.Attacker);
                if (knockback >= PvPController.Config.KnockbackMinimum) {
                    e.Target.KnockBack(e.Weapon.GetKnockback(e.Attacker),
                        e.Attacker.AngleFrom(e.Target.TPlayer.position),
                        e.Target.IsLeftFrom(e.Attacker.TPlayer.position) ? -e.HitDirection : e.HitDirection);
                    e.HitDirection = 0;
                }
            }

            bool isCrit = PvPController.Config.EnableCriticals && PvPUtils.IsCrit(e.Crit);
            e.Target.DamagePlayer(e.Attacker, e.Weapon, e.InflictedDamage * (isCrit.ToInt() + 1), e.HitDirection, isCrit);
            e.Target.CheckShieldParry();

            if (PvPController.Config.EnableDamageChanges)
                e.Target.ShowDamageHit(e.Attacker, isCrit, e.DamageReceived);

            e.Target.ApplyPvPEffects(e.Attacker, e.Weapon, e.Projectile, e.InflictedDamage);

            if (PvPController.Config.EnableProjectileDebuffs)
                e.Target.SetBuff(Database.GetBuffInfo(DbConsts.ProjectileTable,
                    e.PlayerHitReason.SourceProjectileType, true));

            if (PvPController.Config.EnableProjectileSelfBuffs)
                e.Attacker.SetBuff(Database.GetBuffInfo(DbConsts.ProjectileTable,
                    e.PlayerHitReason.SourceProjectileType, false));

            if (PvPController.Config.EnableWeaponDebuffs)
                e.Target.SetBuff(Database.GetBuffInfo(DbConsts.ItemTable, e.Attacker.HeldItem.netID, true));

            if (PvPController.Config.EnableWeaponSelfBuffs)
                e.Attacker.SetBuff(Database.GetBuffInfo(DbConsts.ItemTable, e.Attacker.HeldItem.netID, false));

            if (PvPController.Config.EnableBuffDebuff)
                e.Target.ApplyBuffDebuffs(e.Attacker, e.Weapon);

            if (PvPController.Config.EnableBuffSelfBuff)
                e.Attacker.ApplyBuffSelfBuff();
        }

        /// <summary>
        /// Updates a player's vortex status.
        /// </summary>
        /// <param Name="sender"></param>
        /// <param Name="e"></param>
        private void OnPlayerUpdated(object sender, PlayerUpdateArgs e) {
            e.Player.TPlayer.vortexStealthActive = e.Pulley.GetBit(3);
            if (e.Pulley.GetBit(5)) e.Player.ShieldRaised = DateTime.Now;

            if (!PvPController.Config.EnablePlugin) return;

            if (e.Player.PreviousSelectedItem != e.SelectedSlot) e.Player.PreviousSelectedItem = e.SelectedSlot;
        }

        /// <summary>
        /// Clears a person's interface when they toggle off pvp.
        /// This only activates if a person turns off pvp, not when
        /// they turn it on.
        /// </summary>
        /// <param Name="sender"></param>
        /// <param Name="e"></param>
        private void OnPvPToggled(object sender, TogglePvPArgs e) {
            Interface.ClearInterface(e.Player);
        }
    }
}