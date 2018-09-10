using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.Network {
    public class PlayerSlotArgs : EventArgs {
        public PvPPlayer player;
        public int slotid;

        public PlayerSlotArgs(PvPPlayer player, int slotid) {
            this.player = player;
            this.slotid = slotid;
        }
    }
}
