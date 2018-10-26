using PvPController.Variables;
using Terraria;
using Terraria.ID;

namespace PvPController.Variables {
    public class PvPItem : Item {
        public string name { get { return specialName == "" ? Database.GetData<string>(DBConsts.ItemTable, type, DBConsts.Name) : specialName; } }
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
            return Database.GetData<int>(DBConsts.ItemTable, type, DBConsts.Damage);
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
            return new PvPProjectile(Database.GetData<int>(DBConsts.ItemTable, type, DBConsts.Shoot));
        }

        /// <summary>
        /// Gets the knockback of an item.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public float GetKnockback(PvPPlayer owner) {
            return owner.TPlayer.GetWeaponKnockback(this, Database.GetData<float>(DBConsts.ItemTable, type, DBConsts.Knockback));
        }

        /// <summary>
        /// Returns information about an item's debuff.
        /// </summary>
        /// <returns></returns>
        public BuffDuration GetDebuffInfo() {
            return new BuffDuration(Database.GetData<int>(DBConsts.ItemTable, type, DBConsts.InflictBuffID),
                Database.GetData<int>(DBConsts.ItemTable, type, DBConsts.InflictBuffDuration));
        }

        /// <summary>
        /// Returns information about an item's self buff.
        /// </summary>
        /// <returns></returns>
        public BuffDuration GetSelfBuffInfo() {
            return new BuffDuration(Database.GetData<int>(DBConsts.ItemTable, type, DBConsts.ReceiveBuffID),
                Database.GetData<int>(DBConsts.ItemTable, type, DBConsts.ReceiveBuffDuration));
        }
    }
}
