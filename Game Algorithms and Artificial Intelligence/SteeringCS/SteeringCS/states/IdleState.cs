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
    public class IdleState : IState
    {
        public void Enter(Zombie entity)
        {
            entity.FinalTarget = null;

            entity.SeekBehaviour.IsActive = false;
            entity.WanderBehaviour.IsActive = true;
            entity.SeparationBehaviour.IsActive = true;
        }

        public void Execute(Zombie entity)
        {
            if (entity.FinalTarget != null)
            {
                entity.ChangeState(new InvestigatingState());
            }
        }

        public void Exit(Zombie entity)
        {
        }
    }
}