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
    public class IdleState : IState
    {
        public void Enter(Entity entity)
        {
            entity.FinalTarget = null;

            entity.SeekBehaviour.IsActive = false;
            entity.WanderBehaviour.IsActive = true;
            entity.SeparationBehaviour.IsActive = true;
        }

        public void Execute(Entity entity)
        {
            if (entity.FinalTarget != null)
            {
                entity.ChangeState(new SeekingState());
            }
        }

        public void Exit(Entity entity)
        {
        }
    }
}