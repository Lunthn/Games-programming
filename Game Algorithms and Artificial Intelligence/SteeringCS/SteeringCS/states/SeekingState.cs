using SteeringCS.entity;
using SteeringCS.behaviour;
using SteeringCS.behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteeringCS.states
{
    public class SeekingState : IState
    {
        public void Enter(Entity entity)
        {
            entity.WanderBehaviour.IsActive = false;
            entity.SeekBehaviour.IsActive = true;
            entity.SeparationBehaviour.IsActive = true;
        }

        public void Execute(Entity entity)
        {
            if (entity.FinalTarget == null)
            {
                entity.ChangeState(new IdleState());
                return;
            }

            double distance = Vector_2D.Distance(entity.Position, entity.FinalTarget);
            if (distance < 20.0)
            {
                // all entities to idle, because target is dead
                foreach (Entity e in entity.MyWorld.Entities)
                {
                    e.ChangeState(new IdleState());
                }
            }
        }

        public void Exit(Entity entity)
        {
        }
    }
}