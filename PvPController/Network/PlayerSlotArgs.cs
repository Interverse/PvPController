using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.Network {
    public class PlayerSlotArgs : EventArgs {
        public PvPPlayer player;
        public int slotid;

        public PlayerSlotArgs(MemoryStream data, PvPPlayer player) {
            data.ReadByte(); //Cycles through the MemoryStream data
            this.slotid = data.ReadByte();
            this.player = player;
        }
    }
}
