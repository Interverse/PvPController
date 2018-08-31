using Microsoft.Xna.Framework;
using PvPController.PvPVariables;
using PvPController.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.PacketHandling {
    public class PvPHandler {
        public PvPHandler() {
            DataHandler.PlayerHurtted += OnPlayerHurtted;
            DataHandler.PlayerUpdated += OnPlayerUpdated;
            DataHandler.ProjectileDestroyed += OnProjectileDestroyed;
            DataHandler.PvPToggled += OnPvPToggled;
        }

        public void Unsubscribe() {
            DataHandler.PlayerHurtted -= OnPlayerHurtted;
            DataHandler.PlayerUpdated -= OnPlayerUpdated;
            DataHandler.ProjectileDestroyed -= OnProjectileDestroyed;
            DataHandler.PvPToggled -= OnPvPToggled;
        }

        private void OnPlayerHurtted(object sender, PlayerHurtArgs e) {
            if (!PvPController.config.enablePlugin) return;

            if (!e.target.CanBeHit()) {
                e.args.Handled = true;
                return;
            }
            
            e.args.Handled = true;

            e.target.DamagePlayer(e.attacker, e.weapon, e.inflictedDamage, e.knockback, PvPUtils.IsCrit(e.crit));
            e.target.ApplyPvPEffects(e.attacker, e.weapon, e.projectile, e.inflictedDamage);

            if (PvPController.config.enableProjectileDebuffs)
                e.target.SetBuff(PvPController.database.projectileInfo[e.playerHitReason.SourceProjectileType].debuff);

            if (PvPController.config.enableProjectileSelfBuffs)
                e.attacker.SetBuff(PvPController.database.projectileInfo[e.playerHitReason.SourceProjectileType].selfBuff);

            if (PvPController.config.enableWeaponDebuffs)
                e.target.SetBuff(PvPController.database.itemInfo[e.attacker.GetPlayerItem().netID].debuff);

            if (PvPController.config.enableWeaponSelfBuffs)
                e.attacker.SetBuff(PvPController.database.itemInfo[e.attacker.GetPlayerItem().netID].selfBuff);

            if (PvPController.config.enableBuffDebuff)
                e.target.ApplyBuffDebuffs(e.attacker, e.weapon);

            if (PvPController.config.enableBuffSelfBuff)
                e.attacker.ApplyBuffSelfBuff();
        }

        private void OnPlayerUpdated(object sender, PlayerUpdateArgs e) {
            e.player.TPlayer.vortexStealthActive = (e.pulley & 8) == 8;

            if (!PvPController.config.enablePlugin) return;

            if (e.player.previousSelectedItem != e.selectedSlot) {
                e.player.previousSelectedItem = e.selectedSlot;
            }
        }

        private void OnProjectileDestroyed(object sender, ProjectileDestroyArgs e) {
            PvPController.projectiles[e.projectileID] = null;
        }

        private void OnPvPToggled(object sender, TogglePvPArgs e) {
            PvPUtils.ClearInterface(e.player);
        }
    }
}
