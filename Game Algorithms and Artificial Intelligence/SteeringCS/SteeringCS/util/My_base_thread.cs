using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteeringCS.util
{
    /*
     *   
     // https://stackoverflow.com/questions/8123461/unable-to-inherit-from-a-thread-class-in-c-sharp
     // https://stackoverflow.com/a/8123600
     
     public MyThread : MyBaseThread
      {
        public override void RunThread()
        {
            // Do some stuff
        }
    }
     */


    
    public abstract class My_base_thread
    {
        private Thread _thread;

        protected My_base_thread()
        {
            _thread = new Thread(new ThreadStart(this.Run_thread));
        }

        // Thread methods / properties
        public void Start() => _thread.Start();
        public void Join() => _thread.Join();
        public bool IsAlive => _thread.IsAlive;
        public void Set_thread_as_background(bool b)
        {
            _thread.IsBackground = b;
        }

        // Override in base class
        public abstract void Run_thread();
    }
}

    