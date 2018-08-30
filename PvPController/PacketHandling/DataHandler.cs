using PvPController.PvPVariables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using TerrariaApi.Server;

namespace PvPController.PacketHandling {
    public class DataHandler {
        public static event EventHandler<PlayerHurtArgs> PlayerHurtted;
        public static event EventHandler<PlayerUpdateArgs> PlayerUpdated;
        public static event EventHandler<ProjectileDestroyArgs> ProjectileDestroyed;
        public static event EventHandler<PlayerDeathArgs> PlayerDied;
        public static event EventHandler<TogglePvPArgs> PvPToggled;
        public static event EventHandler<PlayerSlotArgs> PlayerSlotUpdated;

        public static void OnPlayerHurtted(GetDataEventArgs args, PvPPlayer attacker, PvPPlayer target, PvPItem weapon, PvPProjectile projectile,
            PlayerDeathReason playerHitReason, int inflictedDamage, int damageReceived, int knockback, int crit) {

            if (PlayerHurtted != null)
                PlayerHurtted(typeof(DataHandler), new PlayerHurtArgs(args, attacker, target, weapon, projectile, playerHitReason, inflictedDamage, damageReceived, knockback, crit));
        }

        public static void OnPlayerUpdated(MemoryStream data, PvPPlayer player) {
            if (PlayerUpdated != null)
                PlayerUpdated(typeof(DataHandler), new PlayerUpdateArgs(data, player));
        }

        public static void OnProjectileDestroyed(MemoryStream data) {
            if (ProjectileDestroyed != null)
                ProjectileDestroyed(typeof(DataHandler), new ProjectileDestroyArgs(data));
        }

        public static void OnPlayerDead(PvPPlayer dead) {
            if (PlayerDied != null)
                PlayerDied(typeof(DataHandler), new PlayerDeathArgs(dead));
        }

        public static void OnPvPToggled(PvPPlayer player) {
            if (PvPToggled != null)
                PvPToggled(typeof(DataHandler), new TogglePvPArgs(player));
        }

        public static void OnPlayerSlotUpdated(PvPPlayer player, int slotid) {
            if (PlayerSlotUpdated != null)
                PlayerSlotUpdated(typeof(DataHandler), new PlayerSlotArgs(player, slotid));
        }
    }
}
