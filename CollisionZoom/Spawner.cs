using System;

using Point = CollisionZoom.Vector;

namespace CollisionZoom
{
    class Spawner
    {
        Attachment Attachment;
        Action<Point, Vector> Spawn;
        public uint Limit;
        public double RateOfFire;
        double timer;
        public int Fired;

        public Spawner(Attachment attach, Action<Point, Vector> spawn, uint limit, double rof)
        {
            this.Attachment = attach;
            this.Spawn = spawn;
            this.Limit = limit;
            this.RateOfFire = rof;
            timer = 0;
            Fired = 0;
        }

        public void Update(double updateTime)
        {
            timer += updateTime;
            if (this.Limit > 0)
            {
                while (timer >= this.RateOfFire && Fired < this.Limit)
                {
                    timer -= this.RateOfFire;
                    Spawn(this.Attachment.Position, this.Attachment.Velocity);
                    Fired++;
                }
            }
        }
    }
}
