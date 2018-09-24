using Microsoft.Xna.Framework;
using PvPController.Utilities;
using System;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;

namespace PvPController.Variables {
    public class PvPPlayer : TSPlayer {

        DateTime lastHit;
        public PvPPlayer lastHitBy = null;
        public PvPItem lastHitWeapon = null;
        public PvPProjectile lastHitProjectile = null;
        public int previousSelectedItem = 0;

        private int medusaHitCount = 0;

        public bool seeTooltip = true;

        public PvPPlayer(int index) : base(index) {
            lastHit = DateTime.Now;
            User = TShock.Players[index].User;
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

        public double GetAngleFrom(Vector2 target) {
            return Math.Atan2(target.Y - this.Y, target.X - this.X);
        }
        
        public bool IsLeftFrom(Vector2 target) {
            return target.X > this.X;
        }

        /// <summary>
        /// Gets the damage dealt to a person with server side calculations.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="weapon"></param>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public int GetDamageDealt(PvPPlayer attacker, PvPItem weapon, PvPProjectile projectile = null) {
            int damage = (projectile == null || Database.projectileInfo[projectile.type].damage < 1) ?
                weapon.GetPvPDamage(attacker) : projectile.GetConfigDamage();

            damage += PvPUtils.GetAmmoDamage(attacker, weapon);
            damage += PvPUtils.GenerateDamageVariance();
            damage += PvPUtils.GetVortexDamage(attacker, weapon, damage);

            damage -= (int)(this.GetDefenseDifferenceFromModded() * 0.5);

            return damage;
        }

        /// <summary>
        /// Gets the defense of a player. Includes both vanilla and modded defense values.
        /// </summary>
        /// <returns></returns>
        public int GetPlayerDefense() {
            int vanillaArmorDefense = 0;
            int moddedArmorDefense = 0;

            for (int x = 0; x < 9; x++) {
                vanillaArmorDefense += this.TPlayer.armor[x].defense;
                moddedArmorDefense += Database.itemInfo[this.TPlayer.armor[x].netID].defense;
            }
            
            return this.TPlayer.statDefense - vanillaArmorDefense + moddedArmorDefense;
        }

        /// <summary>
        /// Gets the critical percentage of an item.
        /// Note: The ranged crit value is the final crit value, whereas the other crits have to
        /// be added to the existing weapon crit value.
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
        public int GetCrit(PvPItem weapon) {
            int crit = weapon.crit;
            if (weapon.melee) crit += TPlayer.meleeCrit;
            else if (weapon.ranged) crit = TPlayer.rangedCrit;
            else if (weapon.magic) crit += TPlayer.magicCrit;
            else if (weapon.thrown) crit += TPlayer.thrownCrit;

            return crit;
        }

        /// <summary>
        /// Returns the difference from normal defense values from the modded defense values.
        /// </summary>
        /// <returns></returns>
        public int GetDefenseDifferenceFromModded() {
            int vanillaArmorDefense = 0;
            int moddedArmorDefense = 0;

            for (int x = 0; x < 9; x++) {
                vanillaArmorDefense += this.TPlayer.armor[x].defense;
                moddedArmorDefense += Database.itemInfo[this.TPlayer.armor[x].netID].defense;
            }

            return moddedArmorDefense - vanillaArmorDefense;
        }

        /// <summary>
        /// Damages players. Custom knockback and criticals will apply if enabled.
        /// </summary>
        public void DamagePlayer(PvPPlayer attacker, PvPItem weapon, int damage, int hitDirection, bool isCrit) {
            Color color = Color.DarkTurquoise;
            string star = "*";
            if (PvPController.config.enableCriticals) {
                damage *= (isCrit ? 2 : 1);
                star = isCrit ? "!!" : "*";
                color = isCrit ? Color.SlateBlue : Color.DarkTurquoise;
            }

            if (PvPController.config.enableKnockback) {
                this.KnockBack(weapon.GetKnockback(attacker), attacker.GetAngleFrom(this.TPlayer.position), IsLeftFrom(attacker.TPlayer.position) ? -hitDirection : hitDirection);
                hitDirection = 0;
            }

            NetMessage.SendPlayerHurt(this.Index, PlayerDeathReason.ByCustomReason(PvPUtils.GetPvPDeathMessage(attacker, this, weapon, 1)),
                damage, hitDirection, false, true, 5);
            Interface.PlayerTextPopup(attacker, this, star + TerrariaUtils.GetHurtDamage(this, damage) + star, color);
        }

        /// <summary>
        /// Sets a velocity to a player, emulating directional knockback.
        /// 
        /// This method requires SSC to be enabled. To allow knockback to work
        /// on non-SSC servers, the method will temporarily enable SSC to set player
        /// velocity.
        /// </summary>
        public void KnockBack(double knockback, double angle, double hitDirection = 1) {
            if (this.TPlayer.noKnockback) return;
            
            if (PvPController.isSSC) {
                this.TPlayer.velocity.X += (float)(knockback * Math.Cos(angle) * hitDirection);
                this.TPlayer.velocity.Y += (float)(knockback * Math.Sin(angle));

                NetMessage.SendData(13, -1, -1, null, this.Index, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            } else {
                Main.ServerSideCharacter = true;
                NetMessage.SendData(7, this.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                this.IgnoreSSCPackets = true;

                this.TPlayer.velocity.X += (float)(knockback * Math.Cos(angle) * hitDirection);
                this.TPlayer.velocity.Y += (float)(knockback * Math.Sin(angle));

                NetMessage.SendData(13, -1, -1, null, this.Index, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                
                Main.ServerSideCharacter = false;
                NetMessage.SendData(7, this.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                this.IgnoreSSCPackets = false;
            }
        }

        /// <summary>
        /// Applies effects that normally won't work in vanilla pvp.
        /// Effects include nebula/frost armor, yoyo-bag projectiles, and thorns/turtle damage.
        /// </summary>
        public void ApplyPvPEffects(PvPPlayer attacker, PvPItem weapon, PvPProjectile projectile, int damage) {
            this.ApplyReflectDamage(attacker, damage, weapon);
            this.ApplyArmorEffects(attacker, weapon);
            TerrariaUtils.ActivateYoyo(attacker, this, damage, weapon.knockBack);
        }

        /// <summary>
        /// Applies turtle and thorns damage to the attacker.
        /// </summary>
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

        /// <summary>
        /// Applies nebula and frost armor effects.
        /// </summary>
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

        /// <summary>
        /// Applies buffs to the attacker based off own buffs, if any.
        /// </summary>
        public void ApplyBuffDebuffs(PvPPlayer attacker, PvPItem weapon) {
            int buffType;
            for(int x = 0; x < Terraria.Player.maxBuffs; x++) {
                buffType = attacker.TPlayer.buffType[x];
                if (MiscData.flaskDebuffs.ContainsKey(buffType)) {
                    if (weapon.melee) {
                        this.SetBuff(Database.buffInfo[buffType].debuff);
                    }
                    continue;
                }
                this.SetBuff(Database.buffInfo[buffType].debuff);
            }
        }

        /// <summary>
        /// Gets the first available ammo for a weapon.
        /// </summary>
        /// <param name="weapon">The weapon to find ammo for.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Applies buffs to self based off a buff, if any.
        /// </summary>
        public void ApplyBuffSelfBuff() {
            int buffType;
            for (int x = 0; x < Terraria.Player.maxBuffs; x++) {
                buffType = this.TPlayer.buffType[x];
                this.SetBuff(Database.buffInfo[buffType].selfBuff);
            }
        }

        /// <summary>
        /// Determines whether a person can be hit based off the config iframes.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Determines whether a person can be hit with Medusa Head.
        /// A normal Medusa attack hits six times at once, so this method
        /// limits it down to one hit per attack.
        /// </summary>
        /// <returns></returns>
        public bool CheckMedusa() {
            medusaHitCount++;
            if (medusaHitCount != 1) {
                if (medusaHitCount == 6) medusaHitCount = 0;
                return false;
            }

            return true;
        }
    }
}
