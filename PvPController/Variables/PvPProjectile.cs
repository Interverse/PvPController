﻿using Microsoft.Xna.Framework;
using PvPController.Utilities;
using System.Timers;
using Terraria;
using System;
using System.Linq;

namespace PvPController.Variables {
    /// <summary>
    /// The class used to store projectile data. Includes additional methods and variables to
    /// perform pvp based calculations and actions.
    /// </summary>
    public class PvPProjectile : Projectile {

        public Timer timer = new Timer();
        public PvPItem itemOriginated;
        public PvPPlayer ownerProjectile;

        public PvPProjectile() { }

        public PvPProjectile(int type) {
            this.SetDefaults(type);
            this.identity = -1;

            timer.Elapsed += ProjectileElapsed;
        }

        public PvPProjectile(int type, int identity) {
            this.SetDefaults(type);
            this.identity = identity;

            timer.Elapsed += ProjectileElapsed;
        }

        /// <summary>
        /// Sets the owner of the projectile with the owner's index to the server.
        /// </summary>
        /// <param name="owner"></param>
        public void SetOwner(int owner) {
            this.owner = owner;
            ownerProjectile = PvPController.pvpers[owner];
        }

        /// <summary>
        /// Stores the item the projectile was shot from.
        /// </summary>
        /// <param name="item"></param>
        public void SetOriginatedItem(PvPItem item) {
            itemOriginated = item;
        }

        /// <summary>
        /// Gets the projectile damage based off the database.
        /// </summary>
        /// <returns></returns>
        public int GetConfigDamage() {
            return Database.projectileInfo[type].damage;
        }

        /// <summary>
        /// Gets the debuff information from the database.
        /// </summary>
        /// <returns></returns>
        public BuffDuration GetDebuffInfo() {
            return Database.projectileInfo[type].debuff;
        }

        /// <summary>
        /// Gets the self buff information from the database.
        /// </summary>
        /// <returns></returns>
        public BuffDuration GetSelfBuffInfo() {
            return Database.projectileInfo[type].selfBuff;
        }

        /// <summary>
        /// Gets the position of the projectile.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPosition() {
            return ProjectileTracker.GetMainProjectile(this.identity, this.type, this.ownerProjectile.Index).position;
        }

        /// <summary>
        /// Performs additional actions for projectiles.
        /// </summary>
        public void PerformProjectileAction() {
            switch (type) {
                //Medusa Ray projectile
                case 536:
                    for (int x = 0; x < PvPController.pvpers.Length; x++) {
                        if (ownerProjectile.Index == PvPController.pvpers[x].Index) continue;
                        if (!PvPController.pvpers[x].TPlayer.hostile || PvPController.pvpers[x].Dead) continue;
                        if (Vector2.Distance(ownerProjectile.TPlayer.position, PvPController.pvpers[x].TPlayer.position) <= 300) {
                            if (PvPController.pvpers[x].CheckMedusa()) {
                                PvPController.pvpers[x].DamagePlayer(ownerProjectile, itemOriginated, PvPController.pvpers[x].GetDamageDealt(ownerProjectile, itemOriginated, this), 0, PvPUtils.IsCrit(ownerProjectile.GetCrit(itemOriginated)));
                                PvPController.pvpers[x].SetBuff(Database.projectileInfo[535].debuff);
                            }
                        }
                    }
                    break;

                default:
                    if (PvPController.config.enableMinions && MinionUtils.minionStats.ContainsKey(type) && !timer.Enabled) {
                        timer.Interval = MinionUtils.minionStats[type].fireRate * 1000;
                        timer.Enabled = true;
                    }
                    break;
            }
        }

        /// <summary>
        /// Performs timer specific actions for projectiles, such as minions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectileElapsed(object sender, ElapsedEventArgs e) {
            Vector2 position = this.GetPosition();

            if (position == null && timer.Enabled) {
                timer.Dispose();
                return;
            }

            if (PvPController.config.enableMinions) {
                for (int x = 0; x < PvPController.pvpers.Length; x++) {
                    PvPPlayer target = PvPController.pvpers[x];
                    int damage = ownerProjectile.GetDamageDealt(ownerProjectile, itemOriginated, this);

                    if (ownerProjectile.Index == target.Index) continue;
                    if (!target.TPlayer.hostile || target.TPlayer.dead) continue;

                    float startX = position.X + MinionUtils.minionStats[type].offsetX;
                    float startY = position.Y + MinionUtils.minionStats[type].offsetY;

                    if (Vector2.Distance(new Vector2(startX, startY), target.TPlayer.position) <= MinionUtils.minionStats[type].radius * 16) {
                        Vector2 direction = Vector2.Normalize(target.TPlayer.position - position);

                        if (type == 643) {
                            Random random = new Random();
                            startX = target.X + (random.Next(-3, 3) * 16);
                            startY = target.Y + (random.Next(-3, 3) * 16);
                        } else if (type == 623) {
                            startX = target.X + 9;
                            startY = target.Y + 20;
                        }

                        int velocityX = (int)(direction.X * MinionUtils.minionStats[type].velocity);
                        int velocityY = (int)(direction.Y * MinionUtils.minionStats[type].velocity);

                        int index = Projectile.NewProjectile(startX, startY, velocityX, velocityY,
                            MinionUtils.minionStats[type].projectile, damage, target.Index, owner);
                        NetMessage.SendData(27, -1, -1, null, index, 0.0f, 0.0f, 0.0f, 0, 0, 0);

                        ProjectileTracker.InsertProjectile(index, MinionUtils.minionStats[type].projectile, ownerProjectile.Index, ProjectileUtils.GetProjectileWeapon(ownerProjectile, this.type));

                        return;
                    }
                }
            }
        }
    }
}