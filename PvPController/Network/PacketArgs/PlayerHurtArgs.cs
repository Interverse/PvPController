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
            
            var target = PvPController.PvPers[data.ReadByte()];
            var playerHitReason = PlayerDeathReason.FromReader(new BinaryReader(data));
            if (target == null || !target.ConnectionAlive || !target.Active) {
                IsPvPDamage = false;
                return;
            }

            if (playerHitReason.SourcePlayerIndex == -1) {
                IsPvPDamage = false;
                target.LastHitBy = null;
                return;
            }

            Projectile = playerHitReason.SourceProjectileIndex == -1 ?
                null : attacker.ProjTracker.Projectiles[playerHitReason.SourceProjectileType];

            int int1 = data.ReadInt16(); //damage
            int int2 = data.ReadByte(); //knockback

            target.LastHitBy = attacker;
            target.LastHitWeapon = Weapon;
            target.LastHitProjectile = Projectile;

            Attacker = attacker;
            Target = target;

            Weapon = Projectile == null ? attacker.GetPlayerItem : Projectile.ItemOriginated;
            InflictedDamage = PvPController.Config.EnableDamageChanges ? target.GetDamageDealt(attacker, Weapon, Projectile) : int1;
            DamageReceived = target.GetDamageReceived(InflictedDamage);
            HitDirection = int2 - 1;
            Crit = attacker.GetCrit(Weapon);
            PlayerHitReason = playerHitReason;
            IsPvPDamage = true;
        }
    }
}
