using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using PvPController.Utilities;
using Terraria;
using TShockAPI;

namespace PvPController {
    public class Config {
        public static string ConfigPath = Path.Combine(TShock.SavePath, "pvpcontroller.json");
        public static string LogPath = Path.Combine(TShock.SavePath, "pvplog.txt");

        public bool EnablePlugin { get; set; }

        public bool EnableDamageChanges { get; set; }
        public bool EnableCriticals { get; set; }
        public bool EnableKnockback { get; set; }
        public bool EnableMinions { get; set; }

        public bool EnableProjectileDebuffs { get; set; }
        public bool EnableProjectileSelfBuffs { get; set; }
        public bool EnableWeaponDebuffs { get; set; }
        public bool EnableWeaponSelfBuffs { get; set; }
        public bool HealthBasedBuffDuration { get; set; }
        
        public int ParryTime { get; set; }

        public bool EnableTurtle { get; set; }
        public double TurtleMultiplier { get; set; }
        public bool EnableThorns { get; set; }
        public double ThornMultiplier { get; set; }

        public bool EnableNebula { get; set; }
        public double NebulaTier3Duration { get; set; }
        public double NebulaTier2Duration { get; set; }
        public double NebulaTier1Duration { get; set; }

        public bool EnableFrost { get; set; }
        public double FrostDuration { get; set; }

        public double VortexMultiplier { get; set; }

        public bool EnableBuffDebuff { get; set; }
        public bool EnableBuffSelfBuff { get; set; }

        public double KnockbackMinimum { get; set; }

        public double IframeTime { get; set; }

        public int LowerDamageVariance { get; set; }
        public int UpperDamageVariance { get; set; }

        public double LowerMagicDamagePercentage { get; set; }
        public double UpperMagicDamagePercentage { get; set; }

        public string DeathItemTag { get; set; }

        public List<string> NormalDeathMessages { get; set; } = new List<string>();

        public List<string> ReflectedDeathMessages { get; set; } = new List<string>();

        public bool FirstConfigGeneration { get; set; } = true;

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
                HealthBasedBuffDuration = false;

                ParryTime = 1000;

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

                LowerMagicDamagePercentage = 1;
                UpperMagicDamagePercentage = 1;

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

        /// <summary>
        /// Parses all item, projectile, and buff changes and puts it into a .txt file in the tshock folder.
        /// </summary>
        public void LogChange(string log) {
            StreamWriter sw = new StreamWriter(LogPath, true);
            sw.WriteLine(log);
            sw.Close();
        }
    }
}
