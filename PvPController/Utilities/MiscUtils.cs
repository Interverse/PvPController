using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Terraria;

namespace PvPController.Utilities {
    public static class MiscUtils {
        /// <summary>
        /// Attempts to sanitize any ' characters in a string to '' for sql queries.
        /// </summary>
        public static string SanitizeString(this string s) {
            if (!s.Contains("'")) return s;

            string[] temp = s.Split('\'');
            s = temp[0];

            for(int x = 1; x < temp.Length; x++) {
                s += "''" + temp[x];
            }
            return s;
        }

        /// <summary>
        /// Converts a string that is friendly with sql inputs.
        /// </summary>
        public static string SqlString(this string s) => "'" + SanitizeString(s) + "'";

        /// <summary>
        /// Generates a string with a specified amount of line breaks.
        /// </summary>
        /// <param Name="amount">The amount of line breaks.</param>
        public static string LineBreaks(int amount) {
            StringBuilder sb = new StringBuilder();
            for (int x = 0; x < amount; x++) {
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Separates a string into lines after a specified amount of characters.
        /// </summary>
        public static string SeparateToLines(this string s, int charPerLine = 45, string breakSpecifier = "") {
            StringBuilder sb = new StringBuilder();
            int count = 0;

            foreach (char ch in s) {
                if (count != 0 && count >= charPerLine) {
                    if (breakSpecifier != "" && ch.ToString() == breakSpecifier) {
                        sb.Append("\r\n");
                        count = 0;
                    }
                }
                sb.Append(ch);
                count++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a string into a given Type.
        /// </summary>
        /// <returns>Returns false if the string is incompatible with the given Type</returns>
        public static bool TryConvertStringToType(Type referenceType, string input, out object obj) {
            try {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(referenceType);
                obj = typeConverter.ConvertFromString(input.SanitizeString());
                return true;
            } catch {
                obj = default(object);
                return false;
            }
        }

        /// <summary>
        /// Gets a list of projectiles based off the given Name query.
        /// </summary>
        public static List<int> GetProjectileByName(this TShockAPI.Utils util, string name) {
            string nameLower = name.ToLower();
            var found = new List<int>();
            for (int i = 1; i < Main.maxProjectileTypes; i++) {
                string projectileName = Lang.GetProjectileName(i).ToString();
                if (!String.IsNullOrWhiteSpace(projectileName) && projectileName.ToLower() == nameLower)
                    return new List<int> { i };
                if (!String.IsNullOrWhiteSpace(projectileName) && projectileName.ToLower().StartsWith(nameLower))
                    found.Add(i);
            }
            return found;
        }

        /// <summary>
        /// Gets the id of an item, projectile, or buff.
        /// </summary>
        public static List<int> GetIdFromInput(this TShockAPI.Utils util, string input, string name) {
            if (input == DbConsts.ItemTable) {
                var itemsFound = util.GetItemByName(name);
                return itemsFound.Select(c => c.netID).ToList();
            } else if (input == DbConsts.ProjectileTable) {
                return util.GetProjectileByName(name);
            } else if (input == DbConsts.BuffTable) {
                return util.GetBuffByName(name);
            }

            return default(List<int>);
        }

        /// <summary>
        /// Gets the Name of a item, projectile, or buff.
        /// </summary>
        public static string GetNameFromInput(string input, int id) {
            if (input == DbConsts.ItemTable) {
                return Lang.GetItemName(id).ToString();
            } else if (input == DbConsts.ProjectileTable) {
                return Lang.GetProjectileName(id).ToString();
            } else if (input == DbConsts.BuffTable) {
                return Lang.GetBuffName(id);
            }

            return default(string);
        }
        
        /// <summary>
        /// Converts a string to the reference value type,
        /// and sets the string to the given reference value.
        /// </summary>
        public static bool SetValueWithString<T>(ref T value, string str) {
            try {
                value = (T)Convert.ChangeType(str, value.GetType());
                return true;
            } catch {
                return false;
            }
        }
    }
}
