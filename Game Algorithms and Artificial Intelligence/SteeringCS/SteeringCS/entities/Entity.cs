using System;
using System.Collections.Generic;
using System.Drawing;
using SteeringCS.behaviour;
using SteeringCS.behaviours;
using SteeringCS.states;
using SteeringCS.util;

namespace SteeringCS.entity
{
    public class Entity
    {
        public World MyWorld { get; set; }
        public Vector_2D Position { get; set; }
        public string Name { get; set; }
        public Vector_2D Direction { get; set; }
        public Vector_2D Velocity { get; set; }
        public double Mass { get; set; }
        public float Size { get; set; }
        public int DetectionRadius { get; set; } = 150;
        public virtual double MaxVelocity { get; set; }
        public virtual double MinVelocity { get; set; }
        public bool ShowDebugInfo { get; set; } = false;
        public Vector_2D AccelerationForRendering { get; private set; } = new Vector_2D();
        public IState CurrentState { get; set; }
        public List<Steering_Behaviour> Behaviours { get; private set; } = new List<Steering_Behaviour>();

        private readonly Seek_Behaviour _seekBehaviour;
        private readonly Wander_Behaviour _wanderBehaviour;
        private readonly Seperation_Behaviour _separationBehaviour;

        public Seek_Behaviour SeekBehaviour => _seekBehaviour;
        public Wander_Behaviour WanderBehaviour => _wanderBehaviour;
        public Seperation_Behaviour SeparationBehaviour => _separationBehaviour;

        public Vector_2D FinalTarget
        {
            get => _seekBehaviour.Target;
            set => _seekBehaviour.Target = value;
        }

        public Entity(World world, string name, Vector_2D pos, double mass, float size, Vector_2D vel = null)
        {
            MyWorld = world;
            Name = name;
            Position = pos.Clone();
            Velocity = vel?.Clone() ?? new Vector_2D();
            Direction = Velocity.Clone().Normalize();
            Mass = mass;
            Size = size;

            _seekBehaviour = new Seek_Behaviour(this);
            _wanderBehaviour = new Wander_Behaviour(this);
            _separationBehaviour = new Seperation_Behaviour(this, MyWorld.Entities, DetectionRadius);

            Behaviours.Add(_seekBehaviour);
            Behaviours.Add(_wanderBehaviour);
            Behaviours.Add(_separationBehaviour);

            ChangeState(new IdleState());
        }

        public void ChangeState(IState newState)
        {
            CurrentState?.Exit(this);
            CurrentState = newState;
            CurrentState?.Enter(this);
        }

        public virtual void UpdateSimulation(double timeElapsedMs)
        {
            CurrentState?.Execute(this);

            Vector_2D steeringForce = new Vector_2D();

            foreach (var behavior in Behaviours)
            {
                if (behavior.IsActive)
                {
                    steeringForce.Add(behavior.Calculate());
                }
            }

            steeringForce.Divide(Mass);
            AccelerationForRendering = steeringForce.Clone();

            Velocity.Multiply(MyWorld.InertiaPercentage / 100.0);
            Velocity.Add(steeringForce);

            Direction = Vector_2D.Normalize(Velocity);
            Velocity.Clamp_to_length(MinVelocity, MaxVelocity);

            Vector_2D frameMovement = Vector_2D.Multiply(Velocity, timeElapsedMs / 1000.0);
            Position.Add(frameMovement);
        }

        public override string ToString()
        {
            return $"{Name} Position:({Position.X:F1}, {Position.Y:F1}) Velocity:({Velocity.X:F1}, {Velocity.Y:F1}) Mass: {Mass:F1} Size: {Size:F1}";
        }

        public virtual void Render(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            float r = Size;
            RectangleF bodyRect = new RectangleF((float)Position.X - r, (float)Position.Y - r, r * 2, r * 2);

            using (Brush fillBrush = new SolidBrush(Color.FromArgb(80, 120, 50)))
                g.FillEllipse(fillBrush, bodyRect);

            using (Pen outlinePen = new Pen(Color.FromArgb(20, 40, 10), 2.5f))
                g.DrawEllipse(outlinePen, bodyRect);

            Vector_2D nub = Direction.Clone().Scale_to_Length(r);
            using (Pen nubPen = new Pen(Color.FromArgb(20, 40, 10), 2.5f) { EndCap = System.Drawing.Drawing2D.LineCap.Round })
                g.DrawLine(nubPen,
                    (float)Position.X, (float)Position.Y,
                    (float)(Position.X + nub.X), (float)(Position.Y + nub.Y));

            if (ShowDebugInfo) RenderDebug(g);
        }

        private void RenderDebug(Graphics g)
        {
            float dirAngle = (float)(Math.Atan2(Direction.Y, Direction.X) * 180.0 / Math.PI);
            float sweepAngle = 70f;
            float arcStartAngle = dirAngle - sweepAngle / 2f;

            using (Brush visionBrush = new SolidBrush(Color.FromArgb(25, 180, 60, 0)))
            {
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddPie(
                        (float)(Position.X - DetectionRadius), (float)(Position.Y - DetectionRadius),
                        DetectionRadius * 2, DetectionRadius * 2,
                        arcStartAngle, sweepAngle);
                    g.FillPath(visionBrush, path);
                }
            }

            using (Pen visionPen = new Pen(Color.FromArgb(60, 180, 60, 0), 1f))
                g.DrawArc(visionPen,
                    (float)(Position.X - DetectionRadius), (float)(Position.Y - DetectionRadius),
                    DetectionRadius * 2, DetectionRadius * 2,
                    arcStartAngle, sweepAngle);

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
            float yOffset = 20;

            g.DrawString(ToString(), new Font(font, FontStyle.Bold), Brushes.Black, (float)Position.X, (float)Position.Y + yOffset);
            yOffset += 15;

            g.DrawString($"{CurrentState.GetType().Name}", new Font(font, FontStyle.Bold), Brushes.Black, (float)Position.X, (float)Position.Y + yOffset);
            yOffset += 15;

            g.DrawString("Active Behaviours:", new Font(font, FontStyle.Bold), Brushes.Black, (float)Position.X, (float)Position.Y + yOffset);

            foreach (var behavior in Behaviours)
            {
                if (behavior.IsActive)
                {
                    yOffset += 15;
                    g.DrawString($"- {behavior.GetType().Name}", font, Brushes.DarkSlateGray, (float)Position.X, (float)Position.Y + yOffset);
                }
            }
        }
    }
}