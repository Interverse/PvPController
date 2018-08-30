using PvPController.PvPVariables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using TerrariaApi.Server;

namespace PvPController.PacketHandling {
    public class PlayerHurtArgs : EventArgs {
        public GetDataEventArgs args { get; set; }

        public PvPPlayer attacker { get; set; }
        public PvPPlayer target { get; set; }

        public PvPItem weapon { get; set; }

        public PvPProjectile projectile { get; set; }

        public PlayerDeathReason playerHitReason { get; set; }

        public int inflictedDamage { get; set; }
        public int damageReceived { get; set; }
        public int knockback { get; set; }
        public int crit { get; set; }

        public PlayerHurtArgs(GetDataEventArgs args, PvPPlayer attacker, PvPPlayer target, PvPItem weapon, PvPProjectile projectile,
            PlayerDeathReason playerHitReason, int inflictedDamage, int damageReceived, int knockback, int crit) {

            this.args = args;
            this.attacker = attacker;
            this.target = target;
            this.weapon = weapon;
            this.projectile = projectile;
            this.playerHitReason = playerHitReason;
            this.inflictedDamage = inflictedDamage;
            this.damageReceived = damageReceived;
            this.knockback = knockback;
            this.crit = crit;
        }
    }
}
