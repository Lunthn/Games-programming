using System;
using System.Collections.Generic;
using System.Text;

namespace StateMachineAAI
{
    public class StatePatrol : StateInterface
    {
        public void Enter(Entity entity)
        {
            Console.WriteLine("Start patrolling the area");
        }

        public void Execute(Entity entity)
        {
            entity.strength++;
            Console.WriteLine("Patrolling..");
        }

        public void Exit(Entity entity)
        {
            Console.WriteLine("Enough patrolling");
        }
    }
}