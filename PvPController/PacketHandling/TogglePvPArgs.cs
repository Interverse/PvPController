using PvPController.PvPVariables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.PacketHandling {
    public class TogglePvPArgs : EventArgs {
        public PvPPlayer player;

        public TogglePvPArgs(PvPPlayer player) {
            this.player = player;
        }
    }
}
