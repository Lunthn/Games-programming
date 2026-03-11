using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteeringCS.entities
{
    public abstract class Entity
    {
        public World MyWorld { get; set; }
        public Vector_2D Position { get; set; }
        public string Name { get; set; }
        public Vector_2D Direction { get; set; }
        public Vector_2D Velocity { get; set; }
        public double Mass { get; set; }
        public float Size { get; set; }
        public int DetectionRadius { get; set; } = 250;
        public virtual double MaxVelocity { get; set; }
        public virtual double MinVelocity { get; set; }
        public bool ShowDebugInfo { get; set; } = false;
        public Vector_2D AccelerationForRendering { get; set; } = new Vector_2D();
    }
}