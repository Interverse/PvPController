using PvPController.Variables;
using System;
using System.IO;

namespace PvPController.Network.PacketArgs {
    public class PlayerUpdateArgs : EventArgs {
        public PvPPlayer Player { get; set; }

        public int PlayerAction { get; set; }
        public int Pulley { get; set; }
        public int SelectedSlot { get; set; }

        public PlayerUpdateArgs(MemoryStream data, PvPPlayer player) {
            Player = player;

            data.ReadByte();
            PlayerAction = data.ReadByte();
            Pulley = data.ReadByte();
            SelectedSlot = data.ReadByte();
        }
    }
}
