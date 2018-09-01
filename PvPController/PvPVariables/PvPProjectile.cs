using Microsoft.Xna.Framework;
using PvPController.Utilities;
using Terraria;

namespace PvPController.PvPVariables {
    public class PvPProjectile : Projectile {
        public PvPItem itemOriginated;
        public PvPPlayer ownerProjectile;

        public PvPProjectile(int type) {
            this.SetDefaults(type);
        }

        public void SetOwner(int owner) {
            this.owner = owner;
            ownerProjectile = PvPController.pvpers[owner];
        }

        public void SetOriginatedItem(PvPItem item) {
            itemOriginated = item;
        }

        public int GetConfigDamage() {
            return PvPController.database.projectileInfo[type].damage;
        }

        public void PerformProjectileAction() {
            switch (type) {
                case 536:
                    for (int x = 0; x < PvPController.pvpers.Length; x++) {
                        if (ownerProjectile.Index == PvPController.pvpers[x].Index) continue;
                        if (!PvPController.pvpers[x].TPlayer.hostile || PvPController.pvpers[x].Dead) continue;
                        if (Vector2.Distance(ownerProjectile.TPlayer.position, PvPController.pvpers[x].TPlayer.position) <= 300) {
                            if (PvPController.pvpers[x].CheckMedusa()) {
                                PvPController.pvpers[x].DamagePlayer(ownerProjectile, itemOriginated, PvPController.pvpers[x].GetDamageDealt(ownerProjectile, itemOriginated, this), 0, PvPUtils.IsCrit(ownerProjectile.GetCrit(itemOriginated)));
                                PvPController.pvpers[x].SetBuff(156, 40);
                            }
                        }
                    }
                    break;
            }
        }
    }
}
