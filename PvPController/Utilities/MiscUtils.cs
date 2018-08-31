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
    }
}
