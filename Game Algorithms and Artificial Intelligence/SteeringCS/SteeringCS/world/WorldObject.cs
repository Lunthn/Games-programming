using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using SteeringCS.util;

namespace SteeringCS.world
{
    public class WorldObject
    {
        public Vector_2D Position { get; set; }
        public float Radius { get; set; } = 15f;

        public WorldObject(Vector_2D position)
        {
            Position = position;
        }

        public WorldObject(Vector_2D position, float radius) : this(position)
        {
            Radius = radius;
        }

        public void Render(Graphics g)
        {
            float x = (float)Position.X - Radius;
            float y = (float)Position.Y - Radius;
            float diameter = Radius * 2;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (Brush brush = new SolidBrush(Color.SlateGray))
            {
                g.FillEllipse(brush, x, y, diameter, diameter);
            }

            using (Pen pen = new Pen(Color.DarkSlateGray, 2))
            {
                g.DrawEllipse(pen, x, y, diameter, diameter);
            }
        }
    }
}