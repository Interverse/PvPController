using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.Variables {
    public class BuffDuration {
        public int buffid { get; set; }
        public int buffDuration { get; set; }

        public BuffDuration(int buffid, int buffDuration) {
            this.buffid = buffid;
            this.buffDuration = buffDuration;
        }
    }
}
