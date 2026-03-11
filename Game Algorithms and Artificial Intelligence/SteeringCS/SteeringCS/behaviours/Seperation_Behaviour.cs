using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SteeringCS.entities;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace SteeringCS.behaviours
{
    public class Seperation_Behaviour : Steering_Behaviour
    {
        public List<Zombie> Neighbours { get; set; }
        private int _seperationRadius;
        private double _separationStrength;

        public Seperation_Behaviour(Zombie owner, List<Zombie> neighbours, int detectionRadius) : base(owner)
        {
            this.Neighbours = neighbours;
            this._seperationRadius = detectionRadius;
            this._separationStrength = 10.0;
        }

        public override Vector_2D Calculate()
        {
            Vector_2D force = new Vector_2D();

            foreach (var neighbor in Neighbours)
            {
                if (neighbor == this.Owner) continue;

                Vector_2D difference = Vector_2D.Subtract(Owner.Position, neighbor.Position);
                double distance = difference.Length();

                if (distance < _seperationRadius && distance > 0)
                {
                    difference.Normalize();

                    double weight = (_seperationRadius / distance);
                    weight = weight * weight;
                    difference.Multiply(weight * _separationStrength);
                    force.Add(difference);
                }
            }

            return force;
        }

        public override void Render_for_Debug(Graphics g)
        {
            using (Pen pen = new Pen(Color.FromArgb(128, Color.Black), 2))
            {
                foreach (var neighbor in Neighbours)
                {
                    if (neighbor == this.Owner) continue;

                    double distance = Vector_2D.Distance(Owner.Position, neighbor.Position);
                    if (distance < _seperationRadius && distance > 0)
                    {
                        g.DrawLine(pen,
                            (float)Owner.Position.X, (float)Owner.Position.Y,
                            (float)neighbor.Position.X, (float)neighbor.Position.Y);
                    }
                }
            }
        }
    }
}