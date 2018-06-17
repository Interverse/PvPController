using Terraria;

namespace PvPController.PvPVariables {
    public class PvPProjectile : Projectile {
        public PvPItem itemOriginated;

        public PvPProjectile(int type) {
            this.SetDefaults(type);
        }

        public void SetOwner(int owner) {
            this.owner = owner;
        }

        public void SetOriginatedItem(PvPItem item) {
            itemOriginated = item;
        }

        public int GetConfigDamage() {
            return PvPController.config.projectileInfo[type].damage;
        }
    }
}
