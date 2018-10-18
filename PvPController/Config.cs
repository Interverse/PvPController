using Newtonsoft.Json;
using PvPController.Variables;
using System.Collections.Generic;
using System.IO;
using Terraria;
using TShockAPI;

namespace PvPController {
    public class Config {
        public static string configPath = Path.Combine(TShock.SavePath, "pvpcontroller.json");
        public static string documentationPath = Path.Combine(TShock.SavePath, "pvpdocs.txt");

        public bool enablePlugin;

        public bool enableDamageChanges;
        public bool enableCriticals;
        public bool enableKnockback;
        public bool enableMinions;

        public bool enableProjectileDebuffs;
        public bool enableProjectileSelfBuffs;
        public bool enableWeaponDebuffs;
        public bool enableWeaponSelfBuffs;

        public bool enableTurtle;
        public double turtleMultiplier;
        public bool enableThorns;
        public double thornMultiplier;

        public bool enableNebula;
        public double nebulaTier3Duration;
        public double nebulaTier2Duration;
        public double nebulaTier1Duration;

        public bool enableFrost;
        public double frostDuration;

        public double vortexMultiplier;

        public bool enableBuffDebuff;
        public bool enableBuffSelfBuff;

        public double knockbackMinimum;

        public double iframeTime;

        public int lowerDamageVariance;
        public int upperDamageVariance;

        public string deathItemTag;
        
        public List<string> normalDeathMessages = new List<string>();
        
        public List<string> reflectedDeathMessages = new List<string>();

        public bool firstConfigGeneration = true;

        public void Write(string path) {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Config Read(string path) {
            if (!File.Exists(path))
                return new Config();
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
        }

        /// <summary>
        /// Parses all item, projectile, and buff changes and puts it into a .txt file in the /tshock folder.
        /// </summary>
        public void WriteDocumentation() {
            StreamWriter sw = new StreamWriter(documentationPath, false);

            sw.WriteLine("Melee Weapons");
            sw.WriteLine("--------------------");

            for(int x = 0; x < Main.maxItemTypes; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.melee && Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Damage) > 0)
                    sw.WriteLine("{0} ({1}): {2}{3}{4}"
                        .SFormat(Lang.GetItemName(x).Value,
                        x,
                        Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Damage) + " damage",
                        Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Shoot) > 0
                            ? ", shoots {0}{1}".SFormat(Lang.GetProjectileName(Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Shoot)),
                                Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.ShootSpeed) > 0
                                    ? " with {0} shootspeed".SFormat(Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.ShootSpeed))
                                    : "")
                            : "",
                        ", knockback: {0}".SFormat(Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Knockback))));
            }

            sw.WriteLine("");
            sw.WriteLine("Ranged Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxItemTypes; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.ranged && Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Damage) > 0)
                    sw.WriteLine("{0} ({1}): {2}{3}{4}"
                        .SFormat(Lang.GetItemName(x).Value,
                        x,
                        Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Damage) + " damage",
                        Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Shoot) > 0
                            ? ", shoots {0}{1}".SFormat(Lang.GetProjectileName(Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Shoot)),
                                Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.ShootSpeed) > 0
                                    ? " with {0} shootspeed".SFormat(Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.ShootSpeed))
                                    : "")
                            : "",
                        ", knockback: {0}".SFormat(Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Knockback))));
            }

            sw.WriteLine("");
            sw.WriteLine("Magic Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxItemTypes; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.magic && Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Damage) > 0)
                    sw.WriteLine("{0} ({1}): {2}{3}{4}"
                        .SFormat(Lang.GetItemName(x).Value,
                        x,
                        Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Damage) + " damage",
                        Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Shoot) > 0
                            ? ", shoots {0}{1}".SFormat(Lang.GetProjectileName(Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Shoot)),
                                Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.ShootSpeed) > 0
                                    ? " with {0} shootspeed".SFormat(Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.ShootSpeed))
                                    : "")
                            : "",
                        ", knockback: {0}".SFormat(Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Knockback))));
            }

            sw.WriteLine("");
            sw.WriteLine("Throwing Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxItemTypes; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.thrown && Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Damage) > 0)
                    sw.WriteLine("{0} ({1}): {2}{3}{4}"
                        .SFormat(Lang.GetItemName(x).Value,
                        x,
                        Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Damage) + " damage",
                        Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Shoot) > 0
                            ? ", shoots {0}{1}".SFormat(Lang.GetProjectileName(Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Shoot)),
                                Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.ShootSpeed) > 0
                                    ? " with {0} shootspeed".SFormat(Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.ShootSpeed))
                                    : "")
                            : "",
                        ", knockback: {0}".SFormat(Database.GetData<int>(DBConsts.ItemTable, item.type, DBConsts.Knockback))));
            }

            sw.WriteLine("");
            sw.WriteLine("Projectile Damage Changes");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxProjectileTypes; x++) {
                if (Database.GetData<int>(DBConsts.ProjectileTable, x, DBConsts.Damage) > 0)
                    sw.WriteLine("{0} ({1}): {2} damage".SFormat(Lang.GetProjectileName(x).Value, x, Database.GetData<int>(DBConsts.ProjectileTable, x, DBConsts.Damage)));
            }

            sw.WriteLine("");
            sw.WriteLine("Projectile Debuffs");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxProjectileTypes; x++) {
                if (Database.GetData<int>(DBConsts.ProjectileTable, x, DBConsts.InflictBuffID) != 0) {
                    var projectileBuff = Database.GetBuffDuration(DBConsts.ProjectileTable, x, true);
                    sw.WriteLine("{0} ({1}) inflicts the {2} debuff for {3} seconds".SFormat(Lang.GetProjectileName(x).Value, x, Lang.GetBuffName(projectileBuff.buffid), projectileBuff.buffDuration / 60));
                }
            }

            sw.WriteLine("");
            sw.WriteLine("Projectile Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxProjectileTypes; x++) {
                if (Database.GetData<int>(DBConsts.ProjectileTable, x, DBConsts.ReceiveBuffID) != 0) {
                    var projectileBuff = Database.GetBuffDuration(DBConsts.ProjectileTable, x, false);
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetProjectileName(x).Value, x, Lang.GetBuffName(projectileBuff.buffid), projectileBuff.buffDuration / 60));
                }
            }

            sw.WriteLine("");
            sw.WriteLine("Weapon Debuffs");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxItemTypes; x++) {
                if (Database.GetData<int>(DBConsts.ItemTable, x, DBConsts.InflictBuffID) != 0) {
                    var weaponBuff = Database.GetBuffDuration(DBConsts.ItemTable, x, true);
                    sw.WriteLine("{0} ({1}) inflicts {2} debuff for {3} seconds".SFormat(Lang.GetItemName(x).Value, x, Lang.GetBuffName(weaponBuff.buffid), weaponBuff.buffDuration / 60));
                }
            }

            sw.WriteLine("");
            sw.WriteLine("Weapon Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxItemTypes; x++) {
                if (Database.GetData<int>(DBConsts.ItemTable, x, DBConsts.ReceiveBuffID) != 0) {
                    var weaponBuff = Database.GetBuffDuration(DBConsts.ItemTable, x, false);
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetItemName(x).Value, x, Lang.GetBuffName(weaponBuff.buffid), weaponBuff.buffDuration / 60));
                }
            }

            sw.WriteLine("");
            sw.WriteLine("Buff Debuff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxBuffTypes; x++) {
                if (Database.GetData<int>(DBConsts.BuffTable, x, DBConsts.InflictBuffID) != 0) {
                    var buffBuff = Database.GetBuffDuration(DBConsts.BuffTable, x, true);
                    sw.WriteLine("{0} ({1}) inflicts {2} debuff for {3} seconds".SFormat(Lang.GetBuffName(x), x, Lang.GetBuffName(buffBuff.buffid), buffBuff.buffDuration / 60));
                }
            }

            sw.WriteLine("");
            sw.WriteLine("Buff Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Main.maxBuffTypes; x++) {
                if (Database.GetData<int>(DBConsts.BuffTable, x, DBConsts.ReceiveBuffID) != 0) {
                    var buffBuff = Database.GetBuffDuration(DBConsts.BuffTable, x, false);
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetBuffName(x), x, Lang.GetBuffName(buffBuff.buffid), buffBuff.buffDuration / 60));
                }
            }

            sw.Close();
        }

        /// <summary>
        /// Sets all default values in the config and the sql(ite) database.
        /// </summary>
        public bool SetDefaultValues() {
            if (firstConfigGeneration) {
                enablePlugin = true;

                enableDamageChanges = true;
                enableCriticals = false;
                enableKnockback = false;
                enableMinions = false;

                enableProjectileDebuffs = true;
                enableProjectileSelfBuffs = true;
                enableWeaponDebuffs = true;
                enableWeaponSelfBuffs = true;

                enableTurtle = true;
                turtleMultiplier = 1.0;
                enableThorns = true;
                thornMultiplier = 1.0;

                enableNebula = true;
                nebulaTier3Duration = 1.0;
                nebulaTier2Duration = 3.0;
                nebulaTier1Duration = 5.0;

                enableFrost = true;
                frostDuration = 3.0;

                vortexMultiplier = 1.36;

                enableBuffDebuff = true;
                enableBuffSelfBuff = true;

                knockbackMinimum = -1;

                iframeTime = 0.0;

                lowerDamageVariance = 0;
                upperDamageVariance = 0;

                deathItemTag = "";

                normalDeathMessages = MiscData.normalDeathMessages;
                reflectedDeathMessages = MiscData.reflectedDeathMessages;
                
                Database.InitDefaultTables();

                firstConfigGeneration = false;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Resets all values of the config and sql(ite) database to its default values.
        /// </summary>
        public void ResetConfigValues() {
            firstConfigGeneration = true;
            this.SetDefaultValues();
        }
    }
}
