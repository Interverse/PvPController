using PvPController.Utilities;
using TShockAPI;

namespace PvPController {
    public class PluginCommands {
        private static readonly string ModAllParameters = "Parameters: <Items/Projectiles/Buffs> <Name/Shoot/IsShootModded/ShootSpeed/Knockback/Defense/InflictBuffID/InflictBuffDuration/ReceiveBuffID/ReceiveBuffDuration> <value>";

        private static readonly string TableList = "<{0}>".SFormat(string.Join("/", DbConsts.ItemTable, DbConsts.ProjectileTable, DbConsts.BuffTable));
        private static readonly string ItemIdParam = "<\"item Name\"/id>";
        private static readonly string StatList = "<{0}>".SFormat(string.Join("/", DbConsts.Name, DbConsts.Shoot, DbConsts.IsShootModded, DbConsts.ShootSpeed, DbConsts.Knockback, DbConsts.Defense, DbConsts.InflictBuffId, DbConsts.InflictBuffDuration, DbConsts.ReceiveBuffId, DbConsts.ReceiveBuffDuration).SeparateToLines(60, "/"));
        private static readonly string ModStatParameters = "Parameters: {0} {1}\r\n{2} <value>".SFormat(TableList, ItemIdParam, StatList);

        private static readonly string ConfigList = "<{0}>".SFormat(string.Join("/", "enableplugin", "enabledamagechanges", "enablecriticals",
            "enableknockback", "enableminions", "enableprojectiledebuffs", "enableprojectileselfbuffs", "enableweapondebuffs",
            "enableweaponselfbuffs", "enableturtle", "turtlemultiplier", "enablethorns", "thornmultiplier", "enablenebula",
            "nebulatier3duration", "nebulatier2duration", "nebulatier1duration", "enablefrost", "frostduration", "vortexmultiplier",
            "enablebuffdebuff", "enablebuffselfbuff", "knockbackminimum", "iframetime", "lowerdamagevariance", "upperdamagevariance",
            "deathitemtag").SeparateToLines(60, "/"));
        private static readonly string ModConfigParameters = "Parameters: {0}".SFormat(ConfigList);

        private static readonly string ResetList = "Parameters: <config/database/item/projectile/buff>";

        public static void RegisterCommands() {
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", ModConfig, "modconfig", "mc") { HelpText = "Sets config settings to server" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", Reload, "reload", "readconfig") { HelpText = "Sets config settings to server" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", WriteConfig, "writeconfig") { HelpText = "Writes server settings to config" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", ResetPvP, "resetpvp", "rpvp") { HelpText = "Reset values to default" });
            Commands.ChatCommands.Add(new Command("pvpcontroller.config", WriteDocumentation, "writedocumentation") { HelpText = "Writes documentation to a .txt file in /tshock" });

            Commands.ChatCommands.Add(new Command("pvpcontroller.stats", ModStat, "modstat", "ms") { HelpText = "Modifies item/projectile/buff stats. " + ModStatParameters });
            Commands.ChatCommands.Add(new Command("pvpcontroller.all", ModAll, "modall", "ma") { HelpText = "Modifies a setting for all items. " + ModAllParameters });

            Commands.ChatCommands.Add(new Command(ToggleTooltip, "toggletooltip", "tt") { HelpText = "Toggles damage/defense tooltip popups." });
            
            Commands.ChatCommands.Add(new Command("pvpcontroller.dev", SqlInject, "sqlinject") { HelpText = "Allows you to run a SQL command" });
        }

        private static void ModStat(CommandArgs args) {
            var player = args.Player;
            var input = args.Parameters;
            string type;
            string stat;
            bool modifyProjectile = false;

            if (input.Count < 4) {
                player.SendErrorMessage("Invalid Parameters. " + ModStatParameters);
                return;
            }

            switch (input[0].ToLower()) {
                case "items":
                case "item":
                case "i":
                    type = DbConsts.ItemTable;
                    break;

                case "projectiles":
                case "projectile":
                case "proj":
                case "p":
                    type = DbConsts.ProjectileTable;
                    break;

                case "buffs":
                case "buff":
                case "b":
                    type = DbConsts.BuffTable;
                    break;

                default:
                    player.SendErrorMessage("Invalid Type. Possible values are: {0}. You typed {1}.".SFormat(TableList), input[1]);
                    return;
            }

            if (!int.TryParse(input[1], out int id)) {
                var foundSearches = TShock.Utils.GetIdFromInput(type, input[1]);

                if (foundSearches.Count == 1) {
                    id = foundSearches[0];
                } else {
                    if (foundSearches.Count == 0) {
                        player.SendErrorMessage("Found no {0} of Name {1}".SFormat(type, input[1]));
                    } else {
                        player.SendErrorMessage("Found multiple {0} of Name {1}".SFormat(type, input[1]));
                        foreach(int foundId in foundSearches) {
                            player.SendErrorMessage(MiscUtils.GetNameFromInput(type, foundId));
                        }
                    }
                    return;
                }
            }

            switch (input[2].ToLower()) {
                case "Name":
                case "n":
                    stat = DbConsts.Name;
                    break;

                case "damage":
                case "dmg":
                case "d":
                    stat = DbConsts.Damage;
                    break;

                case "shoot":
                case "s":
                    stat = DbConsts.Shoot;
                    modifyProjectile = true;
                    break;

                case "isshootmodded":
                case "ism":
                    stat = DbConsts.IsShootModded;
                    break;

                case "shootspeed":
                case "ss":
                    stat = DbConsts.ShootSpeed;
                    modifyProjectile = true;
                    break;

                case "knockback":
                case "kb":
                    stat = DbConsts.Knockback;
                    break;

                case "defense":
                case "def":
                    stat = DbConsts.Defense;
                    break;

                case "inflictbuffid":
                case "ibid":
                case "ibi":
                    stat = DbConsts.InflictBuffId;
                    break;

                case "inflictbuffduration":
                case "ibd":
                    stat = DbConsts.InflictBuffDuration;
                    break;

                case "receivebuffid":
                case "rbid":
                case "rbi":
                    stat = DbConsts.ReceiveBuffId;
                    break;

                case "receivebuffduration":
                case "rbd":
                    stat = DbConsts.ReceiveBuffDuration;
                    break;

                default:
                    player.SendErrorMessage("Invalid stat of " + input[2] + ". Parameters: " + StatList);
                    return;
            }

            if (!MiscUtils.TryConvertStringToType(Database.GetType(type, stat), input[3], out var value)) {
                player.SendErrorMessage("{0} does not have the {1} stat.".SFormat(type, stat));
                return;
            }

            if (Database.Update(type, id, stat, value)) {
                args.Player.SendSuccessMessage("Successfully converted ({0}){1}'s {2} to {3}".SFormat(type, MiscUtils.GetNameFromInput(type, id), stat, value));
                if (modifyProjectile) {
                    Database.Update(type, id, DbConsts.IsShootModded, true);
                }
            } else {
                args.Player.SendErrorMessage("Failed to convert ({0}){1}'s {2} to {3}".SFormat(type, MiscUtils.GetNameFromInput(type, id), stat, value));
            }
        }

        private static void ModConfig(CommandArgs args) {
            var player = args.Player;

            if (args.Parameters.Count != 2) {
                player.SendErrorMessage("Invalid parameters. " + ModConfigParameters);
                return;
            }

            var varType = args.Parameters[0].ToLower();
            var value = args.Parameters[1];
            bool success;

            switch (varType) {
                case "enableplugin":
                case "ep":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnablePlugin, value);
                    break;

                case "enabledamagechanges":
                case "edc":
                case "ed":
                case "d":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableDamageChanges, value);
                    break;

                case "enablecriticals":
                case "ec":
                case "c":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableCriticals, value);
                    break;

                case "enableknockback":
                case "ek":
                case "k":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableKnockback, value);
                    break;

                case "enableminions":
                case "em":
                case "m":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableMinions, value);
                    break;

                case "enableprojectiledebuffs":
                case "epd":
                case "pd":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableProjectileDebuffs, value);
                    break;

                case "enableprojectileselfbuffs":
                case "epsb":
                case "psb":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableProjectileSelfBuffs, value);
                    break;

                case "enableweapondebuffs":
                case "ewd":
                case "wd":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableWeaponDebuffs, value);
                    break;

                case "enableweaponselfbuffs":
                case "ewsb":
                case "wsb":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableWeaponSelfBuffs, value);
                    break;

                case "enableturtle":
                case "eturtle":
                case "turtle":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableTurtle, value);
                    break;

                case "turtlemultiplier":
                case "turtlem":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.TurtleMultiplier, value);
                    break;

                case "enablethorns":
                case "ethorns":
                case "thorns":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableThorns, value);
                    break;

                case "thornmultiplier":
                case "thornm":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.ThornMultiplier, value);
                    break;

                case "enablenebula":
                case "nebula":
                case "en":
                case "n":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableNebula, value);
                    break;

                case "nebulatier3duration":
                case "nt3d":
                case "n3":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.NebulaTier3Duration, value);
                    break;

                case "nebulatier2duration":
                case "nt2d":
                case "n2":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.NebulaTier2Duration, value);
                    break;

                case "nebulatier1duration":
                case "nt1d":
                case "n1":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.NebulaTier1Duration, value);
                    break;

                case "enablefrost":
                case "efrost":
                case "frost":
                case "f":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableFrost, value);
                    break;

                case "frostduration":
                case "fd":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.FrostDuration, value);
                    break;

                case "vortexmultiplier":
                case "vm":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.VortexMultiplier, value);
                    break;

                case "enablebuffdebuff":
                case "ebd":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableBuffDebuff, value);
                    break;

                case "enablebuffselfbuff":
                case "ebsb":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.EnableBuffSelfBuff, value);
                    break;

                case "knockbackminimum":
                case "km":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.KnockbackMinimum, value);
                    break;

                case "iframetime":
                case "ift":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.IframeTime, value);
                    break;

                case "lowerdamagevariance":
                case "ldv":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.LowerDamageVariance, value);
                    break;

                case "upperdamagevariance":
                case "udv":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.UpperDamageVariance, value);
                    break;

                case "deathitemtag":
                case "dit":
                    success = MiscUtils.SetValueWithString(ref PvPController.Config.DeathItemTag, value);
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
            PvPController.Config.Write(Config.ConfigPath);
        }

        private static void ToggleTooltip(CommandArgs args) {
            PvPController.PvPers[args.Player.Index].SeeTooltip = !PvPController.PvPers[args.Player.Index].SeeTooltip;

            args.Player.SendSuccessMessage("Tooltips: " + PvPController.PvPers[args.Player.Index].SeeTooltip);
            Interface.ClearInterface(PvPController.PvPers[args.Player.Index]);
        }

        private static void ModAll(CommandArgs args) {
            var input = args.Parameters;
            if (input.Count < 3) {
                args.Player.SendErrorMessage("Invalid parameters. " + ModAllParameters);
                return;
            }

            MiscUtils.TryConvertStringToType(Database.GetType(input[0], input[1]), input[2], out var convertedType);

            if (Database.Update(input[0], -1, input[1], convertedType)) 
                args.Player.SendSuccessMessage("Successfully converted all {0} in {1} to {2}".SFormat(input[1], input[0], input[2]));
            else 
                args.Player.SendErrorMessage("Failed to convert all {0} in {1} to {2}".SFormat(input[1], input[0], input[2]));
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

            int id;
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


            if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out id)) {
                args.Player.SendErrorMessage("Please provide a valid id.");
                return;
            }

            Database.DeleteRow(table, id);
            Database.Query(Database.GetDefaultValueSqlString(table, id));
            args.Player.SendSuccessMessage("Reset the values of {0}".SFormat(MiscUtils.GetNameFromInput(table, id)));
        }

        private static void Reload(CommandArgs args) {
            PvPController.Config = Config.Read(Config.ConfigPath);
            args.Player.SendSuccessMessage("PvP config reloaded to server.");
        }

        private static void WriteDocumentation(CommandArgs args) {
            PvPController.Config.WriteDocumentation();
            args.Player.SendSuccessMessage("Wrote documentation in a .txt file in /tshock.");
        }

        private static void SqlInject(CommandArgs args) {
            string statement = string.Join(" ", args.Parameters);

            if (!Database.Query(statement))
                args.Player.SendErrorMessage("SQL statement failed.");
            else
                args.Player.SendSuccessMessage("SQL statement was successful.");
        }
    }
}
