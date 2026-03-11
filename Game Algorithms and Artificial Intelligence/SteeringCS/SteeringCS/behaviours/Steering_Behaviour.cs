using SteeringCS.entities;
using System;
using System.Drawing;

namespace SteeringCS
{
    public abstract class Steering_Behaviour
    {
        public Zombie Owner { get; set; }
        public bool IsActive { get; set; }

        public Steering_Behaviour(Zombie v)
        {
            Owner = v;
            IsActive = false;
        }

        public virtual void Render_for_Debug(Graphics g)
        { }

        public abstract Vector_2D Calculate();
    }
}