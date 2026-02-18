using SteeringCS.entity;
using System;
using System.Drawing;

namespace SteeringCS.behaviour
{
    internal class Arrive_Behaviour : Steering_Behaviour
    {
        public Vector_2D Target { get; set; }

        public Arrive_Behaviour(Vehicle me) : base(me)
        {
        }

        //return the force
        public override Vector_2D Calculate()
        {
            if (Target == null) { return new Vector_2D(0, 0); }

            Vector_2D delta = Vector_2D.Subtract(Target, Owner.Pos);
            double distance = delta.Length();

            if (distance > 0)
            {
                double distanceToSlow = 100;
                double speed = Owner.Maximum_Velocity_in_pixels_per_second * Math.Min(1.0, distance / distanceToSlow);

                Vector_2D desiredVelocity = delta.Clone();
                desiredVelocity.Scale_to_Length(speed);

                return Vector_2D.Subtract(desiredVelocity, Owner.Velocity);
            }

            return new Vector_2D(0, 0);
        }

        public override void Render_for_Debug(Graphics g)
        {
            // line from owner to target:
            Pen p = new Pen(Color.Blue, 3);
            g.DrawLine(p, (int)Target.X, (int)Target.Y, (int)Owner.Pos.X, (int)Owner.Pos.Y);
        }
    }
}