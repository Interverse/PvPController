using Microsoft.Xna.Framework;
using PvPController.Utilities;
using System.Timers;
using Terraria;
using System;

namespace PvPController.PvPVariables {
    public class PvPProjectile : Projectile {

        public Timer timer = new Timer();
        public PvPItem itemOriginated;
        public PvPPlayer ownerProjectile;

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

        public void SetOwner(int owner) {
            this.owner = owner;
            ownerProjectile = PvPController.pvpers[owner];
        }

        public void SetOriginatedItem(PvPItem item) {
            itemOriginated = item;
        }

        public int GetConfigDamage() {
            return PvPController.database.projectileInfo[type].damage;
        }

        public BuffDuration GetDebuffInfo() {
            return PvPController.database.projectileInfo[type].debuff;
        }

        public BuffDuration GetSelfBuffInfo() {
            return PvPController.database.projectileInfo[type].selfBuff;
        }

        public void PerformProjectileAction() {
            switch (type) {
                case 536:
                    for (int x = 0; x < PvPController.pvpers.Length; x++) {
                        if (ownerProjectile.Index == PvPController.pvpers[x].Index) continue;
                        if (!PvPController.pvpers[x].TPlayer.hostile || PvPController.pvpers[x].Dead) continue;
                        if (Vector2.Distance(ownerProjectile.TPlayer.position, PvPController.pvpers[x].TPlayer.position) <= 300) {
                            if (PvPController.pvpers[x].CheckMedusa()) {
                                PvPController.pvpers[x].DamagePlayer(ownerProjectile, itemOriginated, PvPController.pvpers[x].GetDamageDealt(ownerProjectile, itemOriginated, this), 0, PvPUtils.IsCrit(ownerProjectile.GetCrit(itemOriginated)));
                                PvPController.pvpers[x].SetBuff(PvPController.database.projectileInfo[535].debuff);
                            }
                        }
                    }
                    break;

                default:
                    if (PvPController.config.enableMinions && MiscData.minionStats.ContainsKey(type) && !timer.Enabled) {
                        timer.Interval = MiscData.minionStats[type].fireRate * 1000;
                        timer.Enabled = true;
                    }
                    break;
            }
        }

        private void ProjectileElapsed(object sender, ElapsedEventArgs e) {
            if (!Main.projectile[identity].active && timer.Enabled) {
                timer.Dispose();
                return;
            }

            Vector2 position = Main.projectile[identity].position;

            for (int x = 0; x < PvPController.pvpers.Length; x++) {
                PvPPlayer target = PvPController.pvpers[x];
                int damage = ownerProjectile.GetDamageDealt(ownerProjectile, itemOriginated, this);

                if (ownerProjectile.Index == target.Index) continue;
                if (!target.TPlayer.hostile || target.TPlayer.dead) continue;

                float startX = position.X + MiscData.minionStats[type].offsetX;
                float startY = position.Y + MiscData.minionStats[type].offsetY;

                if (Vector2.Distance(new Vector2(startX, startY), target.TPlayer.position) <= MiscData.minionStats[type].radius * 16) {
                    Vector2 direction = Vector2.Normalize(target.TPlayer.position - position);
                    
                    if (type == 643) {
                        Random random = new Random();
                        startX = target.X + (random.Next(-3, 3) * 16);
                        startY = target.Y + (random.Next(-3, 3) * 16);
                    } else if (type == 623) {
                        startX = target.X + 9;
                        startY = target.Y + 20;
                    }

                    int velocityX = (int)(direction.X * MiscData.minionStats[type].velocity);
                    int velocityY = (int)(direction.Y * MiscData.minionStats[type].velocity);

                    int index = Projectile.NewProjectile(startX, startY, velocityX, velocityY,
                        MiscData.minionStats[type].projectile, damage, target.Index, owner);
                    NetMessage.SendData(27, -1, -1, null, index, 0.0f, 0.0f, 0.0f, 0, 0, 0);

                    PvPController.projectiles[index] = new PvPProjectile(MiscData.minionStats[type].projectile, index);
                    PvPController.projectiles[index].SetOwner(ownerProjectile.Index);
                    PvPController.projectiles[index].SetOriginatedItem(itemOriginated);
                    PvPController.projectiles[index].PerformProjectileAction();
                    
                    return;
                }
            }
        }
    }
}
