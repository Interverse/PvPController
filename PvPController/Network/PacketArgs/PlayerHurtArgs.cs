using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using TerrariaApi.Server;

namespace PvPController.Network.PacketArgs {
    public class PlayerHurtArgs : EventArgs {
        public GetDataEventArgs args { get; set; }

        public PvPPlayer attacker { get; set; }
        public PvPPlayer target { get; set; }

        public PvPItem weapon { get; set; }

        public PvPProjectile projectile { get; set; }

        public PlayerDeathReason playerHitReason { get; set; }

        public int inflictedDamage { get; set; }
        public int damageReceived { get; set; }
        public int hitDirection { get; set; }
        public int crit { get; set; }

        public bool isPvPDamage { get; set; }

        public PlayerHurtArgs(GetDataEventArgs args, MemoryStream data, PvPPlayer attacker) {
            this.args = args;
            
            PvPPlayer target = PvPController.pvpers[data.ReadByte()];
            PlayerDeathReason playerHitReason = PlayerDeathReason.FromReader(new BinaryReader(data));
            if (target == null || !target.ConnectionAlive || !target.Active) {
                isPvPDamage = false;
                return;
            }

            if (playerHitReason.SourcePlayerIndex == -1) {
                isPvPDamage = false;
                target.lastHitBy = null;
                return;
            }

            this.projectile = playerHitReason.SourceProjectileIndex == -1 ?
                null : attacker.projTracker.projectiles[playerHitReason.SourceProjectileType];

            int int1 = data.ReadInt16(); //damage
            int int2 = data.ReadByte(); //knockback

            target.lastHitBy = attacker;
            target.lastHitWeapon = weapon;
            target.lastHitProjectile = projectile;

            this.attacker = attacker;
            this.target = target;

            this.weapon = projectile == null ? attacker.GetPlayerItem() : projectile.itemOriginated;
            this.inflictedDamage = PvPController.config.enableDamageChanges ? target.GetDamageDealt(attacker, weapon, projectile) : int1;
            this.damageReceived = target.GetDamageReceived(inflictedDamage);
            this.hitDirection = int2 - 1;
            this.crit = attacker.GetCrit(weapon);
            this.playerHitReason = playerHitReason;
            this.isPvPDamage = true;
        }
    }
}
