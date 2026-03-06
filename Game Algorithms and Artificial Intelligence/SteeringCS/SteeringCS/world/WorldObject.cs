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
    public abstract class WorldObject
    {
        public Vector_2D Position { get; set; }

        public float Radius { get; set; }

        public WorldObject(Vector_2D position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public abstract void Render(Graphics g);
    }
}