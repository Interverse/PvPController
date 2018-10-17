﻿using Newtonsoft.Json;
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

            for(int x = 0; x < Database.itemInfo.Count; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.melee && Database.itemInfo[x].GetData<int>("Damage") > 0)
                    sw.WriteLine("{0} ({1}): {2}{3}{4}"
                        .SFormat(Lang.GetItemName(x).Value,
                        x,
                        Database.itemInfo[x].GetData<int>("Damage") + " damage",
                        Database.itemInfo[x].GetData<int>("Shoot") > 0
                            ? ", shoots {0}{1}".SFormat(Lang.GetProjectileName(Database.itemInfo[x].GetData<int>("Shoot")),
                                Database.itemInfo[x].GetData<int>("ShootSpeed") > 0
                                    ? " with {0} shootspeed".SFormat(Database.itemInfo[x].GetData<int>("ShootSpeed"))
                                    : "")
                            : "",
                        ", knockback: {0}".SFormat(Database.itemInfo[x].GetData<float>("Knockback"))));
            }

            sw.WriteLine("");
            sw.WriteLine("Ranged Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Database.itemInfo.Count; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.ranged && Database.itemInfo[x].GetData<int>("Damage") > 0)
                    sw.WriteLine("{0} ({1}): {2}{3}{4}"
                        .SFormat(Lang.GetItemName(x).Value,
                        x,
                        Database.itemInfo[x].GetData<int>("Damage") + " damage",
                        Database.itemInfo[x].GetData<int>("Shoot") > 0
                            ? ", shoots {0}{1}".SFormat(Lang.GetProjectileName(Database.itemInfo[x].GetData<int>("Shoot")),
                                Database.itemInfo[x].GetData<int>("ShootSpeed") > 0
                                    ? " with {0} shootspeed".SFormat(Database.itemInfo[x].GetData<int>("ShootSpeed"))
                                    : "")
                            : "",
                        ", knockback: {0}".SFormat(Database.itemInfo[x].GetData<float>("Knockback"))));
            }

            sw.WriteLine("");
            sw.WriteLine("Magic Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Database.itemInfo.Count; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.magic && Database.itemInfo[x].GetData<int>("Damage") > 0)
                    sw.WriteLine("{0} ({1}): {2}{3}{4}"
                        .SFormat(Lang.GetItemName(x).Value,
                        x,
                        Database.itemInfo[x].GetData<int>("Damage") + " damage",
                        Database.itemInfo[x].GetData<int>("Shoot") > 0
                            ? ", shoots {0}{1}".SFormat(Lang.GetProjectileName(Database.itemInfo[x].GetData<int>("Shoot")),
                                Database.itemInfo[x].GetData<int>("ShootSpeed") > 0
                                    ? " with {0} shootspeed".SFormat(Database.itemInfo[x].GetData<int>("ShootSpeed"))
                                    : "")
                            : "",
                        ", knockback: {0}".SFormat(Database.itemInfo[x].GetData<float>("Knockback"))));
            }

            sw.WriteLine("");
            sw.WriteLine("Throwing Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Database.itemInfo.Count; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.thrown && Database.itemInfo[x].GetData<int>("Damage") > 0)
                    sw.WriteLine("{0} ({1}): {2}{3}{4}"
                        .SFormat(Lang.GetItemName(x).Value,
                        x,
                        Database.itemInfo[x].GetData<int>("Damage") + " damage",
                        Database.itemInfo[x].GetData<int>("Shoot") > 0
                            ? ", shoots {0}{1}".SFormat(Lang.GetProjectileName(Database.itemInfo[x].GetData<int>("Shoot")),
                                Database.itemInfo[x].GetData<int>("ShootSpeed") > 0
                                    ? " with {0} shootspeed".SFormat(Database.itemInfo[x].GetData<int>("ShootSpeed"))
                                    : "")
                            : "",
                        ", knockback: {0}".SFormat(Database.itemInfo[x].GetData<float>("Knockback"))));
            }

            sw.WriteLine("");
            sw.WriteLine("Projectile Damage Changes");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Database.projectileInfo.Count; x++) {
                if (Database.projectileInfo[x].GetData<int>("Damage") > 0)
                    sw.WriteLine("{0} ({1}): {2} damage".SFormat(Lang.GetProjectileName(x).Value, x, Database.projectileInfo[x].GetData<int>("Damage")));
            }

            sw.WriteLine("");
            sw.WriteLine("Projectile Debuffs");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Database.projectileInfo.Count; x++) {
                if (Database.projectileInfo[x].GetData<BuffDuration>("Debuff").buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts the {2} debuff for {3} seconds".SFormat(Lang.GetProjectileName(x).Value, x, Lang.GetBuffName(Database.projectileInfo[x].GetData<BuffDuration>("Debuff").buffid), Database.projectileInfo[x].GetData<BuffDuration>("Debuff").buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Projectile Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Database.projectileInfo.Count; x++) {
                if (Database.projectileInfo[x].GetData<BuffDuration>("SelfBuff").buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetProjectileName(x).Value, x, Lang.GetBuffName(Database.projectileInfo[x].GetData<BuffDuration>("SelfBuff").buffid), Database.projectileInfo[x].GetData<BuffDuration>("SelfBuff").buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Weapon Debuffs");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Database.itemInfo.Count; x++) {
                if (Database.itemInfo[x].GetData<BuffDuration>("Debuff").buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} debuff for {3} seconds".SFormat(Lang.GetItemName(x).Value, x, Lang.GetBuffName(Database.itemInfo[x].GetData<BuffDuration>("Debuff").buffid), Database.itemInfo[x].GetData<BuffDuration>("Debuff").buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Weapon Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Database.itemInfo.Count; x++) {
                if (Database.itemInfo[x].GetData<BuffDuration>("SelfBuff").buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetItemName(x).Value, x, Lang.GetBuffName(Database.itemInfo[x].GetData<BuffDuration>("SelfBuff").buffid), Database.itemInfo[x].GetData<BuffDuration>("SelfBuff").buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Buff Debuff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Database.buffInfo.Count; x++) {
                if (Database.buffInfo[x].GetData<BuffDuration>("Debuff").buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} debuff for {3} seconds".SFormat(Lang.GetBuffName(x), x, Lang.GetBuffName(Database.buffInfo[x].GetData<BuffDuration>("Debuff").buffid), Database.itemInfo[x].GetData<BuffDuration>("Debuff").buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Buff Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < Database.buffInfo.Count; x++) {
                if (Database.buffInfo[x].GetData<BuffDuration>("SelfBuff").buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetBuffName(x), x, Lang.GetBuffName(Database.buffInfo[x].GetData<BuffDuration>("SelfBuff").buffid), Database.buffInfo[x].GetData<BuffDuration>("SelfBuff").buffDuration / 60));
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

                Database.LoadDatabase();

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
