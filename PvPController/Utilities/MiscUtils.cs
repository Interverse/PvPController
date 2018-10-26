using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace PvPController.Variables {
    public static class MiscUtils {
        /// <summary>
        /// Attempts to sanitize any ' characters in a string to '' for sql(ite) queries.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string SanitizeString(this string s) {
            if (s.Contains("'")) {
                string[] temp = s.Split('\'');
                s = temp[0];

                for(int x = 1; x < temp.Length; x++) {
                    s += "''" + temp[x];
                }
            }
            return s;
        }

        public static string SQLString(this string s) {
            return "'" + SanitizeString(s) + "'";
        }

        /// <summary>
        /// Generates a string with a specified amount of line breaks.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
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
        /// <param name="s"></param>
        /// <returns></returns>
        public static string SeparateToLines(this string s, int charPerLine = 45, string breakSpecifier = "") {
            StringBuilder sb = new StringBuilder();
            int count = 0;

            for (int x = 0; x < s.Length; x++) {
                if (count != 0 && count >= charPerLine) {
                    if (breakSpecifier != "" && s[x].ToString() == breakSpecifier) {
                        sb.Append("\r\n");
                        count = 0;
                    }
                }
                sb.Append(s[x]);
                count++;
            }

            return sb.ToString();
        }

        public static bool TryConvertStringToType(Type referenceType, string input, out object obj) {
            try {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(referenceType);
                obj = typeConverter.ConvertFromString(MiscUtils.SanitizeString(input));
                return true;
            } catch {
                obj = default(object);
                return false;
            }
        }

        public static List<int> GetProjectileByName(this TShockAPI.Utils util, string name) {
            string nameLower = name.ToLower();
            string projectileName;
            var found = new List<int>();
            for (int i = 1; i < Main.maxProjectileTypes; i++) {
                projectileName = Lang.GetProjectileName(i).ToString();
                if (!String.IsNullOrWhiteSpace(projectileName) && projectileName.ToLower() == nameLower)
                    return new List<int> { i };
                if (!String.IsNullOrWhiteSpace(projectileName) && projectileName.ToLower().StartsWith(nameLower))
                    found.Add(i);
            }
            return found;
        }

        public static List<int> GetIDFromInput(this TShockAPI.Utils util, string input, string name) {
            if (input == DBConsts.ItemTable) {
                var itemsFound = util.GetItemByName(name);
                return itemsFound.Select(c => c.netID).ToList();
            } else if (input == DBConsts.ProjectileTable) {
                return util.GetProjectileByName(name);
            } else if (input == DBConsts.BuffTable) {
                return util.GetBuffByName(name);
            }

            return default(List<int>);
        }

        public static string GetNameFromInput(string input, int id) {
            if (input == DBConsts.ItemTable) {
                return Lang.GetItemName(id).ToString();
            } else if (input == DBConsts.ProjectileTable) {
                return Lang.GetProjectileName(id).ToString();
            } else if (input == DBConsts.BuffTable) {
                return Lang.GetBuffName(id);
            }

            return default(string);
        }

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
