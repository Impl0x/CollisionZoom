using System;
using System.Collections.Generic;

using Point = CollisionZoom.Vector;

namespace CollisionZoom
{
    class World
    {
        /// <summary>
        /// The boundaries within which all world entities are contained.
        /// </summary>
        public Rectangle Boundary;
        /// <summary>
        /// List of all the projectiles in the world.
        /// </summary>
        public List<Projectile> Projectiles;
        /// <summary>
        /// Here be gravities.
        /// </summary>
        public List<Gravity> Gravities;
        /// <summary>
        /// The total elapsed time in seconds.
        /// </summary>
        public double Time;
        
        public World(double width, double height)
        {
            this.Boundary = new Rectangle(new Point(width / -2, height / 2), new Point(width / 2, height / -2)); 
            this.Projectiles = new List<Projectile>();
            this.Gravities = new List<Gravity>();
        }

        /// <summary>
        /// Searches the world for a projectile which contains
        /// a given point, and returns that projectile if one exists.
        /// </summary>
        public Projectile Pick(Point p)
        {
            foreach (Projectile proj in this.Projectiles)
            {
                if ((p - proj.Position).Length <= proj.Radius)
                    return proj;
            }
            return null;
        }

        /// <summary>
        /// Adds a new projectile into the world.
        /// </summary>
        public void SpawnProjectile(Projectile p)
        {
            this.Projectiles.Add(p);
        }

        /// <summary>
        /// Adds a given gravity object to the world.
        /// </summary>
        /// <param name="g"></param>
        public void SpawnGravity(Gravity g)
        {
            this.Gravities.Add(g);
        }

        /// <summary>
        /// Updates the world by a given unit of time.
        /// </summary>
        public void Update(double updateTime)
        {
            this.Time += updateTime;
            // Update individual projectiles
            for (int i = 0; i < this.Projectiles.Count; i++)
            {
                Projectile a = this.Projectiles[i];
                foreach(Gravity grav in Gravities)
                {
                    Vector dif = grav.Attachment.Position - a.Position;
                    double length = dif.Length;
                    a.Velocity += dif * ((grav.G * updateTime) / (length * length));
                }
                Projectile.Update(a, updateTime);
            }

            // Perform collision detection on all the projectiles.
            for (int i = 0; i < this.Projectiles.Count; i++)
            {
                for (int j = i + 1; j < this.Projectiles.Count; j++)
                {
                    Projectile a = this.Projectiles[i];
                    Projectile b = this.Projectiles[j];
                    Projectile.TryCollide(a, b);
                }
            }
        }
    }
}
