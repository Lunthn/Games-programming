using SteeringCS.entity;
using System.Drawing;

namespace SteeringCS.behaviour
{
    internal class Seek_Behaviour : Steering_Behaviour
    {
        public Vector_2D Target { get; set; }

        public Seek_Behaviour(Vehicle me) : base(me)
        {
        }

        //return the force
        public override Vector_2D Calculate()
        {
            if (Target == null) { return null; }

            // distance to target;
            Vector_2D delta = Vector_2D.Subtract(Target, Owner.Pos);

            double distance = delta.Length();
            double force;

            // the further away, the bigger the force/desire to go to target
            force = 2 * distance;

            Vector_2D force_vector = delta.Clone();
            force_vector.Scale_to_Length(force);

            return force_vector;
        }

        public override void Render_for_Debug(Graphics g)
        {
            // line from owner to target:
            Pen p = new Pen(Color.Blue, 3);
            g.DrawLine(p, (int)Target.X, (int)Target.Y, (int)Owner.Pos.X, (int)Owner.Pos.Y);
        }
    }
}