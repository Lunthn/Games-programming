using SteeringCS.entity;
using System;
using System.Drawing;

namespace SteeringCS
{
    public abstract class Steering_Behaviour
    {
        public Vehicle Owner { get; set; }

        public Steering_Behaviour(Vehicle v)
        {
            Owner = v;
        }

        public virtual void Render_for_Debug(Graphics g)
        { }

        public abstract Vector_2D Calculate();
    }
}