using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteeringCS.world
{
    public class BushObject : WorldObject
    {
        public BushObject(Vector_2D position, float radius) : base(position, radius)
        {
        }

        public override void Render(Graphics g)
        {
            float x = (float)Position.X - Radius;
            float y = (float)Position.Y - Radius;
            float diameter = Radius * 2;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (Brush brush = new SolidBrush(Color.LightGreen))
            {
                g.FillEllipse(brush, x, y, diameter, diameter);
            }

            using (Pen pen = new Pen(Color.Green, 2))
            {
                g.DrawEllipse(pen, x, y, diameter, diameter);
            }
        }
    }
}