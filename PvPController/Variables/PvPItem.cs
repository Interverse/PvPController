using PvPController.Variables;
using Terraria;
using Terraria.ID;

namespace PvPController.Variables {
    public class PvPItem : Item {
        public string name { get { return specialName == "" ? Database.itemInfo[type].name : specialName; } }
        public string specialName = "";
        public double damage = 0;
        public float knockback = 0;

        public PvPItem() {
            this.SetDefaults();
        }

        public PvPItem(Item item) {
            this.SetDefaults(item.type);
            this.prefix = item.prefix;
            this.damage = item.damage;
            this.knockback = item.knockBack;
        }

        public PvPItem(int type) {
            this.SetDefaults(type);
            this.damage = base.damage;
            this.knockback = base.knockBack;
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
            return owner.TPlayer.GetWeaponKnockback(this, Database.itemInfo[netID].knockback);
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
