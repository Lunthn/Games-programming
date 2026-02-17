using System;
using System.Collections.Generic;
using System.Text;

namespace StateMachineAAI
{
    public class StateAttack : StateInterface
    {
        public void Enter(Entity entity)
        {
            Console.WriteLine("Starting the fight");
        }

        public void Execute(Entity entity)
        {
            Console.WriteLine("Fighting..");
            entity.strength -= 1;
        }

        public void Exit(Entity entity)
        {
            Console.WriteLine("Fight is over");
        }
    }
}