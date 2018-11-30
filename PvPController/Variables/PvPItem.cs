using PvPController.Utilities;
using Terraria;

namespace PvPController.Variables {
    public class PvPItem : Item {
        public string Name => SpecialName == "" ? Database.GetData<string>(DbConsts.ItemTable, type, DbConsts.Name) : SpecialName;
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
        public int GetConfigDamage => Database.GetData<int>(DbConsts.ItemTable, type, DbConsts.Damage);

        /// <summary>
        /// Gets raw damage based off Terraria damage calculations.
        /// </summary>
        public int GetPvPDamage(PvPPlayer owner) => TerrariaUtils.GetWeaponDamage(owner, this);

        /// <summary>
        /// Gets the projectile shot by an item.
        /// </summary>
        public PvPProjectile Shoot => new PvPProjectile(Database.GetData<int>(DbConsts.ItemTable, type, DbConsts.Shoot));

        /// <summary>
        /// Gets the knockback of an item.
        /// </summary>
        public float GetKnockback(PvPPlayer owner) => owner.TPlayer.GetWeaponKnockback(this, Database.GetData<float>(DbConsts.ItemTable, type, DbConsts.Knockback));

        /// <summary>
        /// Gets whether the weapon's projectile has been changed.
        /// </summary>
        public bool IsShootModded => Database.GetData<int>(DbConsts.ItemTable, type, DbConsts.IsShootModded) == 1;

        /// <summary>
        /// Gets the weapon's shoot speed.
        /// </summary>
        public float ShootSpeed => Database.GetData<float>(DbConsts.ItemTable, type, DbConsts.ShootSpeed);

        /// <summary>
        /// Gets the velocity multiplier of the item
        /// </summary>
        public float VelocityMultiplier =>
            Database.GetData<float>(DbConsts.ItemTable, type, DbConsts.VelocityMultiplier);

        /// <summary>
        /// Gets the wrath (%dmg increase) of an item.
        /// </summary>
        public float Wrath => Database.GetData<float>(DbConsts.ItemTable, type, DbConsts.Wrath);

        /// <summary>
        /// Gets the titan (%dmg increase) of an item.
        /// </summary>
        public float GetTitan => Database.GetData<float>(DbConsts.ItemTable, type, DbConsts.Titan);

        /// <summary>
        /// Returns information about an item's debuff.
        /// </summary>
        public BuffInfo Debuff =>
            new BuffInfo(Database.GetData<int>(DbConsts.ItemTable, type, DbConsts.InflictBuffId),
                Database.GetData<int>(DbConsts.ItemTable, type, DbConsts.InflictBuffDuration));

        /// <summary>
        /// Returns information about an item's self buff.
        /// </summary>
        public BuffInfo SelfBuff =>
            new BuffInfo(Database.GetData<int>(DbConsts.ItemTable, type, DbConsts.ReceiveBuffId),
                Database.GetData<int>(DbConsts.ItemTable, type, DbConsts.ReceiveBuffDuration));
    }
}
