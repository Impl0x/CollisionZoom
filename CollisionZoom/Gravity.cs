using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionZoom
{

    class Gravity
    {
        public Attachment Attachment;
        public double G;

        public Gravity(Attachment attach, double g)
        {
            this.Attachment = attach;
            this.G = g;
        }
    }
}
