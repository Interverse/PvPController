using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace PvPController.Variables {
    public class ProjectileUtils {
        public static PvPItem GetProjectileWeapon(PvPPlayer owner, int type) {
            PvPItem weapon;
            if (PresetData.PresetProjDamage.ContainsKey(type)) {
                weapon = new PvPItem();
                weapon.damage = PresetData.PresetProjDamage[type];
                weapon.specialName = Lang.GetProjectileName(type).ToString();
            } else if (PresetData.ProjHooks.ContainsKey(type)) {
                weapon = new PvPItem(type);
            } else if (PresetData.FromWhatItem.ContainsKey(type)) {
                weapon = owner.FindPlayerItem(PresetData.FromWhatItem[type]);
            } else if (PresetData.MinionItem.ContainsKey(type)) {
                weapon = owner.FindPlayerItem(PresetData.MinionItem[type]);
            } else {
                weapon = owner.GetPlayerItem();
            }
            return weapon;
        }

        public static int FindFreeIndex() {
            for (int x = 0; x < Main.projectile.Length; x++) {
                if (!Main.projectile[x].active)
                    return x;
            }

            return -1;
        }

        public static Projectile GetMainProjectile(int identity, int type, int owner) {
            return Main.projectile.Where(c => c != null)
                .Where(c => c.active)
                .SingleOrDefault(c => c.identity == identity && c.type == type && c.owner == owner);
        }
    }
}
