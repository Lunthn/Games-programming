using SteeringCS.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteeringCS.states
{
    public interface IState
    {
        void Enter(Zombie entity);

        void Execute(Zombie entity);

        void Exit(Zombie entity);
    }
}