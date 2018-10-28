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

        public static void HandleData(GetDataEventArgs args, MemoryStream data, PvPPlayer player) {
            switch (args.MsgID) {
                case PacketTypes.PlayerHurtV2:
                    PlayerHurt?.Invoke(typeof(DataHandler), new PlayerHurtArgs(args, data, player));
                    return;

                case PacketTypes.TogglePvp:
                    PvPToggled?.Invoke(typeof(DataHandler), new TogglePvPArgs(player));
                    return;

                case PacketTypes.PlayerSlot:
                    PlayerSlotUpdated?.Invoke(typeof(DataHandler), new PlayerSlotArgs(data, player));
                    return;

                case PacketTypes.PlayerDeathV2:
                    PlayerDied?.Invoke(typeof(DataHandler), new PlayerDeathArgs(player));
                    return;

                case PacketTypes.ProjectileNew:
                    ProjectileNew?.Invoke(typeof(DataHandler), new ProjectileNewArgs(args, data, player));
                    return;

                case PacketTypes.ProjectileDestroy:
                    ProjectileDestroyed?.Invoke(typeof(DataHandler), new ProjectileDestroyArgs(data));
                    return;

                case PacketTypes.PlayerUpdate:
                    PlayerUpdated?.Invoke(typeof(DataHandler), new PlayerUpdateArgs(data, player));
                    return;
            }
        }
    }
}
