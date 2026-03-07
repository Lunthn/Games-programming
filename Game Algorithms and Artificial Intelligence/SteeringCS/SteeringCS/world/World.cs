using SteeringCS.entity;
using SteeringCS.util;
using SteeringCS.world;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace SteeringCS
{
    public class World : My_base_thread
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsPlaying { get; set; }
        public Vector_2D FinalTarget { get; private set; }

        public List<Entity> Entities { get; private set; }
        public List<WorldObject> Objects { get; private set; }

        private readonly object _syncLock = new object();
        private double _updateTimeStepMs;
        private bool _shouldStopLoop = false;
        private bool _isRunning = true;

        private int _inertiaPercentage = 97;

        public int InertiaPercentage
        {
            get => _inertiaPercentage;
            set => _inertiaPercentage = Math.Max(0, Math.Min(100, value));
        }

        private double _maxVelocity = 500;

        public double MaxVelocity
        {
            get => _maxVelocity;
            set
            {
                if (value < _minVelocity) return;
                _maxVelocity = value;
                Entities.ForEach(v => v.MaxVelocity = value);
            }
        }

        private double _minVelocity = 1;

        public double MinVelocity
        {
            get => _minVelocity;
            set
            {
                if (value > _maxVelocity || value < 0) return;
                _minVelocity = value;
                Entities.ForEach(v => v.MinVelocity = value);
            }
        }

        private bool _showDebugInfo = false;

        public bool ShowDebugInfo
        {
            get => _showDebugInfo;
            set
            {
                _showDebugInfo = value;
                Entities.ForEach(v => v.ShowDebugInfo = value);
            }
        }

        public World(int width, int height, int updateTimeStep)
        {
            Set_thread_as_background(true);
            _updateTimeStepMs = updateTimeStep;
            Width = width;
            Height = height;

            Entities = new List<Entity>();
            Objects = new List<WorldObject>();
        }

        private void BuildEntities(int count)
        {
            lock (_syncLock)
            {
                Entities.Clear();
                Random rng = new Random();

                for (int i = 1; i <= count; i++)
                {
                    Vector_2D pos = new Vector_2D(Width * rng.NextDouble(), Height * rng.NextDouble());

                    double mass = (rng.NextDouble() * 25 + 15);
                    float size = (float)(mass / 2);

                    Vector_2D vel = new Vector_2D(1, 0).Multiply(100);
                    vel.Rotate_degrees(rng.NextDouble() * 90 - 45);

                    Entity v = new Entity(this, "Zombie " + i, pos, mass, size, vel);
                    Entities.Add(v);
                }

                MinVelocity = _minVelocity;
                MaxVelocity = _maxVelocity;
                ShowDebugInfo = _showDebugInfo;
            }
        }

        private void BuildObjects(int count)
        {
            lock (_syncLock)
            {
                Objects.Clear();
                Random rng = new Random();
                for (int i = 0; i < count; i++)
                {
                    float radius = (float)Math.Floor(rng.NextDouble() * 20 + 5);
                    Vector_2D position = new Vector_2D(Width * rng.NextDouble(), Height * rng.NextDouble());
                    Objects.Add(new BushObject(position, radius));
                }
            }
        }

        public void Populate(int vehicleCount, int wallCount)
        {
            BuildEntities(vehicleCount);
            BuildObjects(wallCount);
        }

        public void UpdateSimulation()
        {
            lock (_syncLock)
            {
                foreach (Entity v in Entities) v.UpdateSimulation(_updateTimeStepMs);
            }
        }

        public override void Run_thread()
        {
            IsPlaying = true;
            while (true)
            {
                if (_isRunning)
                {
                    DateTime start = DateTime.Now;

                    if (IsPlaying) UpdateSimulation();

                    if (_shouldStopLoop)
                    {
                        _shouldStopLoop = false;
                        _isRunning = false;
                    }

                    int elapsed = (int)(DateTime.Now - start).TotalMilliseconds;
                    int sleepTime = Math.Max(0, (int)_updateTimeStepMs - elapsed);
                    Thread.Sleep(sleepTime);
                }
                else
                {
                    Thread.Sleep(3);
                }
            }
        }

        public void ResetPositions(int vehicleCount)
        {
            int count = Math.Max(1, vehicleCount);
            StopUpdateLoop();
            WaitForLoopToStop();

            lock (_syncLock)
            {
                BuildEntities(vehicleCount);
                StartUpdateLoop();
            }
        }

        public void SetTarget(int x, int y)
        {
            if (FinalTarget == null)
            {
                FinalTarget = new Vector_2D(0, 0);
            }

            FinalTarget.X = x;
            FinalTarget.Y = y;

            foreach (Entity v in Entities) v.FinalTarget = FinalTarget;
        }

        public void SetWorldSize(int w, int h)
        {
            lock (_syncLock)
            {
                Width = w;
                Height = h;
            }
        }

        public void StopUpdateLoop() => _shouldStopLoop = true;

        public void StartUpdateLoop()
        {
            _shouldStopLoop = false;
            _isRunning = true;
        }

        private void WaitForLoopToStop()
        {
            while (_isRunning) Thread.Sleep(1);
        }

        public void Render(Graphics g)
        {
            try
            {
                foreach (Entity v in Entities) v.Render(g);
                foreach (WorldObject w in Objects) w.Render(g);
            }
            catch { /* Ignore collection sync issues during render */ }

            if (FinalTarget != null)
            {
                using (Pen pen = new Pen(Color.Black, 2))
                {
                    float x = (float)FinalTarget.X;
                    float y = (float)FinalTarget.Y;
                    g.DrawLine(pen, x - 5, y - 5, x + 5, y + 5);
                    g.DrawLine(pen, x + 5, y - 5, x - 5, y + 5);
                }
            }
        }
    }
}