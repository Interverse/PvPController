using PvPController.Utilities;
using Terraria;
using Terraria.ID;

namespace PvPController.PvPVariables {
    public class PvPItem : Item {
        public string name = "";
        public double damage = 0;
        public float knockback = 0;

        public PvPItem() {
            this.SetDefaults();
            name = this.Name;
        }

        public PvPItem(Item item) {
            this.SetDefaults(item.type);
            this.prefix = item.prefix;
            this.damage = item.damage;
            this.knockback = item.knockBack;
            name = this.Name;
        }

        public PvPItem(int type) {
            this.SetDefaults(type);
            this.damage = base.damage;
            this.knockback = base.knockBack;
            name = this.Name;
        }

        /// <summary>
        /// Gets damage based off server config.
        /// </summary>
        /// <returns></returns>
        public int GetConfigDamage() {
            return Database.itemInfo[type].damage;
        }

        /// <summary>
        /// Gets raw damage based off Terraria damage calculations.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public int GetPvPDamage(PvPPlayer owner) {
            return TerrariaUtils.GetWeaponDamage(owner, this);
        }

        /// <summary>
        /// Gets the projectile shot by an item.
        /// </summary>
        /// <returns></returns>
        public PvPProjectile GetItemShoot() {
            return new PvPProjectile(Database.itemInfo[type].shoot);
        }

        /// <summary>
        /// Gets the knockback of an item.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public float GetKnockback(PvPPlayer owner) {
            return owner.TPlayer.GetWeaponKnockback(this, knockback);
        }

        /// <summary>
        /// Gets the projectile type based off a weapon. If a weapon uses ammo, it
        /// gets the projectile id of the first available ammo.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public int GetShootProjectileType(PvPPlayer owner) {
            return this.useAmmo == AmmoID.None
                ? owner.GetPlayerItem().GetItemShoot().type
                : owner.GetFirstAvailableAmmo(this).GetItemShoot().type;
        }

        /// <summary>
        /// Returns information about an item's debuff.
        /// </summary>
        /// <returns></returns>
        public BuffDuration GetDebuffInfo() {
            return Database.itemInfo[type].debuff;
        }

        /// <summary>
        /// Returns information about an item's self buff.
        /// </summary>
        /// <returns></returns>
        public BuffDuration GetSelfBuffInfo() {
            return Database.itemInfo[type].selfBuff;
        }
    }
}
