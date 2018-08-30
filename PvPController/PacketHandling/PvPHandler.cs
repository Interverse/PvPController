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
            DataHandler.PlayerSlotUpdated += OnPlayerSlotUpdated;
        }

        public void Unsubscribe() {
            DataHandler.PlayerHurtted -= OnPlayerHurtted;
            DataHandler.PlayerUpdated -= OnPlayerUpdated;
            DataHandler.ProjectileDestroyed -= OnProjectileDestroyed;
            DataHandler.PlayerSlotUpdated -= OnPlayerSlotUpdated;
        }

        private void OnPlayerHurtted(object sender, PlayerHurtArgs e) {
            if (!PvPController.config.enablePlugin) return;

            if (!e.target.CanBeHit()) {
                e.args.Handled = true;
                return;
            }

            if (PvPController.config.enableDamageChanges) {
                e.args.Handled = true;
                e.target.DamagePlayer(e.attacker, e.weapon, e.inflictedDamage, e.knockback);
                e.target.ApplyPvPEffects(e.attacker, e.weapon, e.projectile, e.inflictedDamage);
            }

            if (PvPController.config.enableProjectileDebuffs)
                e.target.SetBuff(PvPController.config.projectileInfo[e.playerHitReason.SourceProjectileType].debuff);

            if (PvPController.config.enableProjectileSelfBuffs)
                e.attacker.SetBuff(PvPController.config.projectileInfo[e.playerHitReason.SourceProjectileType].selfBuff);

            if (PvPController.config.enableWeaponDebuffs)
                e.target.SetBuff(PvPController.config.itemInfo[e.attacker.GetPlayerItem().netID].debuff);

            if (PvPController.config.enableWeaponSelfBuffs)
                e.attacker.SetBuff(PvPController.config.itemInfo[e.attacker.GetPlayerItem().netID].selfBuff);

            if (PvPController.config.enableBuffDebuff)
                e.target.ApplyBuffDebuffs(e.attacker, e.weapon);

            if (PvPController.config.enableBuffSelfBuff)
                e.attacker.ApplyBuffSelfBuff();
        }

        private void OnPlayerUpdated(object sender, PlayerUpdateArgs e) {
            e.player.TPlayer.vortexStealthActive = (e.pulley & 8) == 8;

            if (!PvPController.config.enablePlugin) return;

            if (e.player.previousSelectedItem != e.selectedSlot) {
                if (e.player.seeTooltip) {
                    PvPItem item = PvPUtils.ConvertToPvPItem(e.player.TPlayer.inventory[e.selectedSlot]);

                    int damage = TerrariaUtils.GetWeaponDamage(e.player, item);
                    damage += PvPUtils.GetAmmoDamage(e.player, item);
                    damage += PvPUtils.GetVortexDamage(e.player, item, damage);

                    if (damage > 0) {
                        string message = item.Name + ": " + damage + " damage";
                        PvPUtils.PlayerTextPopup(e.player, message, Color.Goldenrod);
                    }
                }

                e.player.previousSelectedItem = e.selectedSlot;
            }
        }

        private void OnProjectileDestroyed(object sender, ProjectileDestroyArgs e) {
            PvPController.projectiles[e.projectileID] = null;
        }

        private void OnPlayerSlotUpdated(object sender, PlayerSlotArgs e) {
            if (e.player.seeTooltip) {
                if (e.slotid >= 59 && e.slotid <= 67) {
                    Task.Delay(50).ContinueWith(t => {
                        PvPUtils.PlayerTextPopup(e.player, e.player.GetPlayerDefense() + " defense", Color.OrangeRed);
                    });
                }
            }
        }
    }
}
