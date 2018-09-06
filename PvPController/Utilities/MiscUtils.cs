using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.Utilities {
    public class MiscUtils {
        /// <summary>
        /// Attempts to sanitize any ' characters in a string to '' for sql(ite) queries.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string SanitizeString(string s) {
            if (s.Contains("'")) {
                string[] temp = s.Split('\'');
                s = temp[0] + "''" + temp[1];
            }
            return s;
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
        /// Separates a string into lines after each 45 characters.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string SeparateToLines(string s) {
            StringBuilder sb = new StringBuilder();

            for (int x = 0; x < s.Length; x++) {
                if (x != 0 && x % 45 == 0) sb.Append("\r\n");
                sb.Append(s[x]);
            }

            return sb.ToString();
        }
    }
}
