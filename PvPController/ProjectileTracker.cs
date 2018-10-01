using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Terraria;

namespace PvPController {
    public class ProjectileTracker {
        public PvPProjectile[] projectiles = new PvPProjectile[Main.maxProjectileTypes];

        public ProjectileTracker() {
            for(int x = 0; x < projectiles.Length; x++) {
                projectiles[x] = new PvPProjectile(0);
            }
        }

        public void InsertProjectile(int index, int projectileType, int ownerIndex, PvPItem item) {
            projectiles[projectileType] = new PvPProjectile(projectileType);
            projectiles[projectileType].identity = index;
            projectiles[projectileType].SetOwner(ownerIndex);
            projectiles[projectileType].SetOriginatedItem(item);
        }
    }
}
