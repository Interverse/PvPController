using PvPController.Variables;
using System;
using System.IO;

namespace PvPController.Network.PacketArgs {
    public class PlayerSlotArgs : EventArgs {
        public PvPPlayer Player;
        public int SlotId;

        public PlayerSlotArgs(MemoryStream data, PvPPlayer player) {
            data.ReadByte(); //Cycles through the MemoryStream data
            SlotId = data.ReadByte();
            Player = player;
        }
    }
}
