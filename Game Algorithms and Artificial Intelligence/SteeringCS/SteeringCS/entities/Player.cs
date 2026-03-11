using SteeringCS.behaviour;
using SteeringCS.behaviours;
using SteeringCS.states;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteeringCS.entities
{
    public class Player : Entity
    {
        public Player(World world, string name, Vector_2D pos, double mass, float size, Vector_2D vel = null)
        {
            MyWorld = world;
            Name = name;
            Position = pos.Clone();
            Velocity = vel?.Clone() ?? new Vector_2D();
            Direction = Velocity.Clone().Normalize();
            Mass = mass;
            Size = size;
        }
    }
}