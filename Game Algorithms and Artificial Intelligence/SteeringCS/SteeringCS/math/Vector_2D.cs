using SteeringCS.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace SteeringCS
{
    public class Vector_2D
    {
        // convention:
        //              STATIC methods create a new Vector_2D
        //              NON-static method modifies this (and returns this as well, for chaining)

        public double X { get; set; }
        public double Y { get; set; }

        public Vector_2D() : this(0, 0)
        {
        }

        public Vector_2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector_2D(Vector_2D v)
        {
            X = v.X;
            Y = v.Y;
        }

        public void Set(Vector_2D v)
        {
            this.X = v.X;
            this.Y = v.Y;
        }

        public void Set(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector_2D Clone()
        {
            return new Vector_2D(this);
        }

        public override string ToString()
        {
            return "(" + X.ToString("F2") + ", " + Y.ToString("F2") + ", L = " + this.Length() + ")";
        }

        public override bool Equals(object obj)
        {
            try
            {
                Vector_2D that = (Vector_2D)obj;
                return (this.X == that.X) && (this.Y == that.Y);
            }
            catch
            {
                return false;
            }
        }

        // vector2d could be used as a key in a Dictionary (e.g. for spatial partitioning)
        public override int GetHashCode()
        { return (37 * (int)X + 113 * (int)Y); }

        public static Vector_2D Add(Vector_2D v1, Vector_2D v2)
        {
            Vector_2D v3 = v1.Clone();
            v3.Add(v2);
            return v3;
        }

        public Vector_2D Add(Vector_2D v)
        {
            if (v == null)
            {
                throw new NullReferenceException("wooops!");
            }
            this.X += v.X;
            this.Y += v.Y;
            return this;
        }

        public Vector_2D Add(double x, double y)
        {
            this.X += x;
            this.Y += y;
            return this;
        }

        public Vector_2D Subtract(double x, double y)
        {
            this.X -= x;
            this.Y -= y;
            return this;
        }

        public static Vector_2D Subtract(Vector_2D v1, Vector_2D v2)
        {
            Vector_2D v3 = v1.Clone();
            v3.Subtract(v2);
            return v3;
        }

        public Vector_2D Subtract(Vector_2D v)
        {
            if (v == null)
            {
                throw new NullReferenceException("wooops!");
            }
            this.X -= v.X;
            this.Y -= v.Y;
            return this;
        }

        public static Vector_2D Multiply(Vector_2D v1, double value)
        {
            Vector_2D v2 = v1.Clone();
            v2.Multiply(value);
            return v2;
        }

        public Vector_2D Multiply(double value)
        {
            if (Double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new NullReferenceException("wooops!");
            }
            this.X *= value;
            this.Y *= value;
            return this;
        }

        public static Vector_2D Divide(Vector_2D v1, double value)
        {
            Vector_2D v3 = v1.Clone();
            v3.Divide(value);
            return v3;
        }

        public Vector_2D Divide(double value)
        {
            if (value == 0 || Double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new NullReferenceException("wooops!");
            }

            this.X /= value;
            this.Y /= value;
            return this;
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public bool Is_length_zero()
        {
            return X == 0 && Y == 0;
        }

        public static Vector_2D Normalize(Vector_2D v)
        {
            Vector_2D v2 = v.Clone();
            v2.Normalize();
            return v2;
        }

        public Vector_2D Normalize()
        {
            double length = this.Length();
            if (length != 0)
            {
                return Divide(length);
            }
            else
            {
                // maybe throw exception here? Is this something that we want to happen?
                return this;
            }
        }

        public Vector_2D Scale_to_Length(double length)
        {
            if (Is_length_zero())
            {
                return this;
                //       throw new NotImplementedException("woopsie!");
            }
            this.Normalize();
            this.Multiply(length);
            return this;
        }

        public Vector_2D Clamp_to_length(double min_length, double max_length)
        {
            double length = Length();
            if (length > max_length)
            {
                Normalize();
                Multiply(max_length);
            }
            else if (length < min_length)
            {
                Normalize();
                Multiply(min_length);
            }
            return this;
        }

        public static double Distance(Vector_2D v1, Vector_2D v2)
        {
            // this is the length of the difference of v1 and v2:
            //
            //      Vector2D diff = Vector2D.Subtract(v1, v2);
            //      return diff.Length();
            //
            // but this creates a new Vector2d (in substract)
            // so it's faster to just do the calculation here:
            return Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));
        }

        public static double ManhattanDistance(Vector_2D v1, Vector_2D v2)
        {
            return Math.Abs(v1.X - v2.X) + Math.Abs(v1.Y - v2.Y);
        }

        private static readonly double Degrees_to_radians_factor = Math.PI / 180;

        public Vector_2D Rotate_degrees(double degrees)
        {
            double radians = degrees * Degrees_to_radians_factor;
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            double x = this.X;
            double y = this.Y;
            this.X = ca * x - sa * y;
            this.Y = sa * x + ca * y;
            return this;
        }

        public Vector_2D Rotate_90_degrees_counter_clockwise()
        {
            double temp = this.X;
            this.X = -this.Y;
            this.Y = temp;
            return this;
        }

        public Vector_2D Rotate_90_degrees_clockwise()
        {
            double temp = this.X;
            this.X = this.Y;
            this.Y = -temp;
            return this;
        }
    }
}