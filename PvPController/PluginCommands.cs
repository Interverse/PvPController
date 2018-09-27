using PvPController.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace PvPController {
    public class PluginCommands {
        private static string damageParameters = "Parameters: enable (e), critical (c), damagevariance (dv), weapon (w), projectile (p)";
        private static string buffParameters = "Parameters: enable (e), projectilebuff (pb), projectileselfbuff (psb), weaponbuff (wb), weaponselfbuff(wsb), buffdebuff (bd), buffselfbuff (bsb)";
        private static string reflectParameters = "Parameters: enable (e), turtle, thorns";
        private static string armorParameters = "Parameters: defense (d), frost (f), nebula (n), vortex (v)";
        private static string projectileParameters = "Parameters: shoot (s), shootspeed (ss)";
        private static string miscParameters = "Parameters: enableplugin (ep), minion (m), deathitemtag (dit), iframetime (ift), deathmessages (dm), knockback (k)";

        public static void registerCommands() {
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", Reload, "reload", "readconfig") { HelpText = "Sets config settings to server" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", WriteConfig, "writeconfig") { HelpText = "Writes server settings to config" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", ResetConfig, "resetconfig") { HelpText = "Reset server config to default values" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", WriteDocumentation, "writedocumentation") { HelpText = "Writes documentation to a .txt file in /tshock" });

            Commands.ChatCommands.Add(new Command("pvpcontroller.damage", ModDamage, "moddamage", "md") { HelpText = "Modifies damage settings. " + damageParameters });
            Commands.ChatCommands.Add(new Command("pvpcontroller.buff", ModBuff, "modbuff", "mb") { HelpText = "Modifies buff settings. " +  buffParameters });
            Commands.ChatCommands.Add(new Command("pvpcontroller.reflect", ModReflect, "modreflect", "mr") { HelpText = "Modifies reflect damage settings. " + reflectParameters });
            Commands.ChatCommands.Add(new Command("pvpcontroller.armor", ModArmor, "modarmor", "ma") { HelpText = "Modifies armor settings. " + armorParameters });
            Commands.ChatCommands.Add(new Command("pvpcontroller.projectile", ModProjectile, "modprojectile", "modproj", "mp") { HelpText = "Modifies projectile settings. " + projectileParameters });
            Commands.ChatCommands.Add(new Command("pvpcontroller.misc", ModMisc, "modmisc", "mm") { HelpText = "Modifies miscellaneous settings. " + miscParameters });
            
            Commands.ChatCommands.Add(new Command(ToggleTooltip, "toggletooltip", "tt") { HelpText = "Toggles damage/defense tooltip popups." });
        }

        private static void ToggleTooltip(CommandArgs args) {
            PvPController.pvpers[args.Player.Index].seeTooltip = !PvPController.pvpers[args.Player.Index].seeTooltip;

            args.Player.SendSuccessMessage("Tooltips: " + PvPController.pvpers[args.Player.Index].seeTooltip);
            Interface.ClearInterface(PvPController.pvpers[args.Player.Index]);
        }

        private static void ModDamage(CommandArgs args) {
            if (args.Parameters.Count == 0) {
                args.Player.SendErrorMessage("Wrong Syntax. " + damageParameters);
                return;
            }

            switch (args.Parameters[0]) {
                case "enable":
                case "e":
                    PvPController.config.enableDamageChanges = !PvPController.config.enableDamageChanges;
                    args.Player.SendSuccessMessage("Damage mods: " + PvPController.config.enableDamageChanges);
                    break;

                case "critical":
                case "crit":
                case "c":
                    PvPController.config.enableCriticals = !PvPController.config.enableCriticals;
                    args.Player.SendSuccessMessage("Criticals: " + PvPController.config.enableCriticals);
                    break;

                case "damagevariance":
                case "dv":
                    int lowerVariance = 0, upperVariance = 0;

                    switch (args.Parameters.Count) {
                        case 2:
                            if (Int32.TryParse(args.Parameters[1], out upperVariance)) {
                                lowerVariance = -upperVariance;
                                PvPController.config.upperDamageVariance = upperVariance;
                                PvPController.config.lowerDamageVariance = lowerVariance;
                            } else {
                                args.Player.SendErrorMessage("Syntax: /moddamage damagevariance <variance>");
                            }

                            args.Player.SendSuccessMessage("Damage variance set to between " + lowerVariance + " and " + upperVariance + ".");
                            break;
                        case 3:
                            if (Int32.TryParse(args.Parameters[1], out lowerVariance) && Int32.TryParse(args.Parameters[2], out upperVariance)) {
                                PvPController.config.upperDamageVariance = upperVariance;
                                PvPController.config.lowerDamageVariance = lowerVariance;
                            } else {
                                args.Player.SendErrorMessage("Syntax: /moddamage damagevariance <lowervariance> <uppervariance>");
                            }

                            args.Player.SendSuccessMessage("Damage variance set to between " + lowerVariance + " and " + upperVariance + ".");
                            break;
                        default:
                            args.Player.SendErrorMessage("Syntax: /moddamage damagevariance <lowervariance> <uppervariance>");
                            break;
                    }
                    break;

                case "weapon":
                case "w":
                    int itemid = 0;
                    int damage;

                    switch (args.Parameters.Count) {
                        case 3:
                            if (!Int32.TryParse(args.Parameters[1], out itemid)) {
                                List<Item> foundItems = TShock.Utils.GetItemByName(args.Parameters[1]);
                                if (foundItems.Count != 1) {
                                    args.Player.SendErrorMessage("Cannot find item or found multiple possible items for " + args.Parameters[1]);
                                    return;
                                }
                                itemid = foundItems[0].netID;
                            }

                            if (!Int32.TryParse(args.Parameters[2], out damage)) {
                                args.Player.SendErrorMessage("Invalid damage value of " + args.Parameters[2]);
                                return;
                            }

                            if (damage < 0) {
                                Item item = new Item();
                                item.SetDefaults(itemid);
                                damage = item.damage;
                            }

                            Database.itemInfo[itemid].damage = damage;
                            Database.UpdateItems(Database.itemInfo[itemid]);

                            args.Player.SendSuccessMessage("Base damage of " + Lang.GetItemName(itemid).ToString() + " set to " + damage);
                            break;
                        default:
                            args.Player.SendErrorMessage("Syntax: /moddamage weapon <item id or \"name\"> <damage>");
                            break;
                    }
                    break;

                case "projectile":
                case "p":
                    int projectileid = 0;
                    int projectileDamage;

                    switch (args.Parameters.Count) {
                        case 3:
                            if (!Int32.TryParse(args.Parameters[1], out projectileid)) {
                                args.Player.SendErrorMessage("Incorrect projectile id of " + args.Parameters[1]);
                                return;
                            }

                            if (!Int32.TryParse(args.Parameters[2], out projectileDamage)) {
                                args.Player.SendErrorMessage("Invalid damage value of " + args.Parameters[2]);
                                return;
                            }

                            Database.projectileInfo[projectileid].damage = projectileDamage;
                            Database.UpdateProjectiles(Database.projectileInfo[projectileid]);

                            args.Player.SendSuccessMessage("Base projectile damage of " + Lang.GetProjectileName(projectileid).ToString() + " set to " + projectileDamage);
                            break;
                        default:
                            args.Player.SendErrorMessage("Syntax: /moddamage projectile <projectile id> <damage>");
                            break;
                    }
                    break;

                default:
                    args.Player.SendErrorMessage("Wrong Syntax. " + damageParameters);
                    break;
            }
        }

        private static void ModBuff(CommandArgs args) {
            if (args.Parameters.Count == 0) {
                args.Player.SendErrorMessage("Wrong Syntax. " + buffParameters);
                return;
            }

            List<Item> foundItems = null;
            int projectileID = 0;
            int weaponID = 0;
            int buffType = 0;
            int buffType2 = 0;
            int buffDuration = 0;

            switch (args.Parameters[0]) {
                case "enable":
                case "e":
                    if (args.Parameters.Count == 1) {
                        args.Player.SendErrorMessage("Wrong Syntax. /modbuff enable <projectilebuff (pb), projectileselfbuff (psb), weaponbuff (wb), weaponselfbuff (wsb)");
                        return;
                    }
                    switch (args.Parameters[1]) {
                        case "projectilebuff":
                        case "pb":
                            PvPController.config.enableProjectileDebuffs = !PvPController.config.enableProjectileDebuffs;
                            args.Player.SendSuccessMessage("Projectile Debuffs: " + PvPController.config.enableProjectileDebuffs);
                            break;

                        case "projectileselfbuff":
                        case "psb":
                            PvPController.config.enableProjectileSelfBuffs = !PvPController.config.enableProjectileSelfBuffs;
                            args.Player.SendSuccessMessage("Projectile Self Buffs: " + PvPController.config.enableProjectileSelfBuffs);
                            break;

                        case "weaponbuff":
                        case "wb":
                            PvPController.config.enableWeaponDebuffs = !PvPController.config.enableWeaponDebuffs;
                            args.Player.SendSuccessMessage("Weapon Debuffs: " + PvPController.config.enableWeaponDebuffs);
                            break;

                        case "weaponselfbuff":
                        case "wsb":
                            PvPController.config.enableWeaponSelfBuffs = !PvPController.config.enableWeaponSelfBuffs;
                            args.Player.SendSuccessMessage("Weapon Self Buffs: " + PvPController.config.enableWeaponSelfBuffs);
                            break;

                        case "buffdebuff":
                        case "bd":
                            PvPController.config.enableBuffDebuff = !PvPController.config.enableBuffDebuff;
                            args.Player.SendSuccessMessage("Buff Debuffs: " + PvPController.config.enableBuffDebuff);
                            break;

                        case "buffselfbuff":
                        case "bsb":
                            PvPController.config.enableBuffSelfBuff = !PvPController.config.enableBuffSelfBuff;
                            args.Player.SendSuccessMessage("Buff Self Buff: " + PvPController.config.enableBuffSelfBuff);
                            break;

                        default:
                            args.Player.SendErrorMessage("Wrong Syntax. /modbuff enable <projectilebuff (pb), projectileselfbuff (psb), weaponbuff (wb), weaponselfbuff (wsb)");
                            break;
                    }
                    break;

                case "projectilebuff":
                case "pb":

                    switch (args.Parameters.Count) {
                        case 4:
                            if (!Int32.TryParse(args.Parameters[1], out projectileID)) {
                                args.Player.SendErrorMessage("Invalid projectile id of " + args.Parameters[1]);
                                return;
                            }

                            if (!Int32.TryParse(args.Parameters[2], out buffType)) {
                                args.Player.SendErrorMessage("Invalid buff id of " + args.Parameters[2]);
                                return;
                            }

                            if (!Int32.TryParse(args.Parameters[3], out buffDuration)) {
                                args.Player.SendErrorMessage("Invalid duration value of " + args.Parameters[3]);
                                return;
                            }

                            Database.projectileInfo[projectileID].debuff = new BuffDuration(buffType, buffDuration * 60);
                            Database.UpdateProjectiles(Database.projectileInfo[projectileID]);

                            args.Player.SendSuccessMessage("Projectile " + Lang.GetProjectileName(projectileID).ToString() + " set to buff " + Lang.GetBuffName(buffType) + " with a " + buffDuration + "s duration");
                            break;
                        default:
                            args.Player.SendErrorMessage("Syntax: /modbuff projectilebuff <projectile ID> <buff type> <buff duration>");
                            break;
                    }
                    break;

                case "projectileselfbuff":
                case "psb":

                    switch (args.Parameters.Count) {
                        case 4:
                            if (!Int32.TryParse(args.Parameters[1], out projectileID)) {
                                args.Player.SendErrorMessage("Invalid projectile id of " + args.Parameters[1]);
                                return;
                            }

                            if (!Int32.TryParse(args.Parameters[2], out buffType)) {
                                args.Player.SendErrorMessage("Invalid buff id of " + args.Parameters[2]);
                                return;
                            }

                            if (!Int32.TryParse(args.Parameters[3], out buffDuration)) {
                                args.Player.SendErrorMessage("Invalid duration value of " + args.Parameters[3]);
                                return;
                            }

                            Database.projectileInfo[projectileID].selfBuff = new BuffDuration(buffType, buffDuration * 60);
                            Database.UpdateProjectiles(Database.projectileInfo[projectileID]);

                            args.Player.SendSuccessMessage("Projectile " + Lang.GetProjectileName(projectileID).ToString() + " set to buff self " + Lang.GetBuffName(buffType) + " with a " + buffDuration + "s duration");
                            break;
                        default:
                            args.Player.SendErrorMessage("Syntax: /modbuff projectileselfbuff <projectile ID> <buff type> <buff duration>");
                            break;
                    }
                    break;

                case "weaponbuff":
                case "wb":

                    switch (args.Parameters.Count) {
                        case 4:
                            if (!Int32.TryParse(args.Parameters[1], out weaponID)) {
                                foundItems = TShock.Utils.GetItemByName(args.Parameters[1]);
                                if (foundItems.Count != 1) {
                                    args.Player.SendErrorMessage("Cannot find item or found multiple possible items for " + args.Parameters[1]);
                                    return;
                                }
                                weaponID = foundItems[0].netID;
                            }

                            if (!Int32.TryParse(args.Parameters[2], out buffType)) {
                                args.Player.SendErrorMessage("Invalid buff id of " + args.Parameters[2]);
                                return;
                            }

                            if (!Int32.TryParse(args.Parameters[3], out buffDuration)) {
                                args.Player.SendErrorMessage("Invalid duration value of " + args.Parameters[3]);
                                return;
                            }

                            Database.itemInfo[weaponID].debuff = new BuffDuration(buffType, buffDuration * 60);
                            Database.UpdateItems(Database.itemInfo[weaponID]);

                            args.Player.SendSuccessMessage("Item " + Lang.GetItemName(weaponID).ToString() + " set to buff " + Lang.GetBuffName(buffType) + " with a " + buffDuration + "s duration");
                            break;

                        default:
                            args.Player.SendErrorMessage("Syntax: /modbuff weaponbuff <weapon ID or \"name\"> <buff type> <buff duration>");
                            break;
                    }
                    break;

                case "weaponselfbuff":
                case "wsb":

                    switch (args.Parameters.Count) {
                        case 4:
                            if (!Int32.TryParse(args.Parameters[1], out weaponID)) {
                                foundItems = TShock.Utils.GetItemByName(args.Parameters[1]);
                                if (foundItems.Count != 1) {
                                    args.Player.SendErrorMessage("Cannot find item or found multiple possible items for " + args.Parameters[1]);
                                    return;
                                }
                                weaponID = foundItems[0].netID;
                            }

                            if (!Int32.TryParse(args.Parameters[2], out buffType)) {
                                args.Player.SendErrorMessage("Invalid buff id of " + args.Parameters[2]);
                                return;
                            }

                            if (!Int32.TryParse(args.Parameters[3], out buffDuration)) {
                                args.Player.SendErrorMessage("Invalid duration value of " + args.Parameters[3]);
                                return;
                            }

                            Database.itemInfo[weaponID].selfBuff = new BuffDuration(buffType, buffDuration * 60);
                            Database.UpdateItems(Database.itemInfo[weaponID]);

                            args.Player.SendSuccessMessage("Item " + Lang.GetItemName(weaponID).ToString() + " set to buff self " + Lang.GetBuffName(buffType) + " with a " + buffDuration + "s duration");
                            break;
                        default:
                            args.Player.SendErrorMessage("Syntax: /modbuff weaponselfbuff <weapon ID or \"name\"> <buff type> <buff duration>");
                            break;
                    }
                    break;

                case "buffdebuff":
                case "bd":

                    switch (args.Parameters.Count) {
                        case 4:
                            if (!Int32.TryParse(args.Parameters[1], out buffType)) {
                                args.Player.SendErrorMessage("Invalid buff id of " + args.Parameters[2]);
                                return;
                            }

                            if (!Int32.TryParse(args.Parameters[2], out buffType2)) {
                                args.Player.SendErrorMessage("Invalid 2nd buff id of " + args.Parameters[2]);
                                return;
                            }

                            if (!Int32.TryParse(args.Parameters[3], out buffDuration)) {
                                args.Player.SendErrorMessage("Invalid duration value of " + args.Parameters[3]);
                                return;
                            }

                            Database.buffInfo[buffType].debuff = new BuffDuration(buffType2, buffDuration * 60);
                            Database.UpdateBuffs(Database.buffInfo[buffType]);

                            args.Player.SendSuccessMessage("Buff " + Lang.GetBuffName(buffType) + " set to debuff others with " + Lang.GetBuffName(buffType2) + " with a " + buffDuration + "s duration");
                            break;
                        default:
                            args.Player.SendErrorMessage("Syntax: /modbuff buffdebuff <buff type> <inflict buff type> <buff duration>");
                            break;
                    }
                    break;

                case "buffdselfbuff":
                case "bsb":

                    switch (args.Parameters.Count) {
                        case 4:
                            if (!Int32.TryParse(args.Parameters[1], out buffType)) {
                                args.Player.SendErrorMessage("Invalid buff id of " + args.Parameters[2]);
                                return;
                            }

                            if (!Int32.TryParse(args.Parameters[2], out buffType2)) {
                                args.Player.SendErrorMessage("Invalid 2nd buff id of " + args.Parameters[2]);
                                return;
                            }

                            if (!Int32.TryParse(args.Parameters[3], out buffDuration)) {
                                args.Player.SendErrorMessage("Invalid duration value of " + args.Parameters[3]);
                                return;
                            }

                            Database.buffInfo[buffType].selfBuff = new BuffDuration(buffType2, buffDuration * 60);
                            Database.UpdateBuffs(Database.buffInfo[buffType]);

                            args.Player.SendSuccessMessage("Buff " + Lang.GetBuffName(buffType) + " set to buff self with " + Lang.GetBuffName(buffType2) + " with a " + buffDuration + "s duration");
                            break;
                        default:
                            args.Player.SendErrorMessage("Syntax: /modbuff buffselfbuff <buff type> <inflict buff type> <buff duration>");
                            break;
                    }
                    break;

                default:
                    args.Player.SendErrorMessage("Wrong Syntax. " + buffParameters);
                    break;
            }
        }

        private static void ModReflect(CommandArgs args) {
            if (args.Parameters.Count <= 1) {
                args.Player.SendErrorMessage("Wrong Syntax. " + reflectParameters);
                return;
            }

            double reflectMultiplier = 1;

            switch (args.Parameters[0]) {
                case "enable":
                case "e":
                    if (args.Parameters.Count == 1) {
                        args.Player.SendErrorMessage("Wrong Syntax. /modreflect enable <turtle, thorns>");
                        return;
                    }

                    switch (args.Parameters[1]) {
                        case "turtle":
                            PvPController.config.enableTurtle = !PvPController.config.enableTurtle;
                            args.Player.SendSuccessMessage("Turtle damage: " + PvPController.config.enableTurtle);
                            break;

                        case "thorns":
                            PvPController.config.enableThorns = !PvPController.config.enableThorns;
                            args.Player.SendSuccessMessage("Thorn damage: " + PvPController.config.enableThorns);
                            break;

                        default:
                            args.Player.SendErrorMessage("Wrong Syntax. /modreflect enable <turtle, thorns>");
                            break;
                    }
                    
                    break;

                case "turtle":
                    if (!Double.TryParse(args.Parameters[1], out reflectMultiplier)) {
                        args.Player.SendErrorMessage("Invalid damage multiplier of " + args.Parameters[1]);
                        return;
                    }

                    PvPController.config.turtleMultiplier = reflectMultiplier;
                    args.Player.SendSuccessMessage("Set turtle reflect damage to " + reflectMultiplier * 100 + "%.");
                    break;

                case "thorns":
                    if (!Double.TryParse(args.Parameters[1], out reflectMultiplier)) {
                        args.Player.SendErrorMessage("Invalid damage multiplier of " + args.Parameters[1]);
                        return;
                    }

                    PvPController.config.thornMultiplier = reflectMultiplier;
                    args.Player.SendSuccessMessage("Set thorn reflect damage to " + reflectMultiplier * 100 + "%.");
                    break;

                default:
                    args.Player.SendErrorMessage("Wrong Syntax. " + reflectParameters);
                    break;

            }
        }

        private static void ModMisc(CommandArgs args) {
            if (args.Parameters.Count == 0) {
                args.Player.SendErrorMessage("Wrong Syntax. " + miscParameters);
                return;
            }

            int pageNumber;
            int selection;

            switch (args.Parameters[0]) {
                case "enableplugin":
                case "ep":
                    PvPController.config.enablePlugin = !PvPController.config.enablePlugin;
                    args.Player.SendSuccessMessage("Plugin enabled: " + PvPController.config.enablePlugin);
                    break;

                case "minion":
                case "m":
                    PvPController.config.enableMinions = !PvPController.config.enableMinions;
                    args.Player.SendSuccessMessage("Minions: " + PvPController.config.enableMinions);
                    break;

                case "deathitemtag":
                case "dit":
                    if (args.Parameters.Count == 1) {
                        args.Player.SendErrorMessage("Wrong Syntax. /modmisc deathitemtag <custom message> or /modmisc deathitemtag weapon");
                        return;
                    }

                    PvPController.config.deathItemTag = args.Parameters[1];
                    args.Player.SendSuccessMessage("Death tag set to: " + args.Parameters[1]);
                    break;

                case "iframetime":
                case "ift":
                    if (args.Parameters.Count == 1) {
                        args.Player.SendErrorMessage("Wrong Syntax. /modmisc iframetime <seconds>");
                        return;
                    }

                    double iframeTime;

                    if (!Double.TryParse(args.Parameters[1], out iframeTime)) {
                        args.Player.SendErrorMessage("Invalid iframetime of " + args.Parameters[1]);
                        return;
                    }

                    PvPController.config.iframeTime = iframeTime * 1000.0;
                    args.Player.SendSuccessMessage("Invincibility frame time set to " + iframeTime + "s");
                    break;

                case "deathmessages":
                case "dm":

                    if (args.Parameters.Count == 1) {
                        args.Player.SendErrorMessage("Wrong syntax. /modmisc deathmessages <add/del/list>");
                        return;
                    }

                    switch (args.Parameters[1]) {
                        case "add":
                            if (args.Parameters.Count == 2) {
                                args.Player.SendErrorMessage("Invalid Syntax. /modmisc deathmessages add <normal/reflection> <deathmessage>");
                                return;
                            }

                            if (args.Parameters[2] == "normal") {
                                PvPController.config.normalDeathMessages.Add(args.Parameters[3]);
                                args.Player.SendSuccessMessage("Normal death message added: \"" + args.Parameters[3] + "\"");
                            } else if (args.Parameters[2] == "reflection") {
                                PvPController.config.reflectedDeathMessages.Add(args.Parameters[3]);
                                args.Player.SendSuccessMessage("Reflection death message added: \"" + args.Parameters[3] + "\"");
                            } else {
                                args.Player.SendErrorMessage("Invalid Syntax. /modmisc deathmessages add <normal/reflection> <deathmessage>");
                            }
                            break;

                        case "del":
                            if (args.Parameters.Count == 2) {
                                args.Player.SendErrorMessage("Invalid Syntax. /modmisc deathmessages del <normal/reflection> <death message #>");
                                return;
                            }

                            if (args.Parameters[2] == "normal") {
                                if (!Int32.TryParse(args.Parameters[3], out selection) ||  selection > PvPController.config.normalDeathMessages.Count) {
                                    args.Player.SendErrorMessage("Invalid selection of " + args.Parameters[3]);
                                    return;
                                }

                                selection--;

                                args.Player.SendSuccessMessage("Deleted normal death message: " + PvPController.config.normalDeathMessages[selection]);

                                PvPController.config.normalDeathMessages.RemoveAt(selection);
                            } else if (args.Parameters[2] == "reflection") {
                                if (!Int32.TryParse(args.Parameters[3], out selection) || selection > PvPController.config.normalDeathMessages.Count) {
                                    args.Player.SendErrorMessage("Invalid selection of " + args.Parameters[3]);
                                    return;
                                }

                                selection--;

                                args.Player.SendSuccessMessage("Deleted reflection death message: " + PvPController.config.reflectedDeathMessages[selection]);

                                PvPController.config.reflectedDeathMessages.RemoveAt(selection);
                            } else {
                                args.Player.SendErrorMessage("Invalid Syntax. /modmisc deathmessages del <normal/reflection> <death message #>");
                            }
                            break;

                        case "list":
                            if (args.Parameters.Count == 2) {
                                args.Player.SendErrorMessage("Invalid Syntax. /modmisc deathmessages list <normal/reflection>");
                                return;
                            }

                            if (args.Parameters[2] == "normal") {
                                if (!PaginationTools.TryParsePageNumber(args.Parameters, 3, args.Player, out pageNumber))
                                    return;

                                PaginationTools.SendPage(args.Player, pageNumber, PvPController.config.normalDeathMessages, new PaginationTools.Settings() {
                                    HeaderFormat = "Normal Death Messages list ({0}/{1}):",
                                    FooterFormat = "Type {0}modmisc deathmessages normal {{0}} for more death messages.".SFormat((args.Silent ? TShock.Config.CommandSilentSpecifier : TShock.Config.CommandSpecifier))
                                });
                            } else if (args.Parameters[2] == "reflection") {
                                if (!PaginationTools.TryParsePageNumber(args.Parameters, 3, args.Player, out pageNumber))
                                    return;

                                PaginationTools.SendPage(args.Player, pageNumber, PvPController.config.reflectedDeathMessages, new PaginationTools.Settings() {
                                    HeaderFormat = "Reflected Death Messages list ({0}/{1}):",
                                    FooterFormat = "Type {0}modmisc deathmessages reflection {{0}} for more death messages.".SFormat((args.Silent ? TShock.Config.CommandSilentSpecifier : TShock.Config.CommandSpecifier))
                                });
                            } else {
                                args.Player.SendErrorMessage("Invalid Syntax. /modmisc deathmessages list <normal/reflection>");
                            }
                            break;

                        default:
                            args.Player.SendErrorMessage("Wrong syntax. /modmisc deathmessages <add/del/list>");
                            break;
                    }

                    break;

                case "knockback":
                case "k":
                    PvPController.config.enableKnockback = !PvPController.config.enableKnockback;
                    args.Player.SendSuccessMessage("Custom knockback: " + PvPController.config.enableKnockback);
                    break;

                default:
                    args.Player.SendErrorMessage("Wrong Syntax. " + miscParameters);
                    break;
            }
        }

        private static void ModArmor(CommandArgs args) {
            if (args.Parameters.Count == 0) {
                args.Player.SendErrorMessage("Wrong Syntax. " + armorParameters);
                return;
            }

            switch (args.Parameters[0]) {
                case "frost":
                case "f":
                    double frostDuration;

                    if (args.Parameters.Count < 2) {
                        args.Player.SendErrorMessage("Wrong Syntax. /modarmor frost <enable> or /modarmor frost <buffduration>");
                        return;
                    }

                    switch (args.Parameters[1]) {
                        case "enable":
                        case "e":
                            PvPController.config.enableFrost = !PvPController.config.enableFrost;
                            args.Player.SendSuccessMessage("Frost armor debuff: " + PvPController.config.enableFrost);
                            break;

                        default:
                            if (!Double.TryParse(args.Parameters[1], out frostDuration)) {
                                args.Player.SendErrorMessage("Invalid frost duration of " + args.Parameters[1]);
                                return;
                            }

                            PvPController.config.frostDuration = frostDuration;
                            args.Player.SendSuccessMessage("Set frost debuff duration to " + frostDuration + "s.");
                            break;
                    }
                    break;

                case "nebula":
                case "n":
                    double nebulaTier1Duration;
                    double nebulaTier2Duration;
                    double nebulaTier3Duration;

                    if (args.Parameters.Count < 2) {
                        args.Player.SendErrorMessage("Wrong Syntax. /modarmor nebula <enable> or /modarmor nebula <tier #> <duration>");
                        return;
                    }

                    switch (args.Parameters[1]) {
                        case "enable":
                        case "e":
                            PvPController.config.enableNebula = !PvPController.config.enableNebula;
                            args.Player.SendSuccessMessage("Nebula armor buffs: " + PvPController.config.enableNebula);
                            break;

                        case "1":
                            if (!Double.TryParse(args.Parameters[2], out nebulaTier1Duration)) {
                                args.Player.SendErrorMessage("Invalid tier 1 duration of " + args.Parameters[1]);
                                return;
                            }

                            PvPController.config.nebulaTier1Duration = nebulaTier1Duration;
                            args.Player.SendSuccessMessage("Set tier 1 buff duration to " + nebulaTier1Duration + "s.");
                            break;

                        case "2":
                            if (!Double.TryParse(args.Parameters[2], out nebulaTier2Duration)) {
                                args.Player.SendErrorMessage("Invalid tier 2 duration of " + args.Parameters[1]);
                                return;
                            }

                            PvPController.config.nebulaTier2Duration = nebulaTier2Duration;
                            args.Player.SendSuccessMessage("Set nebula tier 2 buff duration to " + nebulaTier2Duration + "s.");
                            break;

                        case "3":
                            if (!Double.TryParse(args.Parameters[2], out nebulaTier3Duration)) {
                                args.Player.SendErrorMessage("Invalid tier 3 duration of " + args.Parameters[1]);
                                return;
                            }

                            PvPController.config.nebulaTier3Duration = nebulaTier3Duration;
                            args.Player.SendSuccessMessage("Set nebula tier 3 buff duration to " + nebulaTier3Duration + "s.");
                            break;

                        default:
                            args.Player.SendErrorMessage("Wrong Syntax. /modarmor nebula <enable> or /modarmor nebula <tier #> <duration>");
                            break;
                    }
                    break;

                case "vortex":
                case "v":
                    if (args.Parameters.Count < 2) {
                        args.Player.SendErrorMessage("Wrong Syntax. /modarmor vortex <percentage>");
                        return;
                    }

                    if (!Double.TryParse(args.Parameters[1], out double vortexMultiplier)) {
                        args.Player.SendErrorMessage("Invalid vortex multiplier of " + args.Parameters[1]);
                        return;
                    }

                    PvPController.config.vortexMultiplier = vortexMultiplier;

                    args.Player.SendSuccessMessage("Set vortex stealth multiplier to " + vortexMultiplier * 100 + "%.");
                    break;

                case "defense":
                case "d":
                    if (args.Parameters.Count < 3) {
                        args.Player.SendErrorMessage("Wrong Syntax. /modarmor defense <item id> <value>");
                        return;
                    }

                    if (!Int32.TryParse(args.Parameters[1], out int itemid)) {
                        args.Player.SendErrorMessage("Invalid item id of " + args.Parameters[1]);
                        return;
                    }

                    if (!Int32.TryParse(args.Parameters[2], out int defense)) {
                        args.Player.SendErrorMessage("Invalid defense value of " + args.Parameters[2]);
                        return;
                    }

                    Database.itemInfo[itemid].defense = defense;
                    Database.UpdateItems(Database.itemInfo[itemid]);
                    args.Player.SendSuccessMessage("Set item {0} to {1} defense.".SFormat(Lang.GetItemNameValue(itemid), defense));
                    break;

                default:
                    args.Player.SendErrorMessage("Wrong Syntax. " + armorParameters);
                    break;
            }
        }
        
        private static void ModProjectile(CommandArgs args) {
            if (args.Parameters.Count == 0) {
                args.Player.SendErrorMessage("Wrong Syntax. " + projectileParameters);
                return;
            }

            switch (args.Parameters[0]) {
                case "shoot":
                case "s":
                    if (args.Parameters.Count < 3) {
                        args.Player.SendErrorMessage("Wrong Syntax. /modprojectile shoot <item id> <projectile id>");
                        return;
                    }

                    if (!Int32.TryParse(args.Parameters[1], out int itemid)) {
                        args.Player.SendErrorMessage("Invalid item id of " + args.Parameters[1]);
                        return;
                    }

                    if (!Int32.TryParse(args.Parameters[2], out int shoot)) {
                        args.Player.SendErrorMessage("Invalid projectile value of " + args.Parameters[2]);
                        return;
                    }

                    Database.itemInfo[itemid].shoot = shoot;
                    Database.itemInfo[itemid].isShootModded = true;
                    Database.UpdateItems(Database.itemInfo[itemid]);
                    args.Player.SendSuccessMessage("Set item {0} to shoot {1}.".SFormat(Lang.GetItemNameValue(itemid), Lang.GetProjectileName(shoot)));
                    break;

                case "shootspeed":
                case "ss":
                    if (args.Parameters.Count < 3) {
                        args.Player.SendErrorMessage("Wrong Syntax. /modprojectile shootspeed <item id> <speed>");
                        return;
                    }

                    if (!Int32.TryParse(args.Parameters[1], out int itemId)) {
                        args.Player.SendErrorMessage("Invalid item id of " + args.Parameters[1]);
                        return;
                    }

                    if (!float.TryParse(args.Parameters[2], out float shootSpeed)) {
                        args.Player.SendErrorMessage("Invalid defense value of " + args.Parameters[2]);
                        return;
                    }

                    Database.itemInfo[itemId].shootSpeed = shootSpeed;
                    Database.UpdateItems(Database.itemInfo[itemId]);
                    args.Player.SendSuccessMessage("Set item {0} to {1} speed.".SFormat(Lang.GetItemNameValue(itemId), shootSpeed));
                    break;

                default:
                    args.Player.SendErrorMessage("Wrong Syntax. " + projectileParameters);
                    break;
            }
        }

        private static void WriteConfig(CommandArgs args) {
            PvPController.config.Write(Config.configPath);
            args.Player.SendSuccessMessage("Written server pvp changes to config.");
        }

        private static void ResetConfig(CommandArgs args) {
            PvPController.config.ResetConfigValues();
            PvPController.config.Write(Config.configPath);
            args.Player.SendSuccessMessage("Reset config values to default.");
        }

        private static void Reload(CommandArgs args) {
            PvPController.config = Config.Read(Config.configPath);
            args.Player.SendSuccessMessage("PvP config reloaded to server.");
        }

        private static void WriteDocumentation(CommandArgs args) {
            PvPController.config.WriteDocumentation();
            args.Player.SendSuccessMessage("Wrote documentation in a .txt file in /tshock.");
        }
    }
}
