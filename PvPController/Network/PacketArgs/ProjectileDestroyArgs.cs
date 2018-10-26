using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.Network.PacketArgs {
    public class ProjectileDestroyArgs : EventArgs {
        public int projectileIndex { get; set; }
        public int owner { get; set; }

        public ProjectileDestroyArgs(MemoryStream data) {
            projectileIndex = data.ReadInt16();
            owner = data.ReadByte();
        }
    }
}
