using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CollisionZoom
{
    static class Program
    {
        public static Random rand = new Random();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using(Window window = new Window())
            {
                window.Run();
            }
        }
    }
}
