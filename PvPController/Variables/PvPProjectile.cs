using Microsoft.Xna.Framework;
using Terraria;
using System.Linq;
using PvPController.Utilities;

namespace PvPController.Variables {
    /// <summary>
    /// The class used to store projectile data. Includes additional methods and variables to
    /// perform pvp based calculations and actions.
    /// </summary>
    public class PvPProjectile : Projectile {
        
        public PvPItem ItemOriginated;
        public PvPPlayer OwnerProjectile;

        public PvPProjectile(int type) {
            this.SetDefaults(type);
            this.identity = -1;
        }

        public PvPProjectile(int type, int identity) {
            this.SetDefaults(type);
            this.identity = identity;
        }

        /// <summary>
        /// Gets the projectile damage based off the database.
        /// </summary>
        /// <returns></returns>
        public int GetConfigDamage() {
            return Database.GetData<int>(DbConsts.ProjectileTable, type, DbConsts.Damage);
        }

        /// <summary>
        /// Gets the debuff information from the database.
        /// </summary>
        /// <returns></returns>
        public BuffInfo GetDebuffInfo() {
            return Database.GetBuffInfo(DbConsts.ProjectileTable, type, true);
        }

        /// <summary>
        /// Gets the self buff information from the database.
        /// </summary>
        /// <returns></returns>
        public BuffInfo GetSelfBuffInfo() {
            return Database.GetBuffInfo(DbConsts.ProjectileTable, type, false);
        }

        /// <summary>
        /// Gets the position of the projectile.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPosition() {
            return ProjectileUtils.GetMainProjectile(this.identity, this.type, this.OwnerProjectile.Index).position;
        }

        /// <summary>
        /// Performs additional actions for projectiles.
        /// </summary>
        public void PerformProjectileAction() {
            switch (type) {
                //Medusa Ray projectile
                case 536:
                    foreach (PvPPlayer pvper in PvPController.PvPers.Where(c => c != null)) {
                        if (OwnerProjectile.Index == pvper.Index) continue;
                        if (!pvper.TPlayer.hostile || pvper.Dead) continue;
                        if (Vector2.Distance(OwnerProjectile.TPlayer.position, pvper.TPlayer.position) <= 300) {
                            if (pvper.CheckMedusa()) {
                                pvper.DamagePlayer(OwnerProjectile, ItemOriginated, pvper.GetDamageDealt(OwnerProjectile, ItemOriginated, this), 0, PvPUtils.IsCrit(OwnerProjectile.GetCrit(ItemOriginated)));
                                pvper.SetBuff(Database.GetBuffInfo(DbConsts.ProjectileTable, 535, true));
                            }
                        }
                    }
                    break;
            }
        }
    }
}
