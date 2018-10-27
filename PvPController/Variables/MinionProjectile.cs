namespace PvPController.Variables {
    public class MinionProjectile {
        public int Projectile;
        public double FireRate;
        public double Radius;
        public double Velocity;
        public int OffsetX;
        public int OffsetY;

        public MinionProjectile(int projectile, double fireRate, double radius, double velocity, int offsetX = 0, int offsetY = 0) {
            Projectile = projectile;
            FireRate = fireRate;
            Radius = radius;
            Velocity = velocity;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }
    }
}
