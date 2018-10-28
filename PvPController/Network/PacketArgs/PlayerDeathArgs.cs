using PvPController.Variables;
using System;

namespace PvPController.Network.PacketArgs {
    public class PlayerDeathArgs : EventArgs {
        public PvPPlayer Dead;

        public PlayerDeathArgs(PvPPlayer dead) {
            Dead = dead;
        }
    }
}
