using System;
using System.IO;
using System.IO.Streams;

namespace PvPController.Network.PacketArgs {
    public class ProjectileDestroyArgs : EventArgs {
        public int ProjectileIndex { get; set; }
        public int Owner { get; set; }

        public ProjectileDestroyArgs(MemoryStream data) {
            ProjectileIndex = data.ReadInt16();
            Owner = data.ReadByte();
        }
    }
}
