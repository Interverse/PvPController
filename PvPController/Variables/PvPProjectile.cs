using Microsoft.Xna.Framework;
using PvPController.Variables;
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
        
        public PvPItem itemOriginated;
        public PvPPlayer ownerProjectile;

        public PvPProjectile(int type) {
            this.SetDefaults(type);
            this.identity = -1;
        }

        public PvPProjectile(int type, int identity) {
            this.SetDefaults(type);
            this.identity = identity;
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
            return Database.GetData<int>(DBConsts.ProjectileTable, type, DBConsts.Damage);
        }

        /// <summary>
        /// Gets the debuff information from the database.
        /// </summary>
        /// <returns></returns>
        public BuffDuration GetDebuffInfo() {
            return Database.GetBuffDuration(DBConsts.ProjectileTable, type, true);
        }

        /// <summary>
        /// Gets the self buff information from the database.
        /// </summary>
        /// <returns></returns>
        public BuffDuration GetSelfBuffInfo() {
            return Database.GetBuffDuration(DBConsts.ProjectileTable, type, false);
        }

        /// <summary>
        /// Gets the position of the projectile.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPosition() {
            return ProjectileUtils.GetMainProjectile(this.identity, this.type, this.ownerProjectile.Index).position;
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
                                PvPController.pvpers[x].SetBuff(Database.GetBuffDuration(DBConsts.ProjectileTable, 535, true));
                            }
                        }
                    }
                    break;
            }
        }
    }
}
