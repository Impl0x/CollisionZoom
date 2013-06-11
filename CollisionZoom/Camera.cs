using System;

using Point = CollisionZoom.Vector;

namespace CollisionZoom
{
    /// <summary>
    /// A persistent, moveable view.
    /// </summary>
    class Camera : ICloneable
    {
        /// <summary>
        /// The location of the center of the camera's view.
        /// </summary>
        public Point Center;
        /// <summary>
        /// The lateral velocity of the camera.
        /// </summary>
        public Vector Velocity;
        /// <summary>
        /// The zoom level of the camera.
        /// </summary>
        public double Zoom;
        /// <summary>
        /// The speed at which the zoom level is changing per second.
        /// </summary>
        public double ZoomVelocity;
        /// <summary>
        /// Gets the extent of the camera's view.
        /// This is the length from the center of the view to any of the edges.
        /// </summary>
        public double Extent
        {
            get
            {
                return Math.Pow(2.0, -this.Zoom);
            }
        }
        /// <summary>
        /// Gets the view transform for the current state of the camera.
        /// </summary>
        public Transform Transform
        {
            get
            {
                return new Transform(this.Center, new Vector(this.Extent, 0.0), new Vector(0.0, this.Extent));
            }
        }

        /// <summary>
        /// The damping factor for lateral camera movement.
        /// </summary>
        private const double damping = 0.1;
        /// <summary>
        /// The damping factor for zoom movement.
        /// </summary>
        private const double zoomDamping = 0.1;
        /// <summary>
        /// The factor for lateral camera movement.
        /// </summary>
        private const double lateralMovement = 0.7;

        public Camera(Point center, double zoom)
        {
            this.Center = center;
            this.Velocity = Vector.Zero;
            this.Zoom = zoom;
            this.ZoomVelocity = 0.0;
        }
        
        /// <summary>
        /// Implementation of the ICloneable interface's Clone function.
        /// </summary>
        public Object Clone()
        {
            return this;
        }

        private double Smoothstep(double t)
        {
            double x = Math.Min(1.0, Math.Max(0.0, t));
            return x*x*x*(x*(x * 6.0 - 15.0) + 10.0);
        }

        /// <summary>
        /// Update's the camera based on mouse control.
        /// Also handles resetting the camera's orientation.
        /// </summary>
        public void Update(double dT)
        {
            double extent = this.Extent;
            this.Center += this.Velocity * extent * dT;
            this.Velocity *= Math.Pow(damping, dT);
            this.Zoom += this.ZoomVelocity * dT;
            this.ZoomVelocity *= Math.Pow(zoomDamping, dT);
        }

        /// <summary>
        /// Changes the velocity and zoom velocity of the camera
        /// to zoom in or out of a given target point.
        /// </summary>
        public void ZoomTo(double amount, Point target)
        {
            double extent = this.Extent;
            Vector dif = target - this.Center;
            this.ZoomVelocity += amount;
            this.Velocity += dif * amount * lateralMovement / extent;
        }

        /// <summary>
        /// Signals the camera to update it's orientation.
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
