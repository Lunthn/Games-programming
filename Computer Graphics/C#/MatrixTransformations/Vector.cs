using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixTransformations
{
    public class Vector
    {
        public float x, y, z, w;

        public Vector(float x, float y) : this(x, y, 0)
        {
        }

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = 1;
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static Vector operator *(Vector v1, Vector v2)
        {
            return new Vector(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        public static Vector operator *(float factor, Vector v1)
        {
            return new Vector(v1.x * factor, v1.y * factor, v1.z * factor);
        }

        public override string ToString()
        {
            return  x + "\n" +
                    y + "\n" +
                    z + "\n" +
                    w; 
        }
    }
}