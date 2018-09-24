using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace PvPController {
    public class ProjectileTracker {
        public static PvPProjectile[] projectiles = new PvPProjectile[Main.maxProjectiles];

        public static void InitializeTracker() {
            for(int x = 0; x < projectiles.Length; x++) {
                projectiles[x] = new PvPProjectile();
            }
        }

        public static int FindFreeIndex() {
            for (int x = 0; x < Main.projectile.Length; x++) {
                if (!Main.projectile[x].active)
                    return x;
            }

            return -1;
        }

        public static int GetProjectileIndex(int identity, int owner) {
            for (int x = 0; x < Main.projectile.Length; x++) {
                if (Main.projectile[x].identity == identity && Main.projectile[x].owner == owner)
                    return x;
            }

            return -1;
        }

        public static Projectile GetMainProjectile(int identity, int type, int owner) {
            return Main.projectile.Where(c => c != null)
                .Where(c => c.active)
                .SingleOrDefault(c => c.identity == identity && c.type == type && c.owner == owner);
        }

        public static void InsertProjectile(int index, int projectileType, int ownerIndex, PvPItem item) {
            projectiles[index] = new PvPProjectile(projectileType, index);
            projectiles[index].SetOwner(ownerIndex);
            projectiles[index].SetOriginatedItem(item);
            projectiles[index].PerformProjectileAction();
        }

        public static void RemoveProjectile(int index) {
            projectiles[index] = null;
        }
    }
}
