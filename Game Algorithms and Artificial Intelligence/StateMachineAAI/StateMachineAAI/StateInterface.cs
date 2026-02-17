using System;
using System.Collections.Generic;
using System.Text;

namespace StateMachineAAI
{
    public interface StateInterface
    {
        public void Enter(Entity entity);

        public void Execute(Entity entity);

        public void Exit(Entity entity);
    }
}