using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using PvPController.Utilities;
using Terraria;
using TShockAPI;

namespace PvPController {
    public class Config {
        public static string ConfigPath = Path.Combine(TShock.SavePath, "pvpcontroller.json");
        public static string DocumentationPath = Path.Combine(TShock.SavePath, "pvpdocs.txt");

        public bool EnablePlugin;

        public bool EnableDamageChanges;
        public bool EnableCriticals;
        public bool EnableKnockback;
        public bool EnableMinions;

        public bool EnableProjectileDebuffs;
        public bool EnableProjectileSelfBuffs;
        public bool EnableWeaponDebuffs;
        public bool EnableWeaponSelfBuffs;

        public bool EnableTurtle;
        public double TurtleMultiplier;
        public bool EnableThorns;
        public double ThornMultiplier;

        public bool EnableNebula;
        public double NebulaTier3Duration;
        public double NebulaTier2Duration;
        public double NebulaTier1Duration;

        public bool EnableFrost;
        public double FrostDuration;

        public double VortexMultiplier;

        public bool EnableBuffDebuff;
        public bool EnableBuffSelfBuff;

        public double KnockbackMinimum;

        public double IframeTime;

        public int LowerDamageVariance;
        public int UpperDamageVariance;

        public string DeathItemTag;
        
        public List<string> NormalDeathMessages = new List<string>();
        
        public List<string> ReflectedDeathMessages = new List<string>();

        public bool FirstConfigGeneration = true;

        /// <summary>
        /// Writes the current internal server config to the external .json file
        /// </summary>
        /// <param Name="path"></param>
        public void Write(string path) {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        /// <summary>
        /// Reads the .json file and stores its contents into the plugin
        /// </summary>
        public static Config Read(string path) {
            if (!File.Exists(path))
                return new Config();
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
        }

        /// <summary>
        /// Parses all item, projectile, and buff changes and puts it into a .txt file in the tshock folder.
        /// </summary>
        public void WriteDocumentation() {
            StreamWriter sw = new StreamWriter(DocumentationPath, false);

            sw.WriteLine("Melee Weapons");
            sw.WriteLine("--------------------");

            for(int x = 0; x < Main.maxItemTypes; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.melee && Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Damage) > 0)
                    sw.WriteLine("{0} ({1}): {2}{3}{4}"
                        .SFormat(Lang.GetItemName(x).Value,
                        x,
                        Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Damage) + " damage",
                        Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Shoot) > 0
                            ? ", shoots {0}{1}".SFormat(Lang.GetProjectileName(Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Shoot)),
                                Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.ShootSpeed) > 0
                                    ? " with {0} shootspeed".SFormat(Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.ShootSpeed))
                                    : "")
                            : "",
                        ", knockback: {0}".SFormat(Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Knockback))));
            }

            sw.WriteLine("");
            sw.WriteLine("Ranged Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxItemTypes; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.ranged && Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Damage) > 0)
                    sw.WriteLine("{0} ({1}): {2}{3}{4}"
                        .SFormat(Lang.GetItemName(x).Value,
                        x,
                        Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Damage) + " damage",
                        Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Shoot) > 0
                            ? ", shoots {0}{1}".SFormat(Lang.GetProjectileName(Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Shoot)),
                                Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.ShootSpeed) > 0
                                    ? " with {0} shootspeed".SFormat(Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.ShootSpeed))
                                    : "")
                            : "",
                        ", knockback: {0}".SFormat(Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Knockback))));
            }

            sw.WriteLine("");
            sw.WriteLine("Magic Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxItemTypes; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.magic && Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Damage) > 0)
                    sw.WriteLine("{0} ({1}): {2}{3}{4}"
                        .SFormat(Lang.GetItemName(x).Value,
                        x,
                        Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Damage) + " damage",
                        Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Shoot) > 0
                            ? ", shoots {0}{1}".SFormat(Lang.GetProjectileName(Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Shoot)),
                                Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.ShootSpeed) > 0
                                    ? " with {0} shootspeed".SFormat(Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.ShootSpeed))
                                    : "")
                            : "",
                        ", knockback: {0}".SFormat(Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Knockback))));
            }

            sw.WriteLine("");
            sw.WriteLine("Throwing Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxItemTypes; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.thrown && Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Damage) > 0)
                    sw.WriteLine("{0} ({1}): {2}{3}{4}"
                        .SFormat(Lang.GetItemName(x).Value,
                        x,
                        Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Damage) + " damage",
                        Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Shoot) > 0
                            ? ", shoots {0}{1}".SFormat(Lang.GetProjectileName(Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Shoot)),
                                Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.ShootSpeed) > 0
                                    ? " with {0} shootspeed".SFormat(Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.ShootSpeed))
                                    : "")
                            : "",
                        ", knockback: {0}".SFormat(Database.GetData<int>(DbConsts.ItemTable, item.type, DbConsts.Knockback))));
            }

            sw.WriteLine("");
            sw.WriteLine("Projectile Damage Changes");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxProjectileTypes; x++) {
                if (Database.GetData<int>(DbConsts.ProjectileTable, x, DbConsts.Damage) > 0)
                    sw.WriteLine("{0} ({1}): {2} damage".SFormat(Lang.GetProjectileName(x).Value, x, Database.GetData<int>(DbConsts.ProjectileTable, x, DbConsts.Damage)));
            }

            sw.WriteLine("");
            sw.WriteLine("Projectile Debuffs");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxProjectileTypes; x++) {
                if (Database.GetData<int>(DbConsts.ProjectileTable, x, DbConsts.InflictBuffId) != 0) {
                    var projectileBuff = Database.GetBuffInfo(DbConsts.ProjectileTable, x, true);
                    sw.WriteLine("{0} ({1}) inflicts the {2} debuff for {3} seconds".SFormat(Lang.GetProjectileName(x).Value, x, Lang.GetBuffName(projectileBuff.BuffId), projectileBuff.BuffDuration / 60));
                }
            }

            sw.WriteLine("");
            sw.WriteLine("Projectile Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxProjectileTypes; x++) {
                if (Database.GetData<int>(DbConsts.ProjectileTable, x, DbConsts.ReceiveBuffId) != 0) {
                    var projectileBuff = Database.GetBuffInfo(DbConsts.ProjectileTable, x, false);
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetProjectileName(x).Value, x, Lang.GetBuffName(projectileBuff.BuffId), projectileBuff.BuffDuration / 60));
                }
            }

            sw.WriteLine("");
            sw.WriteLine("Weapon Debuffs");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxItemTypes; x++) {
                if (Database.GetData<int>(DbConsts.ItemTable, x, DbConsts.InflictBuffId) != 0) {
                    var weaponBuff = Database.GetBuffInfo(DbConsts.ItemTable, x, true);
                    sw.WriteLine("{0} ({1}) inflicts {2} debuff for {3} seconds".SFormat(Lang.GetItemName(x).Value, x, Lang.GetBuffName(weaponBuff.BuffId), weaponBuff.BuffDuration / 60));
                }
            }

            sw.WriteLine("");
            sw.WriteLine("Weapon Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxItemTypes; x++) {
                if (Database.GetData<int>(DbConsts.ItemTable, x, DbConsts.ReceiveBuffId) != 0) {
                    var weaponBuff = Database.GetBuffInfo(DbConsts.ItemTable, x, false);
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetItemName(x).Value, x, Lang.GetBuffName(weaponBuff.BuffId), weaponBuff.BuffDuration / 60));
                }
            }

            sw.WriteLine("");
            sw.WriteLine("Buff Debuff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxBuffTypes; x++) {
                if (Database.GetData<int>(DbConsts.BuffTable, x, DbConsts.InflictBuffId) != 0) {
                    var buffBuff = Database.GetBuffInfo(DbConsts.BuffTable, x, true);
                    sw.WriteLine("{0} ({1}) inflicts {2} debuff for {3} seconds".SFormat(Lang.GetBuffName(x), x, Lang.GetBuffName(buffBuff.BuffId), buffBuff.BuffDuration / 60));
                }
            }

            sw.WriteLine("");
            sw.WriteLine("Buff Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxBuffTypes; x++) {
                if (Database.GetData<int>(DbConsts.BuffTable, x, DbConsts.ReceiveBuffId) != 0) {
                    var buffBuff = Database.GetBuffInfo(DbConsts.BuffTable, x, false);
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetBuffName(x), x, Lang.GetBuffName(buffBuff.BuffId), buffBuff.BuffDuration / 60));
                }
            }

            sw.Close();
        }

        /// <summary>
        /// Sets all default values in the config and the sql(ite) database.
        /// </summary>
        public bool SetDefaultValues() {
            if (FirstConfigGeneration) {
                EnablePlugin = true;

                EnableDamageChanges = true;
                EnableCriticals = false;
                EnableKnockback = false;
                EnableMinions = false;

                EnableProjectileDebuffs = true;
                EnableProjectileSelfBuffs = true;
                EnableWeaponDebuffs = true;
                EnableWeaponSelfBuffs = true;

                EnableTurtle = true;
                TurtleMultiplier = 1.0;
                EnableThorns = true;
                ThornMultiplier = 1.0;

                EnableNebula = true;
                NebulaTier3Duration = 1.0;
                NebulaTier2Duration = 3.0;
                NebulaTier1Duration = 5.0;

                EnableFrost = true;
                FrostDuration = 3.0;

                VortexMultiplier = 1.36;

                EnableBuffDebuff = true;
                EnableBuffSelfBuff = true;

                KnockbackMinimum = -1;

                IframeTime = 0.0;

                LowerDamageVariance = 0;
                UpperDamageVariance = 0;

                DeathItemTag = "";

                NormalDeathMessages = PresetData.NormalDeathMessages;
                ReflectedDeathMessages = PresetData.ReflectedDeathMessages;

                FirstConfigGeneration = false;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Resets all values of the config to its default values.
        /// </summary>
        public void ResetConfigValues() {
            FirstConfigGeneration = true;
            SetDefaultValues();
        }
    }
}
