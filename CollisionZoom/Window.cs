using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System;
using System.Collections.Generic;

using Point = CollisionZoom.Vector;

namespace CollisionZoom
{
    class Window : GameWindow
    {
        public Camera Camera;
        public World World;
        public List<Spawner> Spawners;
        public List<Visual> Visuals;
        bool following = false;
        Attachment cameraAttach;
        bool dragging;
        Point dragInitialPoint = Point.Zero;
        bool running;

        /// <summary>
        /// Gets the current transform from viewspace
        /// coordinates to worldspace coordinates for
        /// this window.
        /// </summary>
        public Transform ViewTransform
        {
            get
            {
                return Render.NormalizeView((float)this.Width / (float)this.Height, this.Camera.Transform);
            }
        }
 
        public Window () : base(800, 600, new GraphicsMode(16, 16))
        { }

        /// <summary>
        /// Gets the worldspace point at the given window coordinates.
        /// </summary>
        public Point Project(int x, int y)
        {
            Transform view = this.ViewTransform;
            return (view.Project(new Point((float)x / (float)this.Width * 2.0 - 1.0, 1.0 - (float)y / (float)this.Height * 2.0)));
        }

        // Input handlers.
        void Mouse_WheelChanged(object sender, OpenTK.Input.MouseWheelEventArgs e)
        {
            double delta = e.DeltaPrecise;
            if (following)
            {
                this.Camera.ZoomVelocity += e.DeltaPrecise;
            }
            else
            {
                this.Camera.ZoomTo((float)e.DeltaPrecise, this.Project(e.X, e.Y));
            }
        }
        void Mouse_ButtonDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            if (e.Button == OpenTK.Input.MouseButton.Left)
            {
                Point mousePoint = this.Project(Mouse.X, Mouse.Y);
                Projectile picked = this.World.Pick(mousePoint);
                if (picked != null)
                {
                    following = true;
                    cameraAttach = picked.Attachment;
                }
            }
        }
        void Keyboard_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if(e.Key == OpenTK.Input.Key.Space)
            {
                this.running = !running;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Mouse.WheelChanged += Mouse_WheelChanged;
            this.Mouse.ButtonDown += Mouse_ButtonDown;
            this.Keyboard.KeyDown += Keyboard_KeyDown;

            this.World = new World(100, 100);
            this.Camera = new Camera(Vector.Zero, -5.0);
            this.Spawners = new List<Spawner>();
            this.Visuals = new List<Visual>();
            running = true;

            Action<Point, Vector> spawn = delegate(Point pos, Vector vel)
            {
                Projectile newP = new Projectile(1.0, 1.0, pos, vel);
                this.World.SpawnProjectile(newP);
                this.Visuals.Add(new ProjectileVisual(newP));
            };

            for(int i = 1; i <= 10; i++)
            {
                Attachment spawnAttach = Attachment.Circular(30.0 * i, 1.0 * i, this.World);
                Spawner s = new Spawner(spawnAttach, spawn, 25, 0.5);
                this.Spawners.Add(s);
                this.Visuals.Add(Visual.Spawner(spawnAttach));
            }

            Attachment gravAttach = Attachment.Fixed(Point.Zero);
            Gravity g = new Gravity(gravAttach, 2000.0);
            this.World.SpawnGravity(g);

            this.VSync = VSyncMode.On;
            this.MakeCurrent();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            double updateTime = e.Time;
            this.Camera.Update(updateTime);

            if (running)
            {
                this.World.Update(updateTime);
                foreach (Spawner s in this.Spawners)
                {
                    s.Update(updateTime);
                }
            }
            
            if(following)
            {
                this.Camera.Center = cameraAttach.Position;
            }

            if(Mouse[OpenTK.Input.MouseButton.Middle])
            {
                following = false;
                if (dragging)
                {
                    this.Camera.ZoomVelocity = 0.0;
                    Point mousePoint = this.Project(Mouse.X, Mouse.Y);
                    Point dif = dragInitialPoint - mousePoint;
                    this.Camera.Center += dif;
                    this.Camera.Velocity = (dif / this.Camera.Extent / e.Time + this.Camera.Velocity * 10.0) / 11.0;
                }
                else
                {
                    dragInitialPoint = this.Project(Mouse.X, Mouse.Y);
                    dragging = true;
                }
            }
            else
            {
                dragging = false;
            }
        }

        Vector3 up = new Vector3(0, 1, 0);
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4d transform = this.ViewTransform.Inverse;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref transform);
            foreach(Visual v in this.Visuals)
            {
                v.Render();
            }
            this.SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
        }
    }
}
