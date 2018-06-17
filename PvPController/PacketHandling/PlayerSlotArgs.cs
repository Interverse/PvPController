using PvPController.PvPVariables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.PacketHandling {
    public class PlayerSlotArgs : EventArgs {
        public PvPPlayer player;
        public int slotid;

        public PlayerSlotArgs(PvPPlayer player, int slotid) {
            this.player = player;
            this.slotid = slotid;
        }
    }
}
