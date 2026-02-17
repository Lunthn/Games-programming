using System;
using System.Collections.Generic;
using System.Text;

namespace StateMachineAAI
{
    public class StateHide : StateInterface
    {
        public void Enter(Entity entity)
        {
            Console.WriteLine("Going to hide");
        }

        public void Execute(Entity entity)
        {
            entity.strength++;
            Console.WriteLine("Hiding..");
        }

        public void Exit(Entity entity)
        {
            Console.WriteLine("Leaving hiding spot");
        }
    }
}