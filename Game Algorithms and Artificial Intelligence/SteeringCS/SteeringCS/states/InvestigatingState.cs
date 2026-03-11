using SteeringCS.entities;
using SteeringCS.behaviour;
using SteeringCS.behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteeringCS.states
{
    public class InvestigatingState : IState
    {
        public void Enter(Zombie entity)
        {
            entity.SeekBehaviour.IsActive = true;
            entity.WanderBehaviour.IsActive = true;
            entity.SeparationBehaviour.IsActive = true;

            entity.MaxVelocity /= 2;
        }

        public void Execute(Zombie entity)
        {
            if (entity.FinalTarget == null)
            {
                entity.ChangeState(new IdleState());
                return;
            }

            double distance = Vector_2D.Distance(entity.Position, entity.FinalTarget);

            if (entity.DetectionRadius >= distance)
            {
                entity.ChangeState(new SeekingState());
            }
        }

        public void Exit(Zombie entity)
        {
            entity.MaxVelocity *= 2;
        }
    }
}