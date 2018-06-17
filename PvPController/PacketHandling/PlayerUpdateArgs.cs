using PvPController.PvPVariables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;

namespace PvPController.PacketHandling {
    public class PlayerUpdateArgs : EventArgs {
        public PvPPlayer player { get; set; }

        public int playerAction { get; set; }
        public int pulley { get; set; }
        public int selectedSlot { get; set; }

        public PlayerUpdateArgs(MemoryStream data, PvPPlayer player) {
            this.player = player;

            data.ReadByte();
            playerAction = data.ReadByte();
            pulley = data.ReadByte();
            this.selectedSlot = data.ReadByte();
        }
    }
}
