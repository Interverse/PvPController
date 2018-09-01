using PvPController.Utilities;
using Terraria;

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

        public int GetConfigDamage() {
            return PvPController.database.itemInfo[type].damage;
        }

        public int GetPvPDamage(PvPPlayer owner) {
            return TerrariaUtils.GetWeaponDamage(owner, this);
        }

        public PvPProjectile GetItemShoot() {
            return new PvPProjectile(PvPController.database.itemInfo[type].shoot);
        }

        public float GetKnockback(PvPPlayer owner) {
            return owner.TPlayer.GetWeaponKnockback(this, knockback);
        }

        public BuffDuration GetDebuffInfo() {
            return PvPController.database.itemInfo[type].debuff;
        }

        public BuffDuration GetSelfBuffInfo() {
            return PvPController.database.itemInfo[type].selfBuff;
        }
    }
}
