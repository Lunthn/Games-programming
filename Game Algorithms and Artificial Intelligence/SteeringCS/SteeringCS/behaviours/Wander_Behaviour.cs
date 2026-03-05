using SteeringCS.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteeringCS.behaviours
{
    public class Wander_Behaviour : Steering_Behaviour
    {
        private static readonly Random _random = new Random();
        private double _wanderAngle;

        public Wander_Behaviour(Entity me) : base(me)
        {
            _wanderAngle = _random.NextDouble() * 360;
        }

        public override Vector_2D Calculate()
        {
            _wanderAngle += (_random.NextDouble() - 0.5) * 10.0;

            Vector_2D wanderDirection = new Vector_2D(1, 0);
            wanderDirection.Rotate_degrees(_wanderAngle);

            wanderDirection.Multiply(Owner.MaxVelocity * 0.2);

            Vector_2D steeringForce = Vector_2D.Subtract(wanderDirection, Owner.Velocity);

            Vector_2D boundaryForce = GetBoundaryAvoidance();
            steeringForce.Add(boundaryForce);

            return steeringForce;
        }

        // make sure entity does not walk out of world when wandering
        private Vector_2D GetBoundaryAvoidance()
        {
            Vector_2D force = new Vector_2D(0, 0);
            const double margin = 80.0;
            const double strength = 400.0;

            if (Owner.Position.X < margin)
            {
                force.X += strength * (1.0 - Owner.Position.X / margin);
            }
            else if (Owner.Position.X > Owner.MyWorld.Width - margin)
            {
                force.X -= strength * (1.0 - (Owner.MyWorld.Width - Owner.Position.X) / margin);
            }

            if (Owner.Position.Y < margin)
            {
                force.Y += strength * (1.0 - Owner.Position.Y / margin);
            }
            else if (Owner.Position.Y > Owner.MyWorld.Height - margin)
            {
                force.Y -= strength * (1.0 - (Owner.MyWorld.Height - Owner.Position.Y) / margin);
            }

            return force;
        }
    }
}