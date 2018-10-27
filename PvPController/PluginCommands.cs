using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace PvPController {
    public class PluginCommands {
        private static string modAllParameters = "Parameters: <Items/Projectiles/Buffs> <Name/Shoot/IsShootModded/ShootSpeed/Knockback/Defense/InflictBuffID/InflictBuffDuration/ReceiveBuffID/ReceiveBuffDuration> <value>";

        private static string tableList = "<{0}>".SFormat(string.Join("/", DBConsts.ItemTable, DBConsts.ProjectileTable, DBConsts.BuffTable));
        private static string itemIDParam = "<\"item name\"/id>";
        private static string statList = "<{0}>".SFormat(string.Join("/", DBConsts.Name, DBConsts.Shoot, DBConsts.IsShootModded, DBConsts.ShootSpeed, DBConsts.Knockback, DBConsts.Defense, DBConsts.InflictBuffID, DBConsts.InflictBuffDuration, DBConsts.ReceiveBuffID, DBConsts.ReceiveBuffDuration).SeparateToLines(60, "/"));
        private static string modStatParameters = "Parameters: {0} {1}\r\n{2} <value>".SFormat(tableList, itemIDParam, statList);

        private static string configList = "<{0}>".SFormat(string.Join("/", "enableplugin", "enabledamagechanges", "enablecriticals",
            "enableknockback", "enableminions", "enableprojectiledebuffs", "enableprojectileselfbuffs", "enableweapondebuffs",
            "enableweaponselfbuffs", "enableturtle", "turtlemultiplier", "enablethorns", "thornmultiplier", "enablenebula",
            "nebulatier3duration", "nebulatier2duration", "nebulatier1duration", "enablefrost", "frostduration", "vortexmultiplier",
            "enablebuffdebuff", "enablebuffselfbuff", "knockbackminimum", "iframetime", "lowerdamagevariance", "upperdamagevariance",
            "deathitemtag").SeparateToLines(60, "/"));
        private static string modConfigParameters = "Parameters: {0}".SFormat(configList);

        private static string resetList = "Parameters: <config/database/item/projectile/buff>";

        public static void registerCommands() {
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", ModConfig, "modconfig", "mc") { HelpText = "Sets config settings to server" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", Reload, "reload", "readconfig") { HelpText = "Sets config settings to server" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", WriteConfig, "writeconfig") { HelpText = "Writes server settings to config" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", ResetPvP, "resetpvp", "rpvp") { HelpText = "Reset values to default" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", WriteDocumentation, "writedocumentation") { HelpText = "Writes documentation to a .txt file in /tshock" });

            Commands.ChatCommands.Add(new Command("pvpcontroller.stats", ModStat, "modstat", "ms") { HelpText = "Modifies item/projectile/buff stats. " + modStatParameters });
            Commands.ChatCommands.Add(new Command("pvpcontroller.all", ModAll, "modall", "ma") { HelpText = "Modifies a setting for all items. " + modAllParameters });

            Commands.ChatCommands.Add(new Command(ToggleTooltip, "toggletooltip", "tt") { HelpText = "Toggles damage/defense tooltip popups." });

            Commands.ChatCommands.Add(new Command("pvpcontroller.dev", MemesFrom2006, "memesfrom2006") { HelpText = "Brings memes from 2006 lol" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.dev", SQLInject, "sqlinject") { HelpText = "Allows you to run a SQL command" });
        }

        private static void MemesFrom2006(CommandArgs args) {
        }

        private static void ModStat(CommandArgs args) {
            var player = args.Player;
            var input = args.Parameters;
            string type = "";
            int id = 0;
            string stat = "";
            object value;
            bool modifyProjectile = false;

            if (input.Count < 4) {
                player.SendErrorMessage("Invalid Parameters. " + modStatParameters);
                return;
            }

            switch (input[0].ToLower()) {
                case "items":
                case "item":
                case "i":
                    type = DBConsts.ItemTable;
                    break;

                case "projectiles":
                case "projectile":
                case "proj":
                case "p":
                    type = DBConsts.ProjectileTable;
                    break;

                case "buffs":
                case "buff":
                case "b":
                    type = DBConsts.BuffTable;
                    break;

                default:
                    player.SendErrorMessage("Invalid Type. Possible values are: {0}. You typed {1}.".SFormat(tableList), input[1]);
                    return;
            }

            if (!int.TryParse(input[1], out id)) {
                var foundSearches = TShock.Utils.GetIDFromInput(type, input[1]);

                if (foundSearches.Count == 1) {
                    id = foundSearches[0];
                } else {
                    if (foundSearches.Count == 0) {
                        player.SendErrorMessage("Found no {0} of name {1}".SFormat(type, input[1]));
                    } else {
                        player.SendErrorMessage("Found multiple {0} of name {1}".SFormat(type, input[1]));
                        foreach(int foundID in foundSearches) {
                            player.SendErrorMessage(MiscUtils.GetNameFromInput(type, foundID));
                        }
                    }
                    return;
                }
            }

            switch (input[2].ToLower()) {
                case "name":
                case "n":
                    stat = DBConsts.Name;
                    break;

                case "damage":
                case "dmg":
                case "d":
                    stat = DBConsts.Damage;
                    break;

                case "shoot":
                case "s":
                    stat = DBConsts.Shoot;
                    modifyProjectile = true;
                    break;

                case "isshootmodded":
                case "ism":
                    stat = DBConsts.IsShootModded;
                    break;

                case "shootspeed":
                case "ss":
                    stat = DBConsts.ShootSpeed;
                    modifyProjectile = true;
                    break;

                case "knockback":
                case "kb":
                    stat = DBConsts.Knockback;
                    break;

                case "defense":
                case "def":
                    stat = DBConsts.Defense;
                    break;

                case "inflictbuffid":
                case "ibid":
                case "ibi":
                    stat = DBConsts.InflictBuffID;
                    break;

                case "inflictbuffduration":
                case "ibd":
                    stat = DBConsts.InflictBuffDuration;
                    break;

                case "receivebuffid":
                case "rbid":
                case "rbi":
                    stat = DBConsts.ReceiveBuffID;
                    break;

                case "receivebuffduration":
                case "rbd":
                    stat = DBConsts.ReceiveBuffDuration;
                    break;

                default:
                    player.SendErrorMessage("Invalid stat of " + input[2] + ". Parameters: " + statList);
                    return;
            }

            if (!MiscUtils.TryConvertStringToType(Database.GetType(type, stat), input[3], out value)) {
                player.SendErrorMessage("{0} do not have the {1} stat.".SFormat(type, stat));
                return;
            }

            if (Database.Update(type, id, stat, value)) {
                args.Player.SendSuccessMessage("Successfully converted ({0}){1}'s {2} to {3}".SFormat(type, MiscUtils.GetNameFromInput(type, id), stat, value));
                if (modifyProjectile) {
                    Database.Update(type, id, DBConsts.IsShootModded, modifyProjectile);
                }
            } else {
                args.Player.SendErrorMessage("Failed to convert ({0}){1}'s {2} to {3}".SFormat(type, MiscUtils.GetNameFromInput(type, id), stat, value));
            }
        }

        private static void ModConfig(CommandArgs args) {
            var player = args.Player;

            if (args.Parameters.Count != 2) {
                player.SendErrorMessage("Invalid parameters. " + modConfigParameters);
                return;
            }

            var varType = args.Parameters[0].ToLower();
            var value = args.Parameters[1];
            bool success = true;

            switch (varType) {
                case "enableplugin":
                case "ep":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enablePlugin, value);
                    break;

                case "enabledamagechanges":
                case "edc":
                case "ed":
                case "d":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableDamageChanges, value);
                    break;

                case "enablecriticals":
                case "ec":
                case "c":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableCriticals, value);
                    break;

                case "enableknockback":
                case "ek":
                case "k":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableKnockback, value);
                    break;

                case "enableminions":
                case "em":
                case "m":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableMinions, value);
                    break;

                case "enableprojectiledebuffs":
                case "epd":
                case "pd":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableProjectileDebuffs, value);
                    break;

                case "enableprojectileselfbuffs":
                case "epsb":
                case "psb":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableProjectileSelfBuffs, value);
                    break;

                case "enableweapondebuffs":
                case "ewd":
                case "wd":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableWeaponDebuffs, value);
                    break;

                case "enableweaponselfbuffs":
                case "ewsb":
                case "wsb":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableWeaponSelfBuffs, value);
                    break;

                case "enableturtle":
                case "eturtle":
                case "turtle":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableTurtle, value);
                    break;

                case "turtlemultiplier":
                case "turtlem":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.turtleMultiplier, value);
                    break;

                case "enablethorns":
                case "ethorns":
                case "thorns":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableThorns, value);
                    break;

                case "thornmultiplier":
                case "thornm":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.thornMultiplier, value);
                    break;

                case "enablenebula":
                case "nebula":
                case "en":
                case "n":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableNebula, value);
                    break;

                case "nebulatier3duration":
                case "nt3d":
                case "n3":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.nebulaTier3Duration, value);
                    break;

                case "nebulatier2duration":
                case "nt2d":
                case "n2":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.nebulaTier2Duration, value);
                    break;

                case "nebulatier1duration":
                case "nt1d":
                case "n1":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.nebulaTier1Duration, value);
                    break;

                case "enablefrost":
                case "efrost":
                case "frost":
                case "f":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableFrost, value);
                    break;

                case "frostduration":
                case "fd":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.frostDuration, value);
                    break;

                case "vortexmultiplier":
                case "vm":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.vortexMultiplier, value);
                    break;

                case "enablebuffdebuff":
                case "ebd":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableBuffDebuff, value);
                    break;

                case "enablebuffselfbuff":
                case "ebsb":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.enableBuffSelfBuff, value);
                    break;

                case "knockbackminimum":
                case "km":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.knockbackMinimum, value);
                    break;

                case "iframetime":
                case "ift":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.iframeTime, value);
                    break;

                case "lowerdamagevariance":
                case "ldv":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.lowerDamageVariance, value);
                    break;

                case "upperdamagevariance":
                case "udv":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.upperDamageVariance, value);
                    break;

                case "deathitemtag":
                case "dit":
                    success = MiscUtils.SetValueWithString(ref PvPController.config.deathItemTag, value);
                    break;

                default:
                    player.SendErrorMessage("Invalid config variable of: " + varType);
                    return;
            }

            if (!success) {
                player.SendErrorMessage("Failed to set value {0} to {1}.".SFormat(value, varType));
                return;
            }

            player.SendSuccessMessage("Set {0} to value {1}.".SFormat(varType, value));
            PvPController.config.Write(Config.configPath);
        }

        private static void ToggleTooltip(CommandArgs args) {
            PvPController.pvpers[args.Player.Index].seeTooltip = !PvPController.pvpers[args.Player.Index].seeTooltip;

            args.Player.SendSuccessMessage("Tooltips: " + PvPController.pvpers[args.Player.Index].seeTooltip);
            Interface.ClearInterface(PvPController.pvpers[args.Player.Index]);
        }

        private static void ModAll(CommandArgs args) {
            var input = args.Parameters;
            if (input.Count < 3) {
                args.Player.SendErrorMessage("Invalid parameters. " + modAllParameters);
                return;
            }

            object convertedType;
            MiscUtils.TryConvertStringToType(Database.GetType(input[0], input[1]), input[2], out convertedType);

            if (Database.Update(input[0], -1, input[1], convertedType)) 
                args.Player.SendSuccessMessage("Successfully converted all {0} in {1} to {2}".SFormat(input[1], input[0], input[2]));
            else 
                args.Player.SendErrorMessage("Failed to convert all {0} in {1} to {2}".SFormat(input[1], input[0], input[2]));
        }

        private static void WriteConfig(CommandArgs args) {
            PvPController.config.Write(Config.configPath);
            args.Player.SendSuccessMessage("Written server pvp changes to config.");
        }

        private static void ResetPvP(CommandArgs args) {
            if (args.Parameters.Count < 1) {
                args.Player.SendErrorMessage("Invalid Syntax: " + resetList);
                return;
            }

            int id;

            switch (args.Parameters[0].ToLower()) {
                case "database":
                case "d":
                    Database.InitDefaultTables();
                    args.Player.SendSuccessMessage("Reset database to default.");
                    return;

                case "config":
                case "c":
                    PvPController.config.ResetConfigValues();
                    PvPController.config.Write(Config.configPath);
                    args.Player.SendSuccessMessage("Reset config values to default.");
                    return;

                case "item":
                case "i":
                    if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out id)) {
                        args.Player.SendErrorMessage("Please provide a valid id.");
                        return;
                    }

                    Database.DeleteRow(DBConsts.ItemTable, id);
                    Database.Query(Database.GetDefaultValueSQLString(DBConsts.ItemTable, id));
                    args.Player.SendSuccessMessage("Reset the values of {0}".SFormat(MiscUtils.GetNameFromInput(DBConsts.ItemTable, id)));
                    return;

                case "projectile":
                case "p":
                    if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out id)) {
                        args.Player.SendErrorMessage("Please provide a valid id.");
                        return;
                    }

                    Database.DeleteRow(DBConsts.ProjectileTable, id);
                    Database.Query(Database.GetDefaultValueSQLString(DBConsts.ProjectileTable, id));
                    args.Player.SendSuccessMessage("Reset the values of {0}".SFormat(MiscUtils.GetNameFromInput(DBConsts.ProjectileTable, id)));
                    return;

                case "buff":
                case "b":
                    if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out id)) {
                        args.Player.SendErrorMessage("Please provide a valid id.");
                        return;
                    }

                    Database.DeleteRow(DBConsts.BuffTable, id);
                    Database.Query(Database.GetDefaultValueSQLString(DBConsts.BuffTable, id));
                    args.Player.SendSuccessMessage("Reset the values of {0}".SFormat(MiscUtils.GetNameFromInput(DBConsts.BuffTable, id)));
                    return;

                default:
                    args.Player.SendErrorMessage("Invalid parameters. " + resetList);
                    return;
            }
        }

        private static void ResetDatabase(CommandArgs args) {
        }

        private static void Reload(CommandArgs args) {
            PvPController.config = Config.Read(Config.configPath);
            args.Player.SendSuccessMessage("PvP config reloaded to server.");
        }

        private static void WriteDocumentation(CommandArgs args) {
            PvPController.config.WriteDocumentation();
            args.Player.SendSuccessMessage("Wrote documentation in a .txt file in /tshock.");
        }

        private static void SQLInject(CommandArgs args) {
            string statement = string.Join(" ", args.Parameters);

            if (!Database.Query(statement))
                args.Player.SendErrorMessage("SQL statement failed.");
            else
                args.Player.SendSuccessMessage("SQL statement was successful.");
        }
    }
}
