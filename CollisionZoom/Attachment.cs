using System;
using Point = CollisionZoom.Vector;

namespace CollisionZoom
{
    abstract class Attachment
    {
        public abstract Point Position { get; }
        public abstract Vector Velocity { get; }

        public static CompositionAttachment operator +(Attachment a, Attachment b)
        {
            return (new CompositionAttachment(a, b));
        }

        public static FixedAttachment Fixed(Point pos)
        {
            return new FixedAttachment(pos);
        }

        public static FunctionAttachment Function(World world, Func<double, Point> func)
        {
            return new FunctionAttachment(world, func);
        }

        /// <summary>
        /// Creates a function attachment which moves along 
        /// a circle with has a given radius and period.
        /// </summary>
        /// <param name="radius">
        /// The radius of the circle.
        /// </param>
        /// <param name="period">
        /// The period of one revolution in seconds.
        /// </param>
        public static FunctionAttachment Circular(double radius, double period, World world)
        {
            Func<double, Point> func = delegate(double time)
            {
                double p = 2.0 * Math.PI / period;
                double x = radius * Math.Cos(p * time);
                double y = radius * Math.Sin(p * time);
                return new Point(x, y);
            };
            return new FunctionAttachment(world, func);
        }

        /// <summary>
        /// Creates a function attachment which moves along
        /// an axis-aligned ellipse with a given width, height, and period.
        /// </summary>
        /// <param name="width">
        /// The width of the ellipse.
        /// </param>
        /// <param name="height">
        /// The height of the ellipse.
        /// </param>
        /// <param name="period">
        /// The period of one revolution in seconds.
        /// </param>
        public static FunctionAttachment Elliptical(double width, double height, double period, World world)
        {
            Func<double, Point> func = delegate(double time)
            {
                double p = 2.0 * Math.PI / period;
                double x = width * Math.Cos(p * time);
                double y = height * Math.Sin(p * time);
                return new Point(x, y);
            };
            return new FunctionAttachment(world, func);
        }
    }


    /// <summary>
    /// An attachment which has a fixed position.
    /// </summary>
    class FixedAttachment : Attachment
    {
        private Point pos;
        public override Point Position 
        { 
            get
            {
                return pos;
            }
        }
        public override Point Velocity
        {
            get { return Vector.Zero; }
        }

        public FixedAttachment(Point pos)
        {
            this.pos = pos;
        }
    }

    /// <summary>
    /// An attachment which bases its position on a function of time.
    /// </summary>
    class FunctionAttachment : Attachment
    {
        public Func<double, Point> Function;
        World World;
        Point pos;

        public override Point Position
        {
            get 
            {
                return Function(this.World.Time);
            }
        }
        public override Point Velocity
        {
            get 
            {
                double epsilon = 0.0001;
                return ((this.Function(this.World.Time + epsilon) - this.Function(this.World.Time)) * (1.0 / epsilon));
            }
        }

        public FunctionAttachment(World world, Func<double, Point> func)
        {
            this.World = world;
            this.Function = func;
        }
    }
        
    /// <summary>
    /// An attachment which bases its position on a physics object.
    /// </summary>
    class PhysicsAttachment : Attachment
    {
        public override Point Position
        {
            get
            {
                return this.Source.Position;
            }
        }
        public override Vector Velocity
        {
            get 
            { 
                return this.Source.Velocity; 
            }
        }

        public Projectile Source;

        public PhysicsAttachment(Projectile source)
        {
            this.Source = source;
        }
    }

    /// <summary>
    /// Composes two attachment sources.
    /// </summary>
    class CompositionAttachment : Attachment
    {
        public override Point Position
        {
            get
            {
                return (SourceA.Position + SourceB.Position);
            }
        }

        public override Point Velocity
        {
            get
            {
                return (SourceA.Velocity + SourceB.Velocity);
            }
        }

        public Attachment SourceA;
        public Attachment SourceB;

        public CompositionAttachment(Attachment a, Attachment b)
        {
            this.SourceA = a;
            this.SourceB = b;
        }
    }
}
