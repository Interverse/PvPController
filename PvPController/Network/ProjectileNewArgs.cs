using Microsoft.Xna.Framework;
using PvPController.Utilities;
using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;

namespace PvPController.Network {
    public class ProjectileNewArgs : EventArgs {

        public GetDataEventArgs args;
        public PvPPlayer attacker;
        public PvPItem weapon;

        public int identity;
        public Vector2 position;
        public Vector2 velocity;
        public Single knockback;
        public int damage;
        public int owner;
        public int type;
        public BitsByte aiFlags;
        public float ai0;
        public float ai1;

        public ProjectileNewArgs(GetDataEventArgs args, MemoryStream data, PvPPlayer attacker) {
            this.args = args;
            this.attacker = attacker;

            identity = data.ReadInt16();
            position = new Vector2(data.ReadSingle(), data.ReadSingle());
            velocity = new Vector2(data.ReadSingle(), data.ReadSingle());
            knockback = data.ReadSingle();
            damage = data.ReadInt16();
            owner = data.ReadByte();
            type = data.ReadInt16();
            aiFlags = (BitsByte)data.ReadByte();
            ai0 = 0;
            ai1 = 0;
            if (aiFlags[0]) {
                ai0 = data.ReadSingle();
            }
            if (aiFlags[1]) {
                ai1 = data.ReadSingle();
            }
            float[] ai = new float[Projectile.maxAI];
            
            weapon = ProjectileUtils.GetProjectileWeapon(attacker, type);
        }
    }
}
