using SteeringCS.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteeringCS.states
{
    public interface IState
    {
        void Enter(Entity entity);

        void Execute(Entity entity);

        void Exit(Entity entity);
    }
}