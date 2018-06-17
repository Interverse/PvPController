using PvPController.PvPVariables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.PacketHandling {
    public class PlayerDeathArgs : EventArgs {
        public PvPPlayer dead;

        public PlayerDeathArgs(PvPPlayer dead) {
            this.dead = dead;
        }
    }
}
