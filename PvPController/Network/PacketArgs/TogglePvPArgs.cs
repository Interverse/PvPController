using PvPController.Variables;
using System;

namespace PvPController.Network.PacketArgs {
    public class TogglePvPArgs : EventArgs {
        public PvPPlayer Player;

        public TogglePvPArgs(PvPPlayer player) {
            Player = player;
        }
    }
}
