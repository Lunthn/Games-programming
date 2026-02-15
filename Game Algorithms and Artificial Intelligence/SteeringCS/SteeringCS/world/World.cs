using SteeringCS.entity;
using SteeringCS.util;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

/*
 * the World is the center. Gui (form) communicates with the world (sends commands, requests render)
 * The world has a bunch of vehicles, possibly obstacles and a grid (for pathfinding), spatial partitioning, etc...
 *
 *  the World::Update_simulation(...) works via threading (see RunThread).
 *  the world::render(...) works with a timer (see Form1).
 *
 */

namespace SteeringCS
{
    public class World : My_base_thread
    {
        public World(int w, int h, int update_time_step)
        {
            Set_thread_as_background(true);
            this.update_time_step_in_ms = update_time_step;
            Width = w;
            Height = h;

            vehicles_list = new List<Vehicle>();
            Seek_target = new Vector_2D(Width / 2, Height / 2);

            Populate(3);
        }

        private void Populate(int nr_of_entities)
        {
            lock (lock_update_object)
            {
                vehicles_list.Clear();

                Set_seek_target(Width / 2, Height / 2);

                Random random_generator = new Random();

                for (int i = 0; i < nr_of_entities; i++)
                {
                    double x = Width * random_generator.NextDouble();
                    double y = Height * random_generator.NextDouble();
                    Vector_2D vehiclePos = new Vector_2D(x, y);
                    Vector_2D vehicleVel = new Vector_2D(1, 0).Multiply(100);
                    double degrees = random_generator.NextDouble() * 90 - 45;
                    vehicleVel.Rotate_degrees(degrees);

                    Vehicle vehicle = new Vehicle(this, "just a vehicle", vehiclePos, vehicleVel);
                    vehicle.Seek_target = Seek_target;
                    vehicles_list.Add(vehicle);
                }

                // the next few lines look weird! It feels as if these lines are just x = x; and therefore do nothing....
                //
                // However, this code does actually work, because these are properties, not attributes!
                // These properties contain a loop that propagates the min/max velocities to the vehicles.
                // So assigning this to itself, triggers this loop and gives all the vehicles their min/max values.
                // However, if these lines are not here, the Min and max velocity of each vehicle are 0
                // and they just stand still, until the max velocity slider is adjusted (because that
                // sends the new max. velocity to the vehicles).
                //
                // This can obviously be replaced by a loop here that sets the min/max of the vehicles
                // But I left it here as a reminder of how properties can have (intended) side effects
                // (which is good) but they also hide the side effects from the developer (which is bad).
                //
                Min_velocity_in_pixels = Min_velocity_in_pixels;
                Max_velocity_in_pixels = Max_velocity_in_pixels;
                Show_debug_info = Show_debug_info;
                Seek_force_like_gravity = Seek_force_like_gravity;
                //
                // end of C#-property weirdness :-)

                // initialise other things like graph...
            }
        }

        public void Randomize_positions(int nr_of_vehicles)
        {
            if (nr_of_vehicles < 1) { nr_of_vehicles = 1; }

            // bool flag used to stop the RunThread while loop
            Stop_update_loop();
            Wait_for_update_loop_to_finish();

            lock (lock_update_object)
            {
                // change the world:
                Populate(nr_of_vehicles);
                Start_update_loop();
            }
        }

        //========================================== settings etc. ====================================

        public List<Vehicle> vehicles_list;

        // the target (crosshairs, controlled with the mouse)
        public Vector_2D Seek_target { get; }

        // don't set the seek_target to a new Vector_2D,
        // because the seek behaviour has a reference to this vector.
        public void Set_seek_target(int x, int y)
        {
            Seek_target.X = x;
            Seek_target.Y = y;
        }

        // resize window
        public void Set_size(int w, int h)
        {
            lock (lock_update_object) // because multithreaded
            {
                Width = w;
                Height = h;
                // do all sorts of initializing/whatever to adjust the simulation to the new window size
            }
        }

        private int Width { get; set; }
        private int Height { get; set; }

        //================================ simulation/thread related =======================================

        private readonly object lock_update_object = new object();

        public bool Play { get; set; }
        private double update_time_step_in_ms;

        public void Update_simulation()
        {
            lock (lock_update_object)
            {
                foreach (Vehicle me in vehicles_list)
                {
                    me.UpdateSimulation(update_time_step_in_ms);
                }
            }
        }

        private void Stop_update_loop()
        {
            _update_loop_should_stop = true;
        }

        private void Start_update_loop()
        {
            _update_loop_should_stop = false;
            _update_loop_is_running = true;
        }

        private void Wait_for_update_loop_to_finish()
        {
            while (_update_loop_is_running) { Thread.Sleep(1); }
        }

        // flags for thread manipulation (it's rude to just kill a thread,
        // so we use a bool flag to notify the thread that it has to do someting)
        private bool _update_loop_should_stop = false;

        private bool _update_loop_is_running = true;

        // called when world.Start() is executed.
        public override void Run_thread()
        {
            Play = true;

            // outer loop: entire life of the world
            while (true)
            {
                if (_update_loop_is_running)
                {
                    // inner loop: one simulation.
                    // Breaks when restart is clicked in form
                    while (true)
                    {
                        DateTime beforeRunOneStep = DateTime.Now;
                        if (Play)
                        {
                            Update_simulation();
                        }

                        if (_update_loop_should_stop)
                        {
                            _update_loop_should_stop = false;
                            _update_loop_is_running = false;
                            break;
                        }

                        // check how much time has passed during calc,... sleep for the rest of the time:
                        double remainingTimeToSleep = Math.Max(0, update_time_step_in_ms - (DateTime.Now - beforeRunOneStep).TotalMilliseconds);
                        Thread.Sleep((int)remainingTimeToSleep);
                    }
                }
                else
                {
                    // idling. Simulation is paused.
                    Thread.Sleep(3);
                }
            }
        }

        public void Render(Graphics g)
        {
            // if ( show the grid/graph ) ...

            try
            {
                // render all the vehicles (and obstacles, etc...)
                foreach (Vehicle me in vehicles_list)
                {
                    // Render method can show debug info as well
                    me.Render(g);
                }
            }
            catch
            {
                // the Update_simulation(...) could change the collection (vehiclesList).
                // (because e.g. a vehicle explodes and is removed from the list).
                //
                // But at the sime time, the render_timer thread could try to render the vehicles
                // using the foreach loop.
                //
                // If a collection is modified while you iterate over it in a foreach loop,
                // an exception is thrown.
            }

            Pen pen = new Pen(Color.Black, 3);
            g.DrawLine(pen, (float)Seek_target.X - 10, (float)Seek_target.Y - 10, (float)Seek_target.X + 10, (float)Seek_target.Y + 10);
            g.DrawLine(pen, (float)Seek_target.X + 10, (float)Seek_target.Y - 10, (float)Seek_target.X - 10, (float)Seek_target.Y + 10);
        }

        ////////////////////////////////////// behaviours and settings /////////////////////////////////////

        // Newton_percentage: every step of the simulation (see Vehicle::UpdateSimulation(...))
        // a part of the old velocity is kept. The Newton_percentage determines how big this part is:
        //             Velocity.Multiply(MyWorld.Newton_percentage / 100.0 );
        //
        // If percentage is 100, you have a frictionless world where once you are in motion,
        // you stay in motion (e.g. think spaceships and planets, or perfect ice).
        // This is one of Newton's laws: a moving object continues moving at the same speed forever
        // unless there is a force acting on the object.
        //
        // if percentage < 100, then you have friction. So if you start with a velocity
        // and there are no forces acting on you, your velocity will decrease, because every
        // step of the simulation, your new velocity is a fraction of the old one

        private int newton_percentage = 97;

        public int Newton_percentage
        {
            get { return newton_percentage; }
            set
            {
                if (value < 0) { value = 0; }
                if (value > 100) { value = 100; }
                newton_percentage = value;
            }
        }

        private double _maximumVelocity_in_pixels_per_second = 500;

        public double Max_velocity_in_pixels
        {
            get { return _maximumVelocity_in_pixels_per_second; }
            set
            {
                if (value < _minimumVelocity_in_pixels_per_second) { return; }
                _maximumVelocity_in_pixels_per_second = value;
                foreach (Vehicle veh in vehicles_list)
                {
                    veh.Maximum_Velocity_in_pixels_per_second = _maximumVelocity_in_pixels_per_second;
                }
            }
        }

        private double _minimumVelocity_in_pixels_per_second = 1;

        public double Min_velocity_in_pixels
        {
            get { return _minimumVelocity_in_pixels_per_second; }
            set
            {
                if (value > Max_velocity_in_pixels) { return; }
                if (value < 0) { value = 0; }
                _minimumVelocity_in_pixels_per_second = value;
                foreach (Vehicle me in vehicles_list)
                {
                    me.Minimum_Velocity_in_pixels_per_second = _minimumVelocity_in_pixels_per_second;
                }
            }
        }

        private bool show_debug_info = true;

        public bool Show_debug_info
        {
            get { return show_debug_info; }
            set
            {
                show_debug_info = value;
                foreach (Vehicle v in vehicles_list)
                {
                    v.Show_debug_info = show_debug_info;
                }
            }
        }

        private bool seek_force_like_gravity = false;

        public bool Seek_force_like_gravity
        {
            get { return seek_force_like_gravity; }
            set
            {
                seek_force_like_gravity = value;
                // let all vehicles know:
                foreach (Vehicle v in vehicles_list)
                {
                    v.Seek_force_like_gravity = value;
                }
            }
        }
    }
}