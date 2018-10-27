using Microsoft.Xna.Framework;
using PvPController.Variables;
using System;
using System.IO;
using System.IO.Streams;
using PvPController.Utilities;
using Terraria;
using TerrariaApi.Server;

namespace PvPController.Network.PacketArgs {
    public class ProjectileNewArgs : EventArgs {
        
        public GetDataEventArgs Args;
        public PvPPlayer Attacker;
        public PvPItem Weapon;

        public int Identity;
        public Vector2 Position;
        public Vector2 Velocity;
        public Single Knockback;
        public int Damage;
        public int Owner;
        public int Type;
        public BitsByte AiFlags;
        public float Ai0;
        public float Ai1;
        public float[] Ai;

        public ProjectileNewArgs(GetDataEventArgs args, MemoryStream data, PvPPlayer attacker) {
            Args = args;
            Attacker = attacker;

            Identity = data.ReadInt16();
            Position = new Vector2(data.ReadSingle(), data.ReadSingle());
            Velocity = new Vector2(data.ReadSingle(), data.ReadSingle());
            Knockback = data.ReadSingle();
            Damage = data.ReadInt16();
            Owner = data.ReadByte();
            Type = data.ReadInt16();
            AiFlags = (BitsByte)data.ReadByte();
            Ai0 = 0;
            Ai1 = 0;
            if (AiFlags[0]) {
                Ai0 = data.ReadSingle();
            }
            if (AiFlags[1]) {
                Ai1 = data.ReadSingle();
            }
            Ai = new float[Projectile.maxAI];
            
            Weapon = ProjectileUtils.GetProjectileWeapon(attacker, Type);
        }
    }
}
