using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.Utilities {
    public class MiscUtils {
        public static string SanitizeString(string s) {
            if (s.Contains("'")) {
                string[] temp = s.Split('\'');
                s = temp[0] + "''" + temp[1];
            }
            return s;
        }

        public static string LineBreaks(int amount) {
            StringBuilder sb = new StringBuilder();
            for (int x = 0; x < amount; x++) {
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

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
