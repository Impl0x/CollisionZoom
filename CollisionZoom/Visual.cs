using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CollisionZoom
{
    /// <summary>
    /// Contains the visual information for an object.
    /// </summary>
    abstract class Visual
    {
        public abstract void Render();

        public static ProjectileVisual Projectile(Projectile source)
        {
            return new ProjectileVisual(source);
        }
        public static ProjectileVisual Projectile(Projectile source, Color4 color)
        {
            return new ProjectileVisual(source, color);
        } 
        public static SpawnerVisual Spawner(Attachment source)
        {
            return new SpawnerVisual(source);
        }
        public static SpawnerVisual Spawner(Attachment source, Color4 color)
        {
            return new SpawnerVisual(source, color);
        }
        public static Color4 RandomColor()
        {
            int r = Program.rand.Next(256);
            int g = Program.rand.Next(256);
            int b = Program.rand.Next(256);
            return new Color4((byte)r, (byte)g, (byte)b, (byte)255);
        }
    }

    class ProjectileVisual : Visual
    {
        private Projectile Source;
        private Color4 Color;

        public ProjectileVisual(Projectile source, Color4 color)
        {
            this.Source = source;
            this.Color = color;
        }

        public ProjectileVisual(Projectile source)
            : this(source, Visual.RandomColor())
        { }

        public override void Render()
        {
            GL.Begin(BeginMode.TriangleFan);
            int triangles = 50;
            double twoPI = Math.PI * 2.0;
            GL.Color4(this.Color);
            GL.Vertex2(this.Source.Position);
            for (int i = 0; i <= triangles; i++)
            {
                GL.Vertex2(this.Source.Position.X + this.Source.Radius * Math.Cos(i * twoPI / triangles),
                           this.Source.Position.Y + this.Source.Radius * Math.Sin(i * twoPI / triangles));
            }
            GL.End();
        }
    }

    class SpawnerVisual : Visual
    {
        private Attachment Source;
        private Color4 Color;

        public SpawnerVisual(Attachment source, Color4 color)
        {
            this.Source = source;
            this.Color = color;
        }

        public SpawnerVisual(Attachment source)
            : this(source, Visual.RandomColor())
        { }

        public override void Render()
        {
            GL.Begin(BeginMode.TriangleFan);
            int triangles = 3;
            double twoPI = Math.PI * 2.0;
            GL.Color4(this.Color);
            GL.Vertex2(this.Source.Position);
            for (int i = 0; i <= triangles; i++)
            {
                GL.Vertex2(this.Source.Position.X + 1.0 * Math.Cos(i * twoPI / triangles),
                           this.Source.Position.Y + 1.0 * Math.Sin(i * twoPI / triangles));
            }
            GL.End();
        }
    }
}
