using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.Network {
    public class PlayerDeathArgs : EventArgs {
        public PvPPlayer dead;

        public PlayerDeathArgs(PvPPlayer dead) {
            this.dead = dead;
        }
    }
}
