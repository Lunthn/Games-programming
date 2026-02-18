using System;
using System.Drawing;
using SteeringCS.entity;
using SteeringCS.util;

namespace SteeringCS.behaviour
{
    internal class Arrive_Behaviour : Steering_Behaviour
    {
        public Vector_2D Target { get; set; }

        public Arrive_Behaviour(Entity me) : base(me)
        {
        }

        public override Vector_2D Calculate()
        {
            if (Target == null) return new Vector_2D(0, 0);

            Vector_2D delta = Vector_2D.Subtract(Target, Owner.Position);
            double distance = delta.Length();

            if (distance <= 0) return new Vector_2D(0, 0);

            const double SlowingDistance = 100.0;
            double speed = Owner.MaxVelocity * Math.Min(1.0, distance / SlowingDistance);

            Vector_2D desiredVelocity = delta.Clone();
            desiredVelocity.Scale_to_Length(speed);

            return Vector_2D.Subtract(desiredVelocity, Owner.Velocity);
        }

        public override void Render_for_Debug(Graphics g)
        {
            if (Target == null) return;

            // Draw a line from the vehicle's position to its arrival target
            using (Pen pen = new Pen(Color.Blue, 2))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                g.DrawLine(pen,
                    (int)Owner.Position.X, (int)Owner.Position.Y,
                    (int)Target.X, (int)Target.Y);
            }
        }
    }
}