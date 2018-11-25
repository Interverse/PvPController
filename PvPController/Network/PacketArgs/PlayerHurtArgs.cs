using PvPController.Variables;
using System;
using System.IO;
using System.IO.Streams;
using Terraria.DataStructures;
using TerrariaApi.Server;

namespace PvPController.Network.PacketArgs {
    public class PlayerHurtArgs : EventArgs {
        public GetDataEventArgs Args { get; set; }

        public PvPPlayer Attacker { get; set; }
        public PvPPlayer Target { get; set; }

        public PvPItem Weapon { get; set; }
        public PvPProjectile Projectile { get; set; }

        public PlayerDeathReason PlayerHitReason { get; set; }

        public int InflictedDamage { get; set; }
        public int DamageReceived { get; set; }
        public int HitDirection { get; set; }
        public int Crit { get; set; }

        public bool IsPvPDamage { get; set; }

        public PlayerHurtArgs(GetDataEventArgs args, MemoryStream data, PvPPlayer attacker) {
            Args = args;
            
            Attacker = attacker;
            int targetId = data.ReadByte();
            if (targetId > -1) {
                Target = PvPController.PvPers[targetId];
                if (Target == null || !Target.ConnectionAlive || !Target.Active) {
                    IsPvPDamage = false;
                    return;
                }
            } else {
                IsPvPDamage = false;
                return;
            }

            PlayerHitReason = PlayerDeathReason.FromReader(new BinaryReader(data));
            
            if (PlayerHitReason.SourcePlayerIndex == -1) {
                IsPvPDamage = false;
                Target.LastHitBy = null;
                return;
            }

            Projectile = PlayerHitReason.SourceProjectileIndex == -1 ?
                null : Attacker.ProjTracker.Projectiles[PlayerHitReason.SourceProjectileType];

            int int1 = data.ReadInt16(); //damage
            int int2 = data.ReadByte(); //knockback

            Target.LastHitBy = Attacker;
            Target.LastHitWeapon = Weapon;
            Target.LastHitProjectile = Projectile;

            Weapon = Projectile?.ItemOriginated ?? Attacker.HeldItem;
            InflictedDamage = PvPController.Config.EnableDamageChanges ? Target.GetDamageDealt(Attacker, Weapon, Projectile) : int1;
            DamageReceived = Target.DamageReceived(InflictedDamage);
            HitDirection = int2 - 1;
            Crit = Attacker.GetCrit(Weapon);
            IsPvPDamage = true;
        }
    }
}
