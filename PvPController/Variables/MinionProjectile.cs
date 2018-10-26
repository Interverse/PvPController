using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.Variables {
    public class MinionProjectile {
        public int projectile;
        public double fireRate;
        public double radius;
        public double velocity;
        public int offsetX;
        public int offsetY;

        public MinionProjectile(int projectile, double fireRate, double radius, double velocity, int offsetX = 0, int offsetY = 0) {
            this.projectile = projectile;
            this.fireRate = fireRate;
            this.radius = radius;
            this.velocity = velocity;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }
    }
}
