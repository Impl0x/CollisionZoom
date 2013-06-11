using System;
using Point = CollisionZoom.Vector;

namespace CollisionZoom
{
    /// <summary>
    /// Represents a circular physics object in the world 
    /// which has no behavior other than moving and being
    /// influenced by other objects.
    /// </summary>
    class Projectile
    {
        /// <summary>
        /// The mass of a projectile.
        /// </summary>
        public readonly double Mass;

        /// <summary>
        /// The circular radius of a projectile.
        /// </summary>
        public readonly double Radius;

        /// <summary>
        /// The position of a projectile's center.
        /// </summary>
        public Point Position;

        public Attachment Attachment
        {
            get
            {
                return new PhysicsAttachment(this);
            }
        }

        /// <summary>
        /// The velocity of a projectile.
        /// </summary>
        public Vector Velocity;

        public static double Drag = 0.01;

        public Projectile(double mass, double radius, Point pos, Vector vel)
        {
            this.Mass = mass;
            this.Radius = radius;
            this.Position = pos;
            this.Velocity = vel;
        }

        public Projectile(double mass, double radius)
            : this(mass, radius, Point.Zero, Vector.Zero)
        { }

        /// <summary>
        /// Updates an individual projectile by a given time interval.
        /// </summary>
        public static void Update(Projectile A, double dT)
        {
            double speed = A.Velocity.Length;
            double dragCoefficient = Drag * A.Radius / A.Mass;
            double drag = Drag * A.Radius * A.Velocity.Length * dT / A.Mass;
            A.Position += A.Velocity * dT;
            if(speed > 0.001) A.Velocity *= (Math.Sqrt(1.0 + 4.0 * speed * dragCoefficient * dT) - 1.0) / (2.0 * speed * dragCoefficient * dT);
        }

        /// <summary>
        /// Checks to see if two projectiles are colliding with each other,
        /// and handles the collision if necessary.
        /// </summary>
        public static void TryCollide(Projectile A, Projectile B)
        {
            Vector difference = B.Position - A.Position;
            double distance = difference.Length;
            double penetration = A.Radius + B.Radius - distance;
            if (penetration > 0.0)
            {
                Vector normal = difference * (1.0 / distance);
                double ima = 1.0 / A.Mass;
                double imb = 1.0 / B.Mass;
                Vector separation = normal * (penetration / (ima + imb));
                A.Position -= separation * ima;
                B.Position += separation * imb;
                double impact = Vector.Dot(A.Velocity - B.Velocity, normal);
                if (impact > 0.0)
                {
                    double cor = 1.0;
                    double j = (1.0 + cor) * impact / (ima + imb);
                    Vector impulse = normal * j;
                    A.Velocity -= impulse * ima;
                    B.Velocity += impulse * imb;
                }
            }
        }
    }
}
