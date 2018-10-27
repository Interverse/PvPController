using PvPController.Utilities;
using Terraria;

namespace PvPController.Variables {
    public class PvPItem : Item {
        public string name => SpecialName == "" ? Database.GetData<string>(DbConsts.ItemTable, type, DbConsts.Name) : SpecialName;
        public string SpecialName = "";
        public double Damage;
        public float Knockback;

        public PvPItem() {
            SetDefaults();
        }

        public PvPItem(Item item) {
            SetDefaults(item.type);
            prefix = item.prefix;
            Damage = item.damage;
            Knockback = item.knockBack;
        }

        public PvPItem(int type) {
            SetDefaults(type);
            Damage = damage;
            Knockback = knockBack;
        }

        /// <summary>
        /// Gets damage based off server config.
        /// </summary>
        /// <returns></returns>
        public int GetConfigDamage => Database.GetData<int>(DbConsts.ItemTable, type, DbConsts.Damage);

        /// <summary>
        /// Gets raw damage based off Terraria damage calculations.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public int GetPvPDamage(PvPPlayer owner) => TerrariaUtils.GetWeaponDamage(owner, this);

        /// <summary>
        /// Gets the projectile shot by an item.
        /// </summary>
        /// <returns></returns>
        public PvPProjectile GetItemShoot => new PvPProjectile(Database.GetData<int>(DbConsts.ItemTable, type, DbConsts.Shoot));

        /// <summary>
        /// Gets the knockback of an item.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public float GetKnockback(PvPPlayer owner) =>
            owner.TPlayer.GetWeaponKnockback(this, Database.GetData<float>(DbConsts.ItemTable, type, DbConsts.Knockback));

        /// <summary>
        /// Returns information about an item's debuff.
        /// </summary>
        /// <returns></returns>
        public BuffInfo GetDebuffInfo =>
            new BuffInfo(Database.GetData<int>(DbConsts.ItemTable, type, DbConsts.InflictBuffId),
                Database.GetData<int>(DbConsts.ItemTable, type, DbConsts.InflictBuffDuration));

        /// <summary>
        /// Returns information about an item's self buff.
        /// </summary>
        /// <returns></returns>
        public BuffInfo GetSelfBuffInfo =>
            new BuffInfo(Database.GetData<int>(DbConsts.ItemTable, type, DbConsts.ReceiveBuffId),
                Database.GetData<int>(DbConsts.ItemTable, type, DbConsts.ReceiveBuffDuration));
    }
}
