using PvPController.Utilities;
using Terraria;

namespace PvPController.PvPVariables {
    public class PvPItem : Item {
        public string name = "";
        public double damage = 0;

        public PvPItem() {
            this.SetDefaults();
            name = this.Name;
        }

        public PvPItem(Item item) {
            this.SetDefaults(item.type);
            this.prefix = item.prefix;
            this.damage = item.damage;
            name = this.Name;
        }

        public PvPItem(int type) {
            this.SetDefaults(type);
            this.damage = base.damage;
            name = this.Name;
        }

        public int GetConfigDamage() {
            return PvPController.database.itemInfo[type].damage;
        }

        public int GetPvPDamage(PvPPlayer owner) {
            return TerrariaUtils.GetWeaponDamage(owner, this);
        }
    }
}
