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
        public Dictionary<int, ItemInfo> itemInfo = new Dictionary<int, ItemInfo>(Main.maxItemTypes);
        public Dictionary<int, ItemInfo> projectileInfo = new Dictionary<int, ItemInfo>(Main.maxProjectileTypes);
        public Dictionary<int, ItemInfo> buffInfo = new Dictionary<int, ItemInfo>(Main.maxBuffTypes);

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

            for(int x = 0; x < itemInfo.Count; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.melee && itemInfo[x].damage > 0)
                    sw.WriteLine("{0} ({1}): {2} damage".SFormat(Lang.GetItemName(x).ToString(), x, itemInfo[x].damage));
            }

            sw.WriteLine("");
            sw.WriteLine("Ranged Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < itemInfo.Count; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.ranged && itemInfo[x].damage > 0)
                    sw.WriteLine("{0} ({1}): {2} damage".SFormat(Lang.GetItemName(x).ToString(), x, itemInfo[x].damage));
            }

            sw.WriteLine("");
            sw.WriteLine("Magic Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < itemInfo.Count; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.magic && itemInfo[x].damage > 0)
                    sw.WriteLine("{0} ({1}): {2} damage".SFormat(Lang.GetItemName(x).ToString(), x, itemInfo[x].damage));
            }

            sw.WriteLine("");
            sw.WriteLine("Throwing Weapons");
            sw.WriteLine("--------------------");

            for (int x = 0; x < itemInfo.Count; x++) {
                Item item = new Item();
                item.SetDefaults(x);

                if (item.thrown && itemInfo[x].damage > 0)
                    sw.WriteLine("{0} ({1}): {2} damage".SFormat(Lang.GetItemName(x).Value, x, itemInfo[x].damage));
            }

            sw.WriteLine("");
            sw.WriteLine("Changed Projectile Damage Changes");
            sw.WriteLine("--------------------");

            for (int x = 0; x < projectileInfo.Count; x++) {
                if (projectileInfo[x].damage > 0)
                    sw.WriteLine("{0} ({1}): {2} damage".SFormat(Lang.GetProjectileName(x).Value, x, projectileInfo[x].damage));
            }

            sw.WriteLine("");
            sw.WriteLine("Projectile Debuffs");
            sw.WriteLine("--------------------");

            for (int x = 0; x < projectileInfo.Count; x++) {
                if (projectileInfo[x].debuff.buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts the {2} debuff for {3} seconds".SFormat(Lang.GetProjectileName(x).Value, x, Lang.GetBuffName(projectileInfo[x].debuff.buffid), projectileInfo[x].debuff.buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Projectile Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < projectileInfo.Count; x++) {
                if (projectileInfo[x].selfBuff.buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetProjectileName(x).Value, x, Lang.GetBuffName(projectileInfo[x].selfBuff.buffid), projectileInfo[x].selfBuff.buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Weapon Debuffs");
            sw.WriteLine("--------------------");

            for (int x = 0; x < itemInfo.Count; x++) {
                if (itemInfo[x].debuff.buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} debuff for {3} seconds".SFormat(Lang.GetItemName(x).Value, x, Lang.GetBuffName(itemInfo[x].debuff.buffid), itemInfo[x].debuff.buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Weapon Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < itemInfo.Count; x++) {
                if (itemInfo[x].selfBuff.buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetItemName(x).Value, x, Lang.GetBuffName(itemInfo[x].selfBuff.buffid), itemInfo[x].selfBuff.buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Buff Debuff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < buffInfo.Count; x++) {
                if (buffInfo[x].debuff.buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} debuff for {3} seconds".SFormat(Lang.GetBuffName(x), x, Lang.GetBuffName(buffInfo[x].debuff.buffid), itemInfo[x].debuff.buffDuration / 60));
            }

            sw.WriteLine("");
            sw.WriteLine("Buff Self Buff");
            sw.WriteLine("--------------------");

            for (int x = 0; x < buffInfo.Count; x++) {
                if (buffInfo[x].selfBuff.buffid != 0)
                    sw.WriteLine("{0} ({1}) inflicts {2} buff to self for {3} seconds".SFormat(Lang.GetBuffName(x), x, Lang.GetBuffName(buffInfo[x].selfBuff.buffid), buffInfo[x].selfBuff.buffDuration / 60));
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

                for (int x = 0; x < Main.maxItemTypes; x++) {
                    itemInfo[x] = new ItemInfo();
                    Item item = new Item();
                    item.SetDefaults(x);

                    itemInfo[x].damage = item.damage;
                    itemInfo[x].defense = item.defense;
                }

                for (int x = 0; x < Main.maxProjectileTypes; x++) {
                    projectileInfo[x] = new ItemInfo();
                    if (MiscData.accessoryOrArmorProjectiles.ContainsKey(x)) {
                        projectileInfo[x].damage = MiscData.accessoryOrArmorProjectiles[x];
                    }
                    if (MiscData.projectileDebuffs.ContainsKey(x)) {
                        projectileInfo[x].debuff = MiscData.projectileDebuffs[x];
                    }
                }

                for (int x = 0; x < Main.maxBuffTypes; x++) {
                    buffInfo[x] = new ItemInfo();
                    if (MiscData.flaskDebuffs.ContainsKey(x)) {
                        buffInfo[x].debuff = MiscData.flaskDebuffs[x];
                    }
                }

                firstConfigGeneration = false;
            }
        }

        public void ResetConfigValues() {
            firstConfigGeneration = true;
            this.SetDefaultValues();
        }
    }
}
