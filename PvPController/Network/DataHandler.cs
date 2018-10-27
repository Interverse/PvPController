using PvPController.Network.PacketArgs;
using PvPController.Variables;
using System;
using System.IO;
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
                    OnPlayerHurt(args, data, attacker);
                    break;

                case PacketTypes.TogglePvp:
                    OnPvPToggled(attacker);
                    break;

                case PacketTypes.PlayerSlot:
                    OnPlayerSlotUpdated(data, attacker);
                    break;

                case PacketTypes.PlayerDeathV2:
                    OnPlayerDead(attacker);
                    break;

                case PacketTypes.ProjectileNew:
                    OnProjectileNew(args, data, attacker);
                    break;

                case PacketTypes.ProjectileDestroy:
                    OnProjectileDestroyed(data);
                    break;

                case PacketTypes.PlayerUpdate:
                    OnPlayerUpdated(data, attacker);
                    break;
            }
        }

        public static void OnPlayerHurt(GetDataEventArgs args, MemoryStream data, PvPPlayer attacker) {
            PlayerHurt?.Invoke(typeof(DataHandler), new PlayerHurtArgs(args, data, attacker));
        }

        public static void OnPlayerUpdated(MemoryStream data, PvPPlayer player) {
            PlayerUpdated?.Invoke(typeof(DataHandler), new PlayerUpdateArgs(data, player));
        }

        public static void OnProjectileNew(GetDataEventArgs args, MemoryStream data, PvPPlayer attacker) {
            ProjectileNew?.Invoke(typeof(DataHandler), new ProjectileNewArgs(args, data, attacker));
        }

        public static void OnProjectileDestroyed(MemoryStream data) {
            ProjectileDestroyed?.Invoke(typeof(DataHandler), new ProjectileDestroyArgs(data));
        }

        public static void OnPlayerDead(PvPPlayer dead) {
            PlayerDied?.Invoke(typeof(DataHandler), new PlayerDeathArgs(dead));
        }

        public static void OnPvPToggled(PvPPlayer player) {
            PvPToggled?.Invoke(typeof(DataHandler), new TogglePvPArgs(player));
        }

        public static void OnPlayerSlotUpdated(MemoryStream data, PvPPlayer player) {
            PlayerSlotUpdated?.Invoke(typeof(DataHandler), new PlayerSlotArgs(data, player));
        }
    }
}
