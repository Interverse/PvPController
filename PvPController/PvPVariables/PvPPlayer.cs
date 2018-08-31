using Microsoft.Xna.Framework;
using PvPController.Utilities;
using System;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;

namespace PvPController.PvPVariables {
    public class PvPPlayer : TSPlayer {

        DateTime lastHit;
        public PvPPlayer lastHitBy = null;
        public PvPItem lastHitWeapon = null;
        public PvPProjectile lastHitProjectile = null;
        public int previousSelectedItem = 0;

        public bool seeTooltip = true;

        public PvPPlayer(int index) : base(index) {
            lastHit = DateTime.Now;
            User = TShock.Players[Index].User;
        }

        public bool TryGetUser() {
            this.User = TShock.Players[Index].User;
            return this.User != null;
        }

        public PvPItem GetPlayerItem() {
            return PvPUtils.ConvertToPvPItem(SelectedItem);
        }

        public PvPItem FindPlayerItem(int type) {
            return PvPUtils.ConvertToPvPItem(TPlayer.inventory[TPlayer.FindItem(type)]);
        }

        public int GetDamageReceived(int damage) {
            return (int)TerrariaUtils.GetHurtDamage(this, damage);
        }

        public int GetDamageDealt(PvPPlayer attacker, PvPItem weapon, PvPProjectile projectile = null) {
            int damage = (projectile == null || PvPController.database.projectileInfo[projectile.type].damage < 1) ?
                weapon.GetPvPDamage(attacker) : projectile.GetConfigDamage();

            damage += PvPUtils.GetAmmoDamage(attacker, weapon);
            damage += PvPUtils.GetDamageVariance();
            damage += PvPUtils.GetVortexDamage(attacker, weapon, damage);

            damage -= (int)(this.GetDefenseDifferenceFromModded() * 0.5);

            return damage;
        }

        public int GetPlayerDefense() {
            int vanillaArmorDefense = 0;
            int moddedArmorDefense = 0;

            for (int x = 0; x < 9; x++) {
                vanillaArmorDefense += this.TPlayer.armor[x].defense;
                moddedArmorDefense += PvPController.database.itemInfo[this.TPlayer.armor[x].netID].defense;
            }
            
            return this.TPlayer.statDefense - vanillaArmorDefense + moddedArmorDefense;
        }

        public int GetDefenseDifferenceFromModded() {
            int vanillaArmorDefense = 0;
            int moddedArmorDefense = 0;

            for (int x = 0; x < 9; x++) {
                vanillaArmorDefense += this.TPlayer.armor[x].defense;
                moddedArmorDefense += PvPController.database.itemInfo[this.TPlayer.armor[x].netID].defense;
            }

            return moddedArmorDefense - vanillaArmorDefense;
        }

        public void DamagePlayer(PvPPlayer attacker, PvPItem weapon, int damage, int hitDirection, bool isCrit) {
            string star = "*";
            if (PvPController.config.enableCriticals) {
                damage *= (isCrit ? 2 : 1);
                star = isCrit ? "!!" : "*";
            }

            NetMessage.SendPlayerHurt(this.Index, PlayerDeathReason.ByCustomReason(PvPUtils.GetPvPDeathMessage(attacker, this, weapon, 1)),
                damage, hitDirection, false, true, 5);
            PvPUtils.PlayerTextPopup(attacker, this, star + TerrariaUtils.GetHurtDamage(this, damage) + star, Color.DarkTurquoise);
        }

        public void ApplyPvPEffects(PvPPlayer attacker, PvPItem weapon, PvPProjectile projectile, int damage) {
            this.ApplyReflectDamage(attacker, damage, weapon);
            this.ApplyArmorEffects(attacker, weapon);
            TerrariaUtils.ActivateYoyo(attacker, this, damage, weapon.knockBack);
        }

        public void ApplyReflectDamage(PvPPlayer attacker, int damage, PvPItem weapon) {
            PvPItem reflectType = new PvPItem();

            if (PvPController.config.enableTurtle && this.TPlayer.setBonus == Language.GetTextValue("ArmorSetBonus.Turtle") && weapon.melee) {
                reflectType.name = "Turtle Armor";
                int turtleDamage = (int)(damage * PvPController.config.turtleMultiplier);

                NetMessage.SendPlayerHurt(attacker.Index, PlayerDeathReason.ByCustomReason(PvPUtils.GetPvPDeathMessage(this, attacker, reflectType, 2)),
                    turtleDamage, 0, false, true, 5);
            }

            if (PvPController.config.enableThorns && this.TPlayer.FindBuffIndex(14) != -1) {
                reflectType.name = "Thorns";
                int thornDamage = (int)(damage * PvPController.config.thornMultiplier);

                NetMessage.SendPlayerHurt(attacker.Index, PlayerDeathReason.ByCustomReason(PvPUtils.GetPvPDeathMessage(this, attacker, reflectType, 2)),
                    thornDamage, 0, false, true, 5);
            } 
        }

        public void ApplyArmorEffects(PvPPlayer attacker, PvPItem weapon) {
            if (weapon.magic
                && attacker.TPlayer.armor[0].netID == 2760 && attacker.TPlayer.armor[1].netID == 2761 && attacker.TPlayer.armor[2].netID == 2762
                && PvPController.config.enableNebula) {

                if (attacker.TPlayer.FindBuffIndex(181) != -1 && PvPController.config.nebulaTier3Duration != 0) {
                    attacker.SetBuff(175, (int)(PvPController.config.nebulaTier3Duration * 60));
                    attacker.SetBuff(178, (int)(PvPController.config.nebulaTier3Duration * 60));
                    attacker.SetBuff(181, (int)(PvPController.config.nebulaTier3Duration * 60));
                } else if (attacker.TPlayer.FindBuffIndex(180) != -1 && PvPController.config.nebulaTier3Duration != 0) {
                    attacker.SetBuff(175, 1);
                    attacker.SetBuff(178, 1);
                    attacker.SetBuff(181, 1);
                    Task.Delay(100).ContinueWith(t => {
                        attacker.SetBuff(175, (int)(PvPController.config.nebulaTier3Duration * 60));
                        attacker.SetBuff(178, (int)(PvPController.config.nebulaTier3Duration * 60));
                        attacker.SetBuff(181, (int)(PvPController.config.nebulaTier3Duration * 60));
                    });
                } else if (attacker.TPlayer.FindBuffIndex(179) != -1 && PvPController.config.nebulaTier2Duration != 0) {
                    attacker.SetBuff(174, 1);
                    attacker.SetBuff(177, 1);
                    attacker.SetBuff(180, 1);
                    Task.Delay(100).ContinueWith(t => {
                        attacker.SetBuff(174, (int)(PvPController.config.nebulaTier2Duration * 60));
                        attacker.SetBuff(177, (int)(PvPController.config.nebulaTier2Duration * 60));
                        attacker.SetBuff(180, (int)(PvPController.config.nebulaTier2Duration * 60));
                    });
                } else {
                    attacker.SetBuff(173, (int)(PvPController.config.nebulaTier1Duration * 60));
                    attacker.SetBuff(176, (int)(PvPController.config.nebulaTier1Duration * 60));
                    attacker.SetBuff(179, (int)(PvPController.config.nebulaTier1Duration * 60));
                }
            }

            if ((weapon.ranged || weapon.melee) 
                && attacker.TPlayer.armor[0].netID == 684 && attacker.TPlayer.armor[1].netID == 685 && attacker.TPlayer.armor[2].netID == 686
                && PvPController.config.enableFrost) {

                this.SetBuff(44, (int)(PvPController.config.frostDuration * 30));
            }
        }

        public void ApplyBuffDebuffs(PvPPlayer attacker, PvPItem weapon) {
            int buffType;
            for(int x = 0; x < Terraria.Player.maxBuffs; x++) {
                buffType = attacker.TPlayer.buffType[x];
                if (MiscData.flaskDebuffs.ContainsKey(buffType)) {
                    if (weapon.melee) {
                        this.SetBuff(PvPController.database.buffInfo[buffType].debuff);
                        continue;
                    }
                }
                this.SetBuff(PvPController.database.buffInfo[buffType].debuff);
            }
        }

        public PvPItem GetFirstAvailableAmmo(PvPItem weapon) {
            int useAmmo = weapon.useAmmo;

            if (useAmmo == AmmoID.None) return new PvPItem();

            for (int x = 54; x < NetItem.InventorySlots; x++) {
                if (this.TPlayer.inventory[x].ammo == useAmmo)
                    return PvPUtils.ConvertToPvPItem(this.TPlayer.inventory[x]);
            }

            for (int x = 0; x < NetItem.InventorySlots - 4; x++) {
                if (this.TPlayer.inventory[x].ammo == useAmmo)
                    return PvPUtils.ConvertToPvPItem(this.TPlayer.inventory[x]);
            }

            return new PvPItem();
        }

        public void ApplyBuffSelfBuff() {
            int buffType;
            for (int x = 0; x < Terraria.Player.maxBuffs; x++) {
                buffType = this.TPlayer.buffType[x];
                this.SetBuff(PvPController.database.buffInfo[buffType].selfBuff);
            }
        }

        public bool CanBeHit() {
            if ((DateTime.Now - lastHit).TotalMilliseconds >= PvPController.config.iframeTime) {
                lastHit = DateTime.Now;
                return true;
            }

            return false;
        }

        public void SetBuff(BuffDuration buffInfo) {
            SetBuff(buffInfo.buffid, buffInfo.buffDuration);
        }

        public override void SetBuff(int type, int time = 3600, bool bypass = false) {
            base.SetBuff(type, time, bypass);
        }
    }
}
