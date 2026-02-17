using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixTransformations
{
    public class Matrix
    {
        private float[,] mat = new float[2, 2];

        public Matrix() : this(0, 0, 0, 0)
        {
        }

        public Matrix(float m11, float m12,
                      float m21, float m22)
        {
            mat[0, 0] = m11; mat[0, 1] = m12;
            mat[1, 0] = m21; mat[1, 1] = m22;
        }

        public Matrix(Vector v) : this(v.x, 0, v.y, 0)
        {
        }

        public Vector ToVector()
        {
            return new Vector(mat[0, 0], mat[1, 0]);
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            Matrix result = new Matrix();

            for (int i = 0; i < m1.mat.GetLength(0); i++)
            {
                for (int j = 0; j < m1.mat.GetLength(1); j++)
                {
                    result.mat[i, j] = m1.mat[i, j] + m2.mat[i, j];
                }
            }

            return result;
        }

        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            Matrix result = new Matrix();

            for (int i = 0; i < m1.mat.GetLength(0); i++)
            {
                for (int j = 0; j < m1.mat.GetLength(1); j++)
                {
                    result.mat[i, j] = m1.mat[i, j] - m2.mat[i, j];
                }
            }

            return result;
        }

        public static Matrix operator *(Matrix m1, float f)
        {
            Matrix result = new Matrix();

            for (int i = 0; i < m1.mat.GetLength(0); i++)
            {
                for (int j = 0; j < m1.mat.GetLength(1); j++)
                {
                    result.mat[i, j] = m1.mat[i, j] * f;
                }
            }

            return result;
        }

        public static Matrix operator *(float f, Matrix m1)
        {
            return m1 * f;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            int size = m1.mat.GetLength(0);
            Matrix result = new Matrix();

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < size; k++)
                    {
                        sum += m1.mat[i, k] * m2.mat[k, j];
                    }
                    result.mat[i, j] = sum;
                }
            }
            return result;
        }

        public static Vector operator *(Matrix m1, Vector v)
        {
            Matrix result = m1 * new Matrix(v);
            return result.ToVector();
        }

        public static Matrix Identity()
        {
            Matrix result = new Matrix();

            for (int i = 0; i < result.mat.GetLength(0); i++)
            {
                result.mat[i, i] = 1;
            }

            return result;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                result.Append("[ ");
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    result.AppendFormat("{0,2} ", mat[i, j]);
                }
                result.Append("]\n");
            }

            return result.ToString();
        }

        public static Matrix ScaleMatrix(float scalar)
        {
            return scalar * Identity();
        }

        public static Matrix RotateMatrix(float degrees)
        {
            double rad = (Math.PI / 180) * degrees;

            float sin = (float)Math.Sin(rad);
            float cos = (float)Math.Cos(rad);

            return new Matrix(cos, -sin, sin, cos);
        }
    }
}