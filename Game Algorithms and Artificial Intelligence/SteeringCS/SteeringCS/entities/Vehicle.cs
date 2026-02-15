using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SteeringCS.behaviour;

namespace SteeringCS.entity
{
    /*
     * simple vehicle with behaviours
     */

    public class Vehicle
    {
        public World MyWorld { get; set; }
        public Vector_2D Pos { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public Vector_2D Direction { get; set; } // vector with length 1
        public Vector_2D Velocity { get; set; }// in pixel per sec.
        public virtual double Maximum_Velocity_in_pixels_per_second { get; set; }
        public virtual double Minimum_Velocity_in_pixels_per_second { get; set; }

        public bool Show_debug_info { get; set; }

        public Vehicle(World world, string name, Vector_2D pos, Vector_2D vel = null)
        {
            Name = name;
            Active = true;
            Pos = pos.Clone();
            MyWorld = world;

            if (vel == null)
            {
                Velocity = new Vector_2D();
            }
            else
            {
                Velocity = vel.Clone();
            }
            Direction = Velocity.Clone().Normalize();

            AccelerationVector_forRendering = new Vector_2D();

            // behaviours:
            SteeringBehaviourList = new List<Steering_Behaviour>();

            _seekBehaviour = new Seek_Behaviour(this);
            SteeringBehaviourList.Add(_seekBehaviour);
        }

        //---------------- behaviours ------------------
        public List<Steering_Behaviour> SteeringBehaviourList { get; set; }

        private Seek_Behaviour _seekBehaviour;

        public Vector_2D Seek_target
        { get { return _seekBehaviour.Target; } set { _seekBehaviour.Target = value; } }

        // etc ...

        public Vector_2D AccelerationVector_forRendering { get; set; }

        public override string ToString()
        {
            return Name + "<pos:" + this.Pos.X.ToString("F0") + "," + this.Pos.Y.ToString("F0") + " v: " + this.Velocity + ">";
        }

        public bool Seek_force_like_gravity { get; set; }

        public virtual void UpdateSimulation(double timeElapsed_in_ms)
        {
            if (!Active) { return; }

            //if ( ..... this vehicle needs to be removed/killed ? ..... )
            // {
            //     some cleanup
            //     return;
            // }

            // calc new velocity
            Vector_2D force_vector = new Vector_2D();

            foreach (Steering_Behaviour sb in SteeringBehaviourList)
            {
                Vector_2D force = sb.Calculate();
                force_vector.Add(force);
            }

            // force to acceleration, some physics:
            //       F = mass * acc --> acc = F/mass.
            //
            // This would mean that heavier vehicles respond slower.
            // It's basically a tuning factor
            //
            double mass = 30; // bigger: slower response
            force_vector.Divide(mass);

            AccelerationVector_forRendering = force_vector.Clone();

            // newton innertia, use current velocity for new velocity
            Velocity.Multiply(MyWorld.Newton_percentage / 100.0);
            Velocity.Add(force_vector);

            if (Velocity.Is_length_zero())
            {
                // vehicle is not allowed to stand still ... that would be boring
                // and might lead to complications in future calculations.
                //
                Velocity = Direction.Clone();
                Velocity.Multiply(Minimum_Velocity_in_pixels_per_second);
            }
            else
            {
                Direction = Vector_2D.Normalize(Velocity);
                Velocity.Clamp_to_length(Minimum_Velocity_in_pixels_per_second, Maximum_Velocity_in_pixels_per_second);
            }

            // calc distance traveled based on speed and time:
            Vector_2D delta = Vector_2D.Multiply(Velocity, timeElapsed_in_ms / 1000.0);

            this.Pos.Add(delta);

            // ---- check colisions: ----

            // if( colliding with something ) {
            //      do something, like an explosion, calculate a new direction or just stop the vehicle ... ?
            // }
        }

        public virtual void Render(Graphics g)
        {
            if (!Active) { return; }

            /*
             * vehicle is a triangle, based on Pos and Direction

                            BL
                            | \
                            |   \
                            |     \
                            |  P    > front
                            |     /
                            |   /
                            | /
                            BR

                            P = Pos

                            front = pos + factor * Direction
                            BL: back_left  = pos + factor2 * orthogonal-of-Direction
                            BR: back_right = pos - factor2 * orthogonal-of-Direction

                            orthogon: rotate by 90 degrees
                                      (i.e. swap X and Y and add - somewhere, see Rotate90DegClockwise())

             */

            // small distance in front of Pos:
            Vector_2D front = Pos.Clone().Add(Direction.Clone().Scale_to_Length(50));
            // small distance to the back and to the left:
            Vector_2D back_left = Pos.Clone().Add(Direction.Clone().Rotate_90_degrees_clockwise().Scale_to_Length(15))
                                            .Add(Direction.Clone().Scale_to_Length(-25));
            // small distance to the back and to the  right:
            Vector_2D back_right = Pos.Clone().Add(Direction.Clone().Rotate_90_degrees_counter_clockwise().Scale_to_Length(15))
                                            .Add(Direction.Clone().Scale_to_Length(-25));

            PointF[] points = { new PointF( (float) front.X, (float) front.Y),
                                new PointF( (float) back_left.X, (float) back_left.Y),
                                new PointF( (float) back_right.X, (float) back_right.Y) };
            g.FillPolygon(Brushes.YellowGreen, points);

            if (Show_debug_info)
            {
                // this behaviour has a fairly boring render, just a line to the target.
                _seekBehaviour.Render_for_Debug(g);

                // if (.... path following ....) { ... show path ... }

                // if (... some fancy behaviour ... ) { show debug info for behaviour ... }

                // etc...

                // some lines for velocity and acceleration (force)
                double scale_for_debug = 0.3; // some suitable scaling factor
                g.DrawLine(new Pen(Color.Red, 7), (int)Pos.X, (int)Pos.Y, (int)Pos.X + (int)(Velocity.X * scale_for_debug), (int)Pos.Y + (int)(Velocity.Y * scale_for_debug));

                double scale2 = 10;
                g.DrawLine(new Pen(Color.Orange, 2), (int)Pos.X, (int)Pos.Y, (int)Pos.X + (int)(AccelerationVector_forRendering.X * scale2), (int)Pos.Y + (int)(AccelerationVector_forRendering.Y * scale2));
            }
        }
    }
}