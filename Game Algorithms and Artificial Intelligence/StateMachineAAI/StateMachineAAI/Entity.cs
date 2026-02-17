using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace StateMachineAAI
{
    public class Entity
    {
        public int strength;
        public StateInterface state;
        private Random _random;

        public Entity(int strength, StateInterface state)
        {
            this.strength = strength;
            this.state = state;
            this._random = new Random();

            this.state.Enter(this);
        }

        public bool EnemyClose()
        {
            return _random.Next(0, 10) <= 3;
        }

        public void Update()
        {
            state.Execute(this);
        }

        public void ChangeState(StateInterface state)
        {
            this.state.Exit(this);
            this.state = state;
            this.state.Enter(this);
        }
    }
}