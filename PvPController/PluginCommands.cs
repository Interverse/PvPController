using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PvPController.Utilities;
using Terraria;
using TShockAPI;

namespace PvPController {
    public class PluginCommands {
        private static readonly string TableList = "<{0}>".SFormat(string.Join("/", DbConsts.ItemTable, DbConsts.ProjectileTable, DbConsts.BuffTable));
        private const string ItemIdParam = "<\"item Name\"/id>";
        private static readonly string StatList = "<{0}>".SFormat(string.Join("/", DbConsts.Name, DbConsts.Shoot, DbConsts.IsShootModded, DbConsts.ShootSpeed, DbConsts.Knockback, DbConsts.Defense, DbConsts.Wrath, DbConsts.Titan, DbConsts.Endurance, DbConsts.InflictBuffId, DbConsts.InflictBuffDuration, DbConsts.ReceiveBuffId, DbConsts.ReceiveBuffDuration).SeparateToLines(60, "/"));
        private static readonly string ModStatParameters = "Parameters: {0} {1}\r\n{2} <value>".SFormat(TableList, ItemIdParam, StatList);
        private static readonly string CheckStatParameters = "Parameters: {0} {1}\r\n{2}".SFormat(TableList, ItemIdParam, StatList);

        private static readonly string ModAllParameters = "Parameters: {0} {1} <value>".SFormat(TableList, StatList);

        private const string ConfigHelp = "<main/pvp/buff/armor/misc>";
        private const string MainConfig = "enableplugin, enabledamagechanges, enablecriticals, deathitemtag, enableknockback, enabletooltip";
        private const string PvPConfig = "knockbackmimimum, iframetime, lowerdamagevariation, upperdamagevariation, lowermagicdamagepercentage, uppermagicdamagepercentage";
        private const string BuffConfig = "enableprojdebuffs, enableprojselfbuffs, enablewepdebuffs, enablewepselfbuffs, enablebuffdebuff, enablebuffselfbuff, healthbasedbuffduration";
        private const string ArmorConfig = "enableturtle, turtlemultiplier, enablenebula, nebulatier(1/2/3)duration, enablefrost, frostduration";
        private const string MiscConfig = "enablethorns, thornsmultiplier, vortexmultiplier, parrytime";

        private const string ResetList = "Parameters: <config/database/item/projectile/buff>";

        public static void RegisterCommands() {
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", ModConfig, "modconfig", "mc") { HelpText = "Sets config settings to server" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", Reload, "reload", "readconfig") { HelpText = "Sets config settings to server" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", WriteConfig, "writeconfig") { HelpText = "Writes server settings to config" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", ResetPvP, "resetpvp", "rpvp") { HelpText = "Reset values to default" });

            Commands.ChatCommands.Add(new Command("pvpcontroller.stats", ModStat, "modstat", "ms") { HelpText = "Modifies item/projectile/buff stats. " + ModStatParameters });
            Commands.ChatCommands.Add(new Command("pvpcontroller.all", ModAll, "modall", "ma") { HelpText = "Modifies a setting for all items. " + ModAllParameters });
            Commands.ChatCommands.Add(new Command("pvpcontroller.all", DPSify, "dpsify") { HelpText = "Changes all weapon's damage to be around the same dps."});

            Commands.ChatCommands.Add(new Command(ToggleTooltip, "toggletooltip", "tt") { HelpText = "Toggles damage/defense tooltip popups." });
            Commands.ChatCommands.Add(new Command(CheckStat, "checkstat", "cs") { HelpText = "Checks a stat of an item." });

            Commands.ChatCommands.Add(new Command("pvpcontroller.dev", SqlInject, "sqlinject") { HelpText = "Allows you to run a SQL command" });
        }

        private static void DPSify(CommandArgs args) {
            List<string> queries = new List<string>();

            if (args.Parameters.Count < 1 || !Double.TryParse(args.Parameters[0], out double dps)) {
                args.Player.SendErrorMessage("Invalid dps value.");
                return;
            }

            dps = dps * dps;

            for (int x = 0; x < Terraria.Main.maxItemTypes; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.damage > 0 && item.ammo == 0) {
                    queries.Add($"UPDATE {DbConsts.ItemTable} SET {DbConsts.Damage} = {(int)Math.Sqrt(dps * item.useTime / 60.0)} WHERE ID = {x}");
                }
            }

            Database.PerformTransaction(queries.ToArray());
            string log = $"Set all weapon's pvp dps to be approx {Math.Sqrt(dps)}";
            args.Player.SendSuccessMessage(log);
            PvPController.Config.LogChange($"({DateTime.Now}) {log}");
        }

        private static void ModStat(CommandArgs args) {
            var player = args.Player;
            var input = args.Parameters;
            bool modifyProjectile = false;

            if (input.Count < 4) {
                player.SendErrorMessage("Invalid Parameters. " + ModStatParameters);
                return;
            }

            if (!TryGetTableFromString(input[0], out string type)) {
                player.SendErrorMessage($"Invalid Type. Possible values are: {TableList}. You typed {input[1]}.");
                return;
            }

            if (!int.TryParse(input[1], out int id)) {
                var foundSearches = TShock.Utils.GetIdFromInput(type, input[1]);

                if (foundSearches.Count == 1) {
                    id = foundSearches[0];
                } else {
                    if (foundSearches.Count == 0) {
                        player.SendErrorMessage($"Found no {type} of Name {input[1]}");
                    } else {
                        player.SendErrorMessage($"Found multiple {type} of Name {input[1]}");
                        foreach(int foundId in foundSearches) {
                            player.SendErrorMessage(MiscUtils.GetNameFromInput(type, foundId));
                        }
                    }
                    return;
                }
            }

            if (!TryGetAttributeFromString(input[2], out string stat)) {
                player.SendErrorMessage("Invalid stat of " + input[2] + ". Parameters: " + StatList);
                return;
            }

            if (stat == DbConsts.Shoot) modifyProjectile = true;

            Type sqlType = Database.GetType(type, stat);

            if (sqlType == default(Type)) {
                player.SendErrorMessage($"{type} do not have the {stat} stat.");
                return;
            }

            if (!MiscUtils.TryConvertStringToType(sqlType, input[3], out var value)) {
                player.SendErrorMessage($"{stat} is incompatible with the value of {input[3]}.");
                return;
            }

            if (Database.Update(type, id, stat, value)) {
                string log = "Successfully converted ({0}){1}'s {2} to {3}".SFormat(type, MiscUtils.GetNameFromInput(type, id), stat, value);
                args.Player.SendSuccessMessage(log);
                PvPController.Config.LogChange($"({DateTime.Now}) {log}");
                if (modifyProjectile) {
                    Database.Update(type, id, DbConsts.IsShootModded, 1);
                }
            } else {
                args.Player.SendErrorMessage("Failed to convert ({0}){1}'s {2} to {3}".SFormat(type, MiscUtils.GetNameFromInput(type, id), stat, value));
            }
        }

        private static void ModConfig(CommandArgs args) {
            var player = args.Player;

            if (args.Parameters.Count < 1) {
                player.SendErrorMessage("Invalid parameters. Type /modconfig " + ConfigHelp + " for config values");
                return;
            }

            switch (args.Parameters[0].ToLower()) {
                case "main":
                    player.SendMessage("Main config values: " + MainConfig, Color.Yellow);
                    return;
                case "pvp":
                    player.SendMessage("PvP config values: " + PvPConfig, Color.Yellow);
                    return;
                case "buff":
                    player.SendMessage("Buff config values: " + BuffConfig, Color.Yellow);
                    return;
                case "armor":
                    player.SendMessage("Armor config values: " + ArmorConfig, Color.Yellow);
                    return;
                case "misc":
                    player.SendMessage("Misc config values: " + MiscConfig, Color.Yellow);
                    return;
            }

            if (args.Parameters.Count != 2) {
                player.SendErrorMessage("Invalid parameters. Type /modconfig <config name> <value>");
                return;
            }

            var configParam = GetConfigAttributeFromString(args.Parameters[0].ToLower());
            var value = args.Parameters[1];

            if (configParam == default(string)) {
                player.SendErrorMessage($"Config value {args.Parameters[0].ToLower()} doesn't exist.");
                return;
            }

            if (!MiscUtils.SetValueWithString(PvPController.Config, configParam, value)) {
                player.SendErrorMessage($"Failed to set value {value} to {configParam}.");
                return;
            }

            string log = $"Set {configParam} to value {value}.";
            player.SendSuccessMessage(log);
            PvPController.Config.LogChange($"({DateTime.Now}) {log}");
            PvPController.Config.Write(Config.ConfigPath);
        }

        private static void ToggleTooltip(CommandArgs args) {
            PvPController.PvPers[args.Player.Index].SeeTooltip = !PvPController.PvPers[args.Player.Index].SeeTooltip;

            args.Player.SendSuccessMessage("Tooltips: " + PvPController.PvPers[args.Player.Index].SeeTooltip);
            Interface.ClearInterface(PvPController.PvPers[args.Player.Index]);
        }
        
        private static void CheckStat(CommandArgs args) {
            var player = args.Player;
            var input = args.Parameters;

            if (input.Count < 3) {
                player.SendErrorMessage("Invalid Parameters. " + CheckStatParameters);
                return;
            }

            if (!TryGetTableFromString(input[0], out string type)) {
                player.SendErrorMessage($"Invalid Type. Possible values are: {TableList}. You typed {input[0]}.");
                return;
            }

            if (!int.TryParse(input[1], out int id)) {
                var foundSearches = TShock.Utils.GetIdFromInput(type, input[1]);

                if (foundSearches.Count == 1) {
                    id = foundSearches[0];
                } else {
                    if (foundSearches.Count == 0) {
                        player.SendErrorMessage($"Found no {type} of Name {input[1]}");
                    } else {
                        player.SendErrorMessage($"Found multiple {type} of Name {input[1]}");
                        foreach (int foundId in foundSearches) {
                            player.SendErrorMessage(MiscUtils.GetNameFromInput(type, foundId));
                        }
                    }
                    return;
                }
            }

            if (!TryGetAttributeFromString(input[2], out string stat)) {
                player.SendErrorMessage("Invalid stat of " + input[2] + ". Parameters: " + StatList);
                return;
            }

            Type sqlType = Database.GetType(type, stat);

            if (sqlType == default(Type)) {
                player.SendErrorMessage($"{type} do not have the {stat} stat.");
                return;
            }

            player.SendMessage("({0}){1}'s {2}: {3}".SFormat(type, MiscUtils.GetNameFromInput(type, id), stat, Database.GetDataWithType(type, id, stat, sqlType)), Color.Yellow);
        }

        private static void ModAll(CommandArgs args) {
            var input = args.Parameters;
            if (input.Count < 3) {
                args.Player.SendErrorMessage("Invalid parameters. " + ModAllParameters);
                return;
            }

            if (!TryGetTableFromString(input[0], out string table)) {
                args.Player.SendErrorMessage($"Invalid Type. Possible values are: {TableList}. You typed {input[1]}.");
                return;
            }

            if (!TryGetAttributeFromString(input[1], out string attribute)) {
                args.Player.SendErrorMessage("Invalid stat of " + input[1] + ". Parameters: " + StatList);
                return;
            }

            if (!MiscUtils.TryConvertStringToType(Database.GetType(table, attribute), input[2], out var convertedType)) {
                args.Player.SendErrorMessage($"{attribute} is incompatible with the value {input[2]}");
                return;
            }

            if (Database.Update(table, -1, attribute, convertedType)) {
                string log = $"Successfully converted all {attribute} in {table} to {convertedType}";
                args.Player.SendSuccessMessage(log);
                PvPController.Config.LogChange($"({DateTime.Now}) {log}");
            } else {
                args.Player.SendErrorMessage($"Failed to convert all {attribute} in {table} to {convertedType}");
            }
        }

        private static void WriteConfig(CommandArgs args) {
            PvPController.Config.Write(Config.ConfigPath);
            args.Player.SendSuccessMessage("Written server pvp changes to config.");
        }

        private static void ResetPvP(CommandArgs args) {
            if (args.Parameters.Count < 1) {
                args.Player.SendErrorMessage("Invalid Syntax: " + ResetList);
                return;
            }

            string table;

            switch (args.Parameters[0].ToLower()) {
                case "database":
                case "d":
                    Database.InitDefaultTables();
                    args.Player.SendSuccessMessage("Reset database to default.");
                    return;

                case "config":
                case "c":
                    PvPController.Config.ResetConfigValues();
                    PvPController.Config.Write(Config.ConfigPath);
                    args.Player.SendSuccessMessage("Reset config values to default.");
                    return;

                case "item":
                case "i":
                    table = DbConsts.ItemTable;
                    break;

                case "projectile":
                case "p":
                    table = DbConsts.ProjectileTable;
                    break;

                case "buff":
                case "b":
                    table = DbConsts.BuffTable;
                    break;

                default:
                    args.Player.SendErrorMessage("Invalid parameters. " + ResetList);
                    return;
            }


            if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out int id)) {
                args.Player.SendErrorMessage("Please provide a valid id.");
                return;
            }

            Database.DeleteRow(table, id);
            Database.Query(Database.GetDefaultValueSqlString(table, id));

            string log = "Reset the values of {0}".SFormat(MiscUtils.GetNameFromInput(table, id));
            args.Player.SendSuccessMessage(log);
            PvPController.Config.LogChange($"({DateTime.Now}) {log}");
        }

        private static void Reload(CommandArgs args) {
            PvPController.Config = Config.Read(Config.ConfigPath);
            args.Player.SendSuccessMessage("PvP config reloaded to server.");
        }

        private static void SqlInject(CommandArgs args) {
            string statement = string.Join(" ", args.Parameters);

            if (!Database.Query(statement))
                args.Player.SendErrorMessage("SQL statement failed.");
            else
                args.Player.SendSuccessMessage("SQL statement was successful.");
        }

        /// <summary>
        /// Gets the table name from a string.
        /// </summary>
        private static bool TryGetTableFromString(string input, out string table) {
            switch (input.ToLower()) {
                case "items":
                case "item":
                case "i":
                    table = DbConsts.ItemTable;
                    break;

                case "projectiles":
                case "projectile":
                case "proj":
                case "p":
                    table = DbConsts.ProjectileTable;
                    break;

                case "buffs":
                case "buff":
                case "b":
                    table = DbConsts.BuffTable;
                    break;

                default:
                    table = input;
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets an attribute's sql column name from a string.
        /// </summary>
        private static bool TryGetAttributeFromString(string input, out string attribute) {
            switch (input.ToLower()) {
                case "name":
                case "n":
                    attribute = DbConsts.Name;
                    break;

                case "damage":
                case "dmg":
                case "d":
                    attribute = DbConsts.Damage;
                    break;

                case "shoot":
                case "s":
                    attribute = DbConsts.Shoot;
                    break;

                case "isshootmodded":
                case "ism":
                    attribute = DbConsts.IsShootModded;
                    break;

                case "shootspeed":
                case "ss":
                    attribute = DbConsts.ShootSpeed;
                    break;

                case "knockback":
                case "kb":
                    attribute = DbConsts.Knockback;
                    break;

                case "defense":
                case "def":
                    attribute = DbConsts.Defense;
                    break;

                case "inflictbuffid":
                case "ibid":
                case "ibi":
                    attribute = DbConsts.InflictBuffId;
                    break;

                case "inflictbuffduration":
                case "ibd":
                    attribute = DbConsts.InflictBuffDuration;
                    break;

                case "receivebuffid":
                case "rbid":
                case "rbi":
                    attribute = DbConsts.ReceiveBuffId;
                    break;

                case "receivebuffduration":
                case "rbd":
                    attribute = DbConsts.ReceiveBuffDuration;
                    break;

                case "titan":
                case "t":
                    attribute = DbConsts.Titan;
                    break;

                case "endurance":
                case "e":
                    attribute = DbConsts.Endurance;
                    break;

                case "wrath":
                case "w":
                    attribute = DbConsts.Wrath;
                    break;

                default:
                    attribute = input;
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Get's a config value's name from an input
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static string GetConfigAttributeFromString(string val) {
            switch (val) {
                case "enableplugin":
                case "ep":
                    return "EnablePlugin";

                case "enabledamagechanges":
                case "edc":
                case "ed":
                case "d":
                    return "EnableDamageChanges";

                case "enablecriticals":
                case "ec":
                case "c":
                    return "EnableCriticals";

                case "enableknockback":
                case "ek":
                case "k":
                    return "EnableKnockback";

                case "enableminions":
                case "em":
                case "m":
                    return "EnableMinions";
                    
                case "enabletooltip":
                case "et":
                    return "EnableTooltip";

                case "enableprojectiledebuffs":
                case "epd":
                case "pd":
                    return "EnableProjectileDebuffs";

                case "enableprojectileselfbuffs":
                case "epsb":
                case "psb":
                    return "EnableProjectileSelfBuffs";

                case "enableweapondebuffs":
                case "ewd":
                case "wd":
                    return "EnableWeaponDebuffs";

                case "enableweaponselfbuffs":
                case "ewsb":
                case "wsb":
                    return "EnableWeaponSelfBuffs";

                case "healthbasedbuffduration":
                case "hbbd":
                case "hbd":
                    return "HealthBasedBuffDuration";

                case "enableturtle":
                case "eturtle":
                case "turtle":
                    return "EnableTurtle";

                case "turtlemultiplier":
                case "turtlem":
                    return "TurtleMultiplier";

                case "enablethorns":
                case "ethorns":
                case "thorns":
                    return "EnableThorns";

                case "thornmultiplier":
                case "thornm":
                    return "ThornMultiplier";

                case "enablenebula":
                case "nebula":
                case "en":
                case "n":
                    return "EnableNebula";

                case "nebulatier3duration":
                case "nt3d":
                case "n3":
                    return "NebulaTier3Duration";

                case "nebulatier2duration":
                case "nt2d":
                case "n2":
                    return "NebulaTier2Duration";

                case "nebulatier1duration":
                case "nt1d":
                case "n1":
                    return "NebulaTier1Duration";

                case "enablefrost":
                case "efrost":
                case "frost":
                case "f":
                    return "EnableFrost";

                case "frostduration":
                case "fd":
                    return "FrostDuration";

                case "vortexmultiplier":
                case "vm":
                    return "VortexMultiplier";

                case "enablebuffdebuff":
                case "ebd":
                    return "EnableBuffDebuff";

                case "enablebuffselfbuff":
                case "ebsb":
                    return "EnableBuffSelfBuff";

                case "knockbackminimum":
                case "km":
                    return "KnockbackMinimum";

                case "iframetime":
                case "ift":
                    return "IframeTime";

                case "lowerdamagevariance":
                case "ldv":
                    return "LowerDamageVariance";

                case "upperdamagevariance":
                case "udv":
                    return "UpperDamageVariance";

                case "deathitemtag":
                case "dit":
                    return "DeathItemTag";

                case "parrytime":
                case "pt":
                    return "ParryTime";

                case "lowermagicdamagepercentage":
                case "lmdp":
                case "lm":
                    return "LowerMagicDamagePercentage";

                case "uppermagicdamagepercentage":
                case "umdp":
                case "um":
                    return "UpperMagicDamagePercentage";

                default:
                    return val;
            }
        }
    }
}
