using PvPController.Network.PacketArgs;
using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using TerrariaApi.Server;

namespace PvPController.Network {
    /// <summary>
    /// Creates hooks for plugins to use.
    /// </summary>
    public class DataHandler {
        public static event EventHandler<PlayerHurtArgs> PlayerHurt;
        public static event EventHandler<PlayerUpdateArgs> PlayerUpdated;
        public static event EventHandler<ProjectileNewArgs> ProjectileNew;
        public static event EventHandler<ProjectileDestroyArgs> ProjectileDestroyed;
        public static event EventHandler<PlayerDeathArgs> PlayerDied;
        public static event EventHandler<TogglePvPArgs> PvPToggled;
        public static event EventHandler<PlayerSlotArgs> PlayerSlotUpdated;

        public static void HandleData(GetDataEventArgs args, MemoryStream data, PvPPlayer attacker) {
            switch (args.MsgID) {
                case PacketTypes.PlayerHurtV2:
                    DataHandler.OnPlayerHurt(args, data, attacker);
                    break;

                case PacketTypes.TogglePvp:
                    DataHandler.OnPvPToggled(attacker);
                    break;

                case PacketTypes.PlayerSlot:
                    DataHandler.OnPlayerSlotUpdated(data, attacker);
                    break;

                case PacketTypes.PlayerDeathV2:
                    DataHandler.OnPlayerDead(attacker);
                    break;

                case PacketTypes.ProjectileNew:
                    DataHandler.OnProjectileNew(args, data, attacker);
                    break;

                case PacketTypes.ProjectileDestroy:
                    DataHandler.OnProjectileDestroyed(data);
                    break;

                case PacketTypes.PlayerUpdate:
                    DataHandler.OnPlayerUpdated(data, attacker);
                    break;
            }
        }

        public static void OnPlayerHurt(GetDataEventArgs args, MemoryStream data, PvPPlayer attacker) {
            if (PlayerHurt != null)
                PlayerHurt(typeof(DataHandler), new PlayerHurtArgs(args, data, attacker));
        }

        public static void OnPlayerUpdated(MemoryStream data, PvPPlayer player) {
            if (PlayerUpdated != null)
                PlayerUpdated(typeof(DataHandler), new PlayerUpdateArgs(data, player));
        }

        public static void OnProjectileNew(GetDataEventArgs args, MemoryStream data, PvPPlayer attacker) {
            if (ProjectileNew != null)
                ProjectileNew(typeof(DataHandler), new ProjectileNewArgs(args, data, attacker));
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

        public static void OnPlayerSlotUpdated(MemoryStream data, PvPPlayer player) {
            if (PlayerSlotUpdated != null)
                PlayerSlotUpdated(typeof(DataHandler), new PlayerSlotArgs(data, player));
        }
    }
}
