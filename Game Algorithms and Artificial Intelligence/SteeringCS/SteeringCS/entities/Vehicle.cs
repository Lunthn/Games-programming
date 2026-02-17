using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SteeringCS.behaviour;

namespace SteeringCS.entity
{
    public class Vehicle
    {
        public World MyWorld { get; set; }
        public Vector_2D Pos { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public Vector_2D Direction { get; set; }
        public Vector_2D Velocity { get; set; }
        public virtual double Maximum_Velocity_in_pixels_per_second { get; set; }
        public virtual double Minimum_Velocity_in_pixels_per_second { get; set; }

        public bool Show_debug_info { get; set; }
        public List<Steering_Behaviour> SteeringBehaviourList { get; set; }

        private Arrive_Behaviour _arriveBehaviour;

        public Vector_2D Arrive_target
        { get { return _arriveBehaviour.Target; } set { _arriveBehaviour.Target = value; } }

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

            SteeringBehaviourList = new List<Steering_Behaviour>();

            _arriveBehaviour = new Arrive_Behaviour(this);
            SteeringBehaviourList.Add(_arriveBehaviour);
        }

        public Vector_2D AccelerationVector_forRendering { get; set; }

        public override string ToString()
        {
            return Name + "<pos:" + this.Pos.X.ToString("F0") + "," + this.Pos.Y.ToString("F0") + " v: " + this.Velocity + ">";
        }

        public virtual void UpdateSimulation(double timeElapsed_in_ms)
        {
            if (!Active) { return; }

            Vector_2D force_vector = new Vector_2D();

            foreach (Steering_Behaviour sb in SteeringBehaviourList)
            {
                Vector_2D force = sb.Calculate();
                force_vector.Add(force);
            }

            double mass = 10; // bigger: slower response
            force_vector.Divide(mass);

            AccelerationVector_forRendering = force_vector.Clone();

            Velocity.Multiply(MyWorld.Newton_percentage / 100.0);
            Velocity.Add(force_vector);

            Direction = Vector_2D.Normalize(Velocity);
            Velocity.Clamp_to_length(Minimum_Velocity_in_pixels_per_second, Maximum_Velocity_in_pixels_per_second);

            // calc distance traveled based on speed and time:
            Vector_2D delta = Vector_2D.Multiply(Velocity, timeElapsed_in_ms / 1000.0);

            this.Pos.Add(delta);
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

            Vector_2D front = Pos.Clone().Add(Direction.Clone().Scale_to_Length(50));

            Vector_2D back_left = Pos.Clone().Add(Direction.Clone().Rotate_90_degrees_clockwise().Scale_to_Length(15))
                                            .Add(Direction.Clone().Scale_to_Length(-25));
            Vector_2D back_right = Pos.Clone().Add(Direction.Clone().Rotate_90_degrees_counter_clockwise().Scale_to_Length(15))
                                            .Add(Direction.Clone().Scale_to_Length(-25));

            PointF[] points = { new PointF( (float) front.X, (float) front.Y),
                                new PointF( (float) back_left.X, (float) back_left.Y),
                                new PointF( (float) back_right.X, (float) back_right.Y) };
            g.FillPolygon(Brushes.Gray, points);

            if (Show_debug_info)
            {
                foreach (Steering_Behaviour sb in SteeringBehaviourList)
                {
                    sb.Render_for_Debug(g);
                }

                // if (.... path following ....) { ... show path ... }

                // if (... some fancy behaviour ... ) { show debug info for behaviour ... }

                double scale_for_debug = 0.3;
                g.DrawLine(new Pen(Color.Red, 7), (int)Pos.X, (int)Pos.Y, (int)Pos.X + (int)(Velocity.X * scale_for_debug), (int)Pos.Y + (int)(Velocity.Y * scale_for_debug));

                double scale2 = 10;
                g.DrawLine(new Pen(Color.Orange, 2), (int)Pos.X, (int)Pos.Y, (int)Pos.X + (int)(AccelerationVector_forRendering.X * scale2), (int)Pos.Y + (int)(AccelerationVector_forRendering.Y * scale2));

                Font debugFont = new Font("Arial", 8);
                float textYOffset = 40;

                g.DrawString("Active Steering Behaviours:", new Font(debugFont, FontStyle.Bold), Brushes.Black, (float)Pos.X, (float)Pos.Y + textYOffset);

                for (int i = 0; i < SteeringBehaviourList.Count; i++)
                {
                    textYOffset += 15;
                    string behaviorName = SteeringBehaviourList[i].GetType().Name;

                    g.DrawString(behaviorName, debugFont, Brushes.DarkSlateGray, (float)Pos.X, (float)Pos.Y + textYOffset);
                }
            }
        }
    }
}