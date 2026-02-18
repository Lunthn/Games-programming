using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteeringCS.world
{
    public class Wall
    {
        private Vector_2D _start { get; set; }
        private Vector_2D _end { get; set; }

        public Wall(Vector_2D start, Vector_2D end)
        {
            _start = start;
            _end = end;
        }

        public void Render(Graphics g)
        {
            using (Pen pen = new Pen(Color.Gray, 3))
            {
                g.DrawLine(pen, 
                           new PointF((float)_start.X, (float)_start.Y), 
                           new PointF((float)_end.X, (float)_end.Y));
            }
        }
    }
}