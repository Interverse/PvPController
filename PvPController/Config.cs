using Newtonsoft.Json;
using PvPController.Utilities;
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

        public double iframeTime;

        public int lowerDamageVariance;
        public int upperDamageVariance;

        public string deathItemTag;
        
        public List<string> normalDeathMessages = new List<string>();
        
        public List<string> reflectedDeathMessages = new List<string>();

        public string Instructions = "Press ctrl+f to find your item faster! (Everything is sorted in IDs)";

        public bool firstConfigGeneration = true;

        public void Write(string path) {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Config Read(string path) {
            if (!File.Exists(path))
                return new Config();
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
        }

        public void WriteDocumentation() {
            StreamWriter sw = new StreamWriter(documentationPath, false);

            sw.WriteLine("Melee Weapons");
            sw.WriteLine("--------------------");

            for(int x = 0; x < PvPController.database.itemInfo.Count; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.melee && PvPController.database.itemInfo[x].damage > 0)
                    sw.WriteLine("{0} ({1}): {2} damage".SFormat(Lang.GetItemName(x).ToString(), x, PvPController.database.itemInfo[x].damage));
            }

            sw.WriteLine("");
            sw.WriteLine("Ranged Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < PvPController.database.itemInfo.Count; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.ranged && PvPController.database.itemInfo[x].damage > 0)
                    sw.WriteLine("{0} ({1}): {2} damage".SFormat(Lang.GetItemName(x).ToString(), x, PvPController.database.itemInfo[x].damage));
            }

            sw.WriteLine("");
            sw.WriteLine("Magic Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < PvPController.database.itemInfo.Count; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.magic && PvPController.database.itemInfo[x].damage > 0)
                    sw.WriteLine("{0} ({1}): {2} damage".SFormat(Lang.GetItemName(x).ToString(), x, PvPController.database.itemInfo[x].damage));
            }

            sw.WriteLine("");
            sw.WriteLine("Throwing Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < PvPController.database.itemInfo.Count; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.thrown && PvPController.database.itemInfo[x].damage > 0)
                    sw.WriteLine("{0} ({1}): {2} damage".SFormat(Lang.GetItemName(x).Value, x, PvPController.database.itemInfo[x].damage));
            }

            sw.WriteLine("");
            sw.WriteLine("Changed Projectile Damage Changes");
            sw.WriteLine("--------------------");

            for (int x = 0; x < PvPController.database.projectileInfo.Count; x++) {
                if (PvPController.database.projectileInfo[x].damage > 0)
                    sw.WriteLine("{0} ({1}): {2} damage".SFormat(Lang.GetProjectileName(x).Value, x, PvPController.database.projectileInfo[x].damage));
            }

            sw.WriteLine("");
            sw.WriteLine("Projectile Debuffs");
            sw.WriteLine("--------------------");

            for (int x = 0; x < PvPController.database.projectileInfo.Count; x++) {
                if (PvPController.database.projectileInfo[x].debuff.buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts the {2} debuff for {3} seconds".SFormat(Lang.GetProjectileName(x).Value, x, Lang.GetBuffName(PvPController.database.projectileInfo[x].debuff.buffid), PvPController.database.projectileInfo[x].debuff.buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Projectile Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < PvPController.database.projectileInfo.Count; x++) {
                if (PvPController.database.projectileInfo[x].selfBuff.buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetProjectileName(x).Value, x, Lang.GetBuffName(PvPController.database.projectileInfo[x].selfBuff.buffid), PvPController.database.projectileInfo[x].selfBuff.buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Weapon Debuffs");
            sw.WriteLine("--------------------");

            for (int x = 0; x < PvPController.database.itemInfo.Count; x++) {
                if (PvPController.database.itemInfo[x].debuff.buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} debuff for {3} seconds".SFormat(Lang.GetItemName(x).Value, x, Lang.GetBuffName(PvPController.database.itemInfo[x].debuff.buffid), PvPController.database.itemInfo[x].debuff.buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Weapon Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < PvPController.database.itemInfo.Count; x++) {
                if (PvPController.database.itemInfo[x].selfBuff.buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetItemName(x).Value, x, Lang.GetBuffName(PvPController.database.itemInfo[x].selfBuff.buffid), PvPController.database.itemInfo[x].selfBuff.buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Buff Debuff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < PvPController.database.buffInfo.Count; x++) {
                if (PvPController.database.buffInfo[x].debuff.buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} debuff for {3} seconds".SFormat(Lang.GetBuffName(x), x, Lang.GetBuffName(PvPController.database.buffInfo[x].debuff.buffid), PvPController.database.itemInfo[x].debuff.buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Buff Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < PvPController.database.buffInfo.Count; x++) {
                if (PvPController.database.buffInfo[x].selfBuff.buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetBuffName(x), x, Lang.GetBuffName(PvPController.database.buffInfo[x].selfBuff.buffid), PvPController.database.buffInfo[x].selfBuff.buffDuration / 60));
            }

            sw.Close();
        }

        public void SetDefaultValues() {
            if (firstConfigGeneration) {
                enablePlugin = true;

                enableDamageChanges = true;

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

                iframeTime = 0.0;

                lowerDamageVariance = 0;
                upperDamageVariance = 0;

                deathItemTag = "";

                normalDeathMessages = MiscData.normalDeathMessages;
                reflectedDeathMessages = MiscData.reflectedDeathMessages;

                PvPController.database.InitDefaultTables();

                firstConfigGeneration = false;

                PvPController.database.LoadDatabase();
            }
        }

        public void ResetConfigValues() {
            firstConfigGeneration = true;
            this.SetDefaultValues();
        }
    }
}
