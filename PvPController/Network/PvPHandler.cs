using Microsoft.Xna.Framework;
using PvPController.Network.PacketArgs;
using PvPController.Utilities;
using PvPController.Variables;
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
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnNewProjectile(object sender, ProjectileNewArgs e) {
            if (!PvPController.Config.EnablePlugin) return;

            if (Main.projectile[e.Identity].active && Main.projectile[e.Identity].type == e.Type) return;

            var isModified = false;
            int index = ProjectileUtils.FindFreeIndex();

            if (e.Attacker == null || !e.Attacker.TPlayer.hostile) return;

            if (Database.GetData<int>(DbConsts.ItemTable, e.Weapon.netID, DbConsts.Shoot) > 0 &&
                Database.GetData<bool>(DbConsts.ItemTable, e.Weapon.netID, DbConsts.IsShootModded)) {
                e.Type = Database.GetData<int>(DbConsts.ItemTable, e.Weapon.netID, DbConsts.Shoot);
                isModified = true;
            }

            if (Database.GetData<float>(DbConsts.ItemTable, e.Weapon.netID, DbConsts.ShootSpeed) > 0) {
                e.Velocity = Vector2.Normalize(e.Velocity) *
                             Database.GetData<float>(DbConsts.ItemTable, e.Weapon.netID, DbConsts.ShootSpeed);
                isModified = true;
            }

            if (isModified) {
                e.Args.Handled = true;

                var projectile = Main.projectile[index];
                projectile.SetDefaults(e.Type);
                projectile.identity = index;
                projectile.damage = e.Damage;
                projectile.active = true;
                projectile.owner = e.Owner;
                projectile.velocity = e.Velocity;
                projectile.position = e.Position;

                NetMessage.SendData(27, -1, -1, null, index);
            }

            e.Attacker.ProjTracker.InsertProjectile(index, e.Type, e.Owner, e.Weapon);
            e.Attacker.ProjTracker.Projectiles[e.Type].PerformProjectileAction();
        }

        /// <summary>
        /// Calculates pvp damage and performs interactions with players,
        /// such as buffs and other miscellaneous broken vanilla pvp mechanics.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerHurt(object sender, PlayerHurtArgs e) {
            if (!PvPController.Config.EnablePlugin) return;

            if (!e.IsPvPDamage) return;

            e.Args.Handled = true;

            if (!e.Target.CanBeHit()) return;

            if (PvPController.Config.EnableKnockback) {
                float knockback = e.Weapon.GetKnockback(e.Attacker);
                if (knockback >= PvPController.Config.KnockbackMinimum) {
                    e.Target.KnockBack(e.Weapon.GetKnockback(e.Attacker),
                        e.Attacker.GetAngleFrom(e.Target.TPlayer.position),
                        e.Target.IsLeftFrom(e.Attacker.TPlayer.position) ? -e.HitDirection : e.HitDirection);
                    e.HitDirection = 0;
                }
            }

            e.Target.DamagePlayer(e.Attacker, e.Weapon, e.InflictedDamage, e.HitDirection,
                PvPController.Config.EnableCriticals && PvPUtils.IsCrit(e.Crit));

            e.Target.ApplyPvPEffects(e.Attacker, e.Weapon, e.Projectile, e.InflictedDamage);

            if (PvPController.Config.EnableProjectileDebuffs)
                e.Target.SetBuff(Database.GetBuffInfo(DbConsts.ProjectileTable,
                    e.PlayerHitReason.SourceProjectileType, true));

            if (PvPController.Config.EnableProjectileSelfBuffs)
                e.Attacker.SetBuff(Database.GetBuffInfo(DbConsts.ProjectileTable,
                    e.PlayerHitReason.SourceProjectileType, false));

            if (PvPController.Config.EnableWeaponDebuffs)
                e.Target.SetBuff(Database.GetBuffInfo(DbConsts.ItemTable, e.Attacker.GetPlayerItem.netID, true));

            if (PvPController.Config.EnableWeaponSelfBuffs)
                e.Attacker.SetBuff(
                    Database.GetBuffInfo(DbConsts.ItemTable, e.Attacker.GetPlayerItem.netID, false));

            if (PvPController.Config.EnableBuffDebuff)
                e.Target.ApplyBuffDebuffs(e.Attacker, e.Weapon);

            if (PvPController.Config.EnableBuffSelfBuff)
                e.Attacker.ApplyBuffSelfBuff();
        }

        /// <summary>
        /// Updates a player's vortex status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerUpdated(object sender, PlayerUpdateArgs e) {
            e.Player.TPlayer.vortexStealthActive = (e.Pulley & 8) == 8;

            if (!PvPController.Config.EnablePlugin) return;

            if (e.Player.PreviousSelectedItem != e.SelectedSlot) e.Player.PreviousSelectedItem = e.SelectedSlot;
        }

        /// <summary>
        /// Clears a person's interface when they toggle off pvp.
        /// This only activates if a person turns off pvp, not when
        /// they turn it on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPvPToggled(object sender, TogglePvPArgs e) {
            Interface.ClearInterface(e.Player);
        }
    }
}