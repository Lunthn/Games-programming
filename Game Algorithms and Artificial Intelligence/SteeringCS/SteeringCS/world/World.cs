using SteeringCS.entity;
using SteeringCS.util;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Threading;

namespace SteeringCS
{
    /// <summary>
    /// World acts as the simulation hub.
    /// Updates run on a background thread; rendering is triggered by the UI timer.
    /// </summary>
    public class World : My_base_thread
    {
        // --- Properties & Fields ---
        private int Width { get; set; }

        private int Height { get; set; }
        public List<Vehicle> vehicles_list;
        public Vector_2D Seek_target { get; }

        private readonly object lock_update_object = new object();
        public bool Play { get; set; }
        private double update_time_step_in_ms;

        // --- Threading Flags ---
        private bool _update_loop_should_stop = false;

        private bool _update_loop_is_running = true;

        // --- Settings with Propagating Properties ---
        private int newton_percentage = 97;

        public int Newton_percentage
        {
            get => newton_percentage;
            set => newton_percentage = Math.Max(0, Math.Min(100, value));
        }

        private double _maximumVelocity_in_pixels_per_second = 500;

        public double Max_velocity_in_pixels
        {
            get => _maximumVelocity_in_pixels_per_second;
            set
            {
                if (value < _minimumVelocity_in_pixels_per_second) return;
                _maximumVelocity_in_pixels_per_second = value;
                vehicles_list.ForEach(v => v.Maximum_Velocity_in_pixels_per_second = value);
            }
        }

        private double _minimumVelocity_in_pixels_per_second = 1;

        public double Min_velocity_in_pixels
        {
            get => _minimumVelocity_in_pixels_per_second;
            set
            {
                if (value > Max_velocity_in_pixels || value < 0) return;
                _minimumVelocity_in_pixels_per_second = value;
                vehicles_list.ForEach(v => v.Minimum_Velocity_in_pixels_per_second = value);
            }
        }

        private bool show_debug_info = true;

        public bool Show_debug_info
        {
            get => show_debug_info;
            set
            {
                show_debug_info = value;
                vehicles_list.ForEach(v => v.Show_debug_info = value);
            }
        }

        // --- Initialization ---
        // --- Initialization --
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
                Random rng = new Random();

                for (int i = 0; i < nr_of_entities; i++)
                {
                    Vector_2D pos = new Vector_2D(Width * rng.NextDouble(), Height * rng.NextDouble());
                    Vector_2D vel = new Vector_2D(1, 0).Multiply(100);
                    vel.Rotate_degrees(rng.NextDouble() * 90 - 45);

                    Vehicle v = new Vehicle(this, "vehicle", pos, vel) { Seek_target = Seek_target };
                    vehicles_list.Add(v);
                }

                // Self-assignment triggers property logic to sync vehicle stats
                Min_velocity_in_pixels = Min_velocity_in_pixels;
                Max_velocity_in_pixels = Max_velocity_in_pixels;
                Show_debug_info = Show_debug_info;
            }
        }

        // --- Simulation Logic ---
        public void Update_simulation()
        {
            lock (lock_update_object)
            {
                foreach (Vehicle v in vehicles_list) v.UpdateSimulation(update_time_step_in_ms);
            }
        }

        public override void Run_thread()
        {
            Play = true;
            while (true)
            {
                if (_update_loop_is_running)
                {
                    while (true)
                    {
                        DateTime start = DateTime.Now;
                        if (Play) Update_simulation();

                        if (_update_loop_should_stop)
                        {
                            _update_loop_should_stop = false;
                            _update_loop_is_running = false;
                            break;
                        }

                        int sleep = (int)Math.Max(0, update_time_step_in_ms - (DateTime.Now - start).TotalMilliseconds);
                        Thread.Sleep(sleep);
                    }
                }
                else Thread.Sleep(3);
            }
        }

        // --- Control Methods ---
        public void Randomize_positions(int nr_of_vehicles)
        {
            if (nr_of_vehicles < 1) nr_of_vehicles = 1;
            Stop_update_loop();
            Wait_for_update_loop_to_finish();

            lock (lock_update_object)
            {
                Populate(nr_of_vehicles);
                Start_update_loop();
            }
        }

        public void Set_seek_target(int x, int y)
        {
            Seek_target.X = x;
            Seek_target.Y = y;
        }

        public void Set_size(int w, int h)
        {
            lock (lock_update_object)
            {
                Width = w;
                Height = h;
            }
        }

        private void Stop_update_loop() => _update_loop_should_stop = true;

        private void Start_update_loop()
        { _update_loop_should_stop = false; _update_loop_is_running = true; }

        private void Wait_for_update_loop_to_finish()
        { while (_update_loop_is_running) Thread.Sleep(1); }

        // --- Rendering ---
        public void Render(Graphics g)
        {
            try
            {
                foreach (Vehicle v in vehicles_list) v.Render(g);
            }
            catch { /* Ignore collection sync issues during render */ }

            using (Pen pen = new Pen(Color.Black, 3))
            {
                float x = (float)Seek_target.X;
                float y = (float)Seek_target.Y;
                g.DrawLine(pen, x - 10, y - 10, x + 10, y + 10);
                g.DrawLine(pen, x + 10, y - 10, x - 10, y + 10);
            }
        }
    }
}