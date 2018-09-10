using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPController.Utilities {
    public class MinionUtils {
        //Minion IDs
        public static Dictionary<int, int> minionItem = new Dictionary<int, int> {
            { 373, 2364 }, //Hornet Staff
            { 375, 2365 }, //Imp Staff
            { 377, 2366 }, //Queen Spider Staff
            { 379, 2366 }, //Queen Spider Staff (Baby Spider)
            { 387, 2535 }, //Optic Staff (Retanimini)
            { 388, 2535 }, //Optic Staff (Spazmamini)
            { 390, 2551 }, //Spider Staff
            { 391, 2551 }, //Spider Staff
            { 392, 2551 }, //Spider Staff
            { 393, 2584 }, //Pirate Staff
            { 394, 2584 }, //Pirate Staff
            { 395, 2584 }, //Pirate Staff
            { 191, 1157 }, //Pygmy Staff
            { 192, 1157 }, //Pygmy Staff
            { 193, 1157 }, //Pygmy Staff
            { 194, 1157 }, //Pygmy Staff
            { 423, 2749 }, //Xeno Staff
            { 317, 1802 }, //Raven Staff
            { 407, 2621 }, //Tempest Staff
            { 408, 2621 }, //Tempest Staff
            { 533, 3249 }, //Deadly Sphere Staff
            { 625, 3531 }, //Stardust Dragon Staff
            { 626, 3531 }, //Stardust Dragon Staff
            { 627, 3531 }, //Stardust Dragon Staff
            { 628, 3531 }, //Stardust Dragon Staff
            { 613, 3474 }, //Stardust Cell Staff
            { 614, 3474 }, //Stardust Cell Staff (Mini)
            { 308, 1572 }, //Staff of the Frost Hydra
            { 309, 1572 }, //Staff of the Frost Hydra
            { 641, 3569 }, //Lunar Portal Staff
            { 642, 3569 }, //Lunar Portal Staff
            { 643, 3571 }, //Rainbow Crystal Staff
            { 644, 3571 }, //Rainbow Crystal Staff
            { 663, 3818 }, //Flameburst Rod
			{ 664, 3818 }, //Flameburst Rod
            { 665, 3819 }, //Flameburst Cane
            { 666, 3819 }, //Flameburst Cane
            { 667, 3820 }, //Flameburst Staff
            { 668, 3820 }, //Flameburst Staff
            { 677, 3824 }, //Ballista Rod
            { 678, 3825 }, //Ballista Cane
            { 679, 3826 }, //Ballista Staff
            { 688, 3829 }, //Lightning Aura Rod
            { 689, 3830 }, //Lightning Aura Cane
            { 690, 3831 }, //Lightning Aura Staff
            { 691, 3823 }, //Explosive Trap Rod
            { 692, 3824 }, //Explosive Trap Cane
            { 693, 3825 }, //Explosive Trap Staff
            { 694, 3823 }, //Explosive Trap Rod
            { 695, 3824 }, //Explosive Trap Cane
            { 696, 3825 }, //Explosive Trap Staff
        };

        //Minion Data
        public static Dictionary<int, MinionProjectile> minionStats = new Dictionary<int, MinionProjectile> {
            { 191, new MinionProjectile(195, 0.5, 40, 12) }, //Pygmy
            { 192, new MinionProjectile(195, 0.5, 40, 12) }, //Pygmy
            { 193, new MinionProjectile(195, 0.5, 40, 12) }, //Pygmy
            { 194, new MinionProjectile(195, 0.5, 40, 12) }, //Pygmy
            { 407, new MinionProjectile(408, 0.6, 50, 14, 0 ,9) }, //Tempest
            { 377, new MinionProjectile(378, 1.0, 30, 9, 30, 10) }, //Spider
            { 308, new MinionProjectile(309, 1.0, 50, 6, 40, 45) }, //Hydra
            { 641, new MinionProjectile(642, 2.0, 30, 0) }, //Lunar Portal
            { 643, new MinionProjectile(644, 1.5, 30, 0) }, //Rainbow
            { 373, new MinionProjectile(374, 0.8, 50, 10, 0, 3) }, //Hornet
            { 375, new MinionProjectile(376, 2.5, 50, 11) }, //Imp
            { 387, new MinionProjectile(389, 0.8, 50, 8) }, //Retanimini
            { 388, new MinionProjectile(389, 0.8, 50, 8) }, //Spazmamini
            { 623, new MinionProjectile(624, 1.2, 6, 0) }, //Stardust Guardian
            { 663, new MinionProjectile(664, 1.7, 50, 12, 0, 18) }, //Flameburst Rod
            { 665, new MinionProjectile(666, 1.7, 50, 15, 0, 18) }, //Flameburst Cane
            { 667, new MinionProjectile(668, 1.7, 50, 18, 0, 18) }, //Flameburst Staff
            { 677, new MinionProjectile(680, 3.5, 50, 16, 0, 16) }, //Ballista Rod
            { 678, new MinionProjectile(680, 3.5, 50, 16, 0, 16) }, //Ballista Cane
            { 679, new MinionProjectile(680, 3.5, 50, 16, 0, 16) }, //Ballista Staff
            { 691, new MinionProjectile(694, 1.0, 5, 0, 7, -24) }, //Explosive Trap Rod
            { 692, new MinionProjectile(695, 1.0, 5, 0, 7, -24) }, //Explosive Trap Cane
            { 693, new MinionProjectile(696, 1.0, 5, 0, 7, -24) },  //Explosive Trap Staff
        };

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
}
