using SteeringCS.entity;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace SteeringCS.behaviour
{
    internal class Wandering_Behaviour : Steering_Behaviour
    {
        public double Wander_radius = 100;    
        public double Wander_distance = 120;
        private double _wanderAngle;

        public Wandering_Behaviour(Vehicle me) : base(me)
        {
            this._wanderAngle = 0;
        }

        public override Vector_2D Calculate()
        {
            double jitter = 0.5;
            _wanderAngle += (new Random().NextDouble() * 100 ) * jitter;

            Vector_2D target = new Vector_2D(
                Math.Cos(_wanderAngle) * Wander_radius,
                Math.Sin(_wanderAngle) * Wander_radius
            );

            Vector_2D force = Owner.Direction.Clone().Multiply(Wander_distance).Add(target);

            return force;
        }

        public override void Render_for_Debug(Graphics g)
        {
            Vector_2D heading = new Vector_2D(Owner.Velocity);
            if (heading.Length() < 0.01) heading = new Vector_2D(0, -1);
            heading.Normalize();

            Vector_2D circleCenter = new Vector_2D(Owner.Pos).Add(heading.Multiply(Wander_distance));

            using (Pen circlePen = new Pen(Color.FromArgb(100, Color.Gray), 1))
            {
                float r = (float)Wander_radius;
                g.DrawEllipse(circlePen, (float)circleCenter.X - r, (float)circleCenter.Y - r, r * 2, r * 2);
            }

            Vector_2D force = new Vector_2D(circleCenter).Add(new Vector_2D(
                Math.Cos(_wanderAngle) * Wander_radius,
                Math.Sin(_wanderAngle) * Wander_radius
            ));


            using (Pen p = new Pen(Color.Yellow, 3))
            {
                g.DrawLine(p, (float)Owner.Pos.X, (float)Owner.Pos.Y, (float)force.X, (float)force.Y);
            }
        }
    }
}