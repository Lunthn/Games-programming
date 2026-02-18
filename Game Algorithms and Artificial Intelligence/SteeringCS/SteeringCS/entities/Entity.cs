using System;
using System.Collections.Generic;
using System.Drawing;
using SteeringCS.behaviour;
using SteeringCS.util;

namespace SteeringCS.entity
{
    public class Entity
    {
        public World MyWorld { get; set; }
        public Vector_2D Position { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public Vector_2D Direction { get; set; }
        public Vector_2D Velocity { get; set; }
        public virtual double MaxVelocity { get; set; }
        public virtual double MinVelocity { get; set; }
        public bool ShowDebugInfo { get; set; }
        public List<Steering_Behaviour> Behaviours { get; private set; }
        public Vector_2D AccelerationForRendering { get; private set; }

        private readonly Arrive_Behaviour _arriveBehaviour;

        public Vector_2D ArriveTarget
        {
            get => _arriveBehaviour.Target;
            set => _arriveBehaviour.Target = value;
        }

        public Entity(World world, string name, Vector_2D pos, Vector_2D vel = null)
        {
            MyWorld = world;
            Name = name;
            Position = pos.Clone();
            Velocity = vel?.Clone() ?? new Vector_2D();
            Direction = Velocity.Clone().Normalize();

            IsActive = true;
            AccelerationForRendering = new Vector_2D();
            Behaviours = new List<Steering_Behaviour>();

            _arriveBehaviour = new Arrive_Behaviour(this);
            Behaviours.Add(_arriveBehaviour);
        }

        public virtual void UpdateSimulation(double timeElapsedMs)
        {
            if (!IsActive) return;

            Vector_2D steeringForce = new Vector_2D();

            foreach (var behavior in Behaviours)
            {
                steeringForce.Add(behavior.Calculate());
            }

            const double mass = 30.0;
            steeringForce.Divide(mass);
            AccelerationForRendering = steeringForce.Clone();

            Velocity.Multiply(MyWorld.InertiaPercentage / 100.0);
            Velocity.Add(steeringForce);

            Direction = Vector_2D.Normalize(Velocity);
            Velocity.Clamp_to_length(MinVelocity, MaxVelocity);

            Vector_2D frameMovement = Vector_2D.Multiply(Velocity, timeElapsedMs / 1000.0);
            Position.Add(frameMovement);
        }

        public virtual void Render(Graphics g)
        {
            if (!IsActive) return;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Vector_2D head = Direction.Clone().Scale_to_Length(30);
            Vector_2D sideOffset = Direction.Clone().Rotate_90_degrees_clockwise().Scale_to_Length(15);
            Vector_2D backOffset = Direction.Clone().Scale_to_Length(-15);

            PointF[] bodyPoints =
            {
                new PointF((float)(Position.X + head.X), (float)(Position.Y + head.Y)),
                new PointF((float)(Position.X + sideOffset.X + backOffset.X), (float)(Position.Y + sideOffset.Y + backOffset.Y)),
                new PointF((float)(Position.X - sideOffset.X + backOffset.X), (float)(Position.Y - sideOffset.Y + backOffset.Y))
            };
            Color primaryColor = Color.FromArgb(70, 130, 180);
            Color outlineColor = Color.FromArgb(40, 44, 52);

            using (Brush fillBrush = new SolidBrush(primaryColor))
            {
                g.FillPolygon(fillBrush, bodyPoints);
            }

            using (Pen softPen = new Pen(outlineColor, 3))
            {
                softPen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                g.DrawPolygon(softPen, bodyPoints);
            }

            if (ShowDebugInfo) RenderDebug(g);
        }

        private void RenderDebug(Graphics g)
        {
            foreach (var sb in Behaviours) sb.Render_for_Debug(g);

            const double velScale = 0.3;
            g.DrawLine(new Pen(Color.Red, 2),
                (int)Position.X, (int)Position.Y,
                (int)(Position.X + Velocity.X * velScale), (int)(Position.Y + Velocity.Y * velScale));

            const double accScale = 10.0;
            g.DrawLine(new Pen(Color.Orange, 2),
                (int)Position.X, (int)Position.Y,
                (int)(Position.X + AccelerationForRendering.X * accScale), (int)(Position.Y + AccelerationForRendering.Y * accScale));

            Font font = new Font("Arial", 8);
            float yOffset = 40;
            g.DrawString("Active Behaviours:", new Font(font, FontStyle.Bold), Brushes.Black, (float)Position.X, (float)Position.Y + yOffset);

            foreach (var behavior in Behaviours)
            {
                yOffset += 15;
                g.DrawString($"- {behavior.GetType().Name}", font, Brushes.DarkSlateGray, (float)Position.X, (float)Position.Y + yOffset);
            }
        }
    }
}