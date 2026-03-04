namespace MatrixTransformations
{
    public class Matrix
    {
        float[,] mat = new float[4, 4];
        public float[,] GetMat() => mat;

        public Matrix() : this(0, 0, 0, 0)
        {
        }

        public Matrix(float m11, float m12, float m21, float m22) : this(m11, m12, 0, m21, m22, 0, 0, 0, 1)
        {
        }

        public Matrix(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
        {
            mat[0, 0] = m11; mat[0, 1] = m12; mat[0, 2] = m13; mat[0, 3] = 0;
            mat[1, 0] = m21; mat[1, 1] = m22; mat[1, 2] = m23; mat[1, 3] = 0;
            mat[2, 0] = m31; mat[2, 1] = m32; mat[2, 2] = m33; mat[2, 3] = 0;
            mat[3, 0] = 0;      mat[3, 1] = 0;      mat[3, 2] = 0;      mat[3, 3] = 1;
        }

        public Matrix(Vector v)
        {
            mat[0, 0] = v.x; mat[0, 1] = 0; mat[0, 2] = 0; mat[0, 3] = 0;
            mat[1, 0] = v.y; mat[1, 1] = 0; mat[1, 2] = 0; mat[1, 3] = 0;
            mat[2, 0] = v.z; mat[2, 1] = 0; mat[2, 2] = 0; mat[2, 3] = 0;
            mat[3, 0] = v.w; mat[3, 1] = 0; mat[3, 2] = 0; mat[3, 3] = 0;
        }

        public Vector ToVector()
        {
            return new Vector(mat[0, 0], mat[1, 0], mat[2, 0]);
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            var tempMat = new Matrix();
            float[,] mat = tempMat.mat;

            for (int i = 0; i < m1.mat.GetLength(1); i++)
            {
                for (int j = 0; j < m1.mat.GetLength(0); j++)
                {
                    mat[i, j] = m1.mat[i, j] + m2.mat[i, j];
                }
            }

            return new Matrix(
                mat[0, 0], mat[0, 1], mat[0,2], 
                mat[1, 0], mat[1, 1], mat[1,2],
                mat[2,0], mat[2, 1], mat[2, 2]);
        }

        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            var tempMat = new Matrix();
            float[,] mat = tempMat.mat;

            for (int i = 0; i < m1.mat.GetLength(1); i++)
            {
                for (int j = 0; j < m1.mat.GetLength(0); j++)
                {
                    tempMat.mat[i, j] = m1.mat[i, j] - m2.mat[i, j];
                }
            }

            return new Matrix(
                mat[0, 0], mat[0, 1], mat[0, 2],
                mat[1, 0], mat[1, 1], mat[1, 2],
                mat[2, 0], mat[2, 1], mat[2, 2]);
        }

        public static Matrix operator *(Matrix m1, float f)
        {
            var tempMat = new Matrix();

            for (int i = 0; i < m1.mat.GetLength(1); i++)
            {
                for (int j = 0; j < m1.mat.GetLength(0); j++)
                {
                    tempMat.mat[i, j] = m1.mat[i, j] * f;
                }
            }

            return tempMat;
        }

        public static Matrix operator *(float f, Matrix m1)
        {
            return m1 * f;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            int rows = m1.mat.GetLength(0);
            int cols = m2.mat.GetLength(1);
            int sharedLength = m1.mat.GetLength(1);

            var tempMat = new Matrix();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    float sum = 0;

                    for (int k = 0; k < sharedLength; k++)
                    {
                        sum += m1.mat[r, k] * m2.mat[k, c];
                    }

                    tempMat.mat[r, c] = sum;
                }
            }

            return tempMat;
        }

        public static Vector operator *(Matrix m1, Vector v)
        {
            var m2 = new Matrix(v);

            var resultMat = m1 * m2;

            return resultMat.ToVector();
        }

        public static Matrix Identity()
        {
            Matrix result = new Matrix();

            for(int i = 0; i < result.mat.GetLength(0); i++)
            {
                result.mat[i,i] = 1;
            }

            return result;
        }

        public static Matrix ScaleMatrix(float s)
        {
            Matrix result = Matrix.Identity();

            for(int i = 0; i < result.mat.GetLength(0) - 1; i++)
            {
                result.mat[i,i] = s;
            }

            return result;
        }

        public static Matrix RotateMatrixX(float degrees)
        {
            double theta = (Math.PI / 180) * degrees;

            float sin = (float)Math.Sin(theta);
            float cos = (float)Math.Cos(theta);

            Matrix result = Identity();

            result.mat[1, 1] = cos;
            result.mat[1, 2] = -sin;
            result.mat[2, 1] = sin;
            result.mat[2, 2] = cos;

            return result;
        }

        public static Matrix RotateMatrixY(float degrees)
        {
            double theta = (Math.PI / 180) * degrees;

            float sin = (float)Math.Sin(theta);
            float cos = (float)Math.Cos(theta);

            Matrix result = Identity();

            result.mat[0, 0] = cos;
            result.mat[0, 2] = sin;
            result.mat[2, 0] = -sin;
            result.mat[2, 2] = cos;

            return result;
        }

        public static Matrix RotateMatrixZ(float degrees)
        {
            double theta = (Math.PI / 180) * degrees;

            float sin = (float)Math.Sin(theta);
            float cos = (float)Math.Cos(theta);

            Matrix result = Identity();

            result.mat[0, 0] = cos;
            result.mat[0, 1] = -sin;
            result.mat[1, 0] = sin;
            result.mat[1, 1] = cos;

            return result;
        }

        public static Matrix TranslateMatrix(Vector t)
        {
            var tempMat = Identity();
            tempMat.mat[0, 3] = t.x;
            tempMat.mat[1, 3] = t.y;
            tempMat.mat[2, 3] = t.z;
            return tempMat;
        }

        public static Matrix ViewMatrix(float r, float theta, float phi)
        {
            Matrix result = Identity();

            double theta_rad = (Math.PI / 180) * theta;
            double phi_rad = (Math.PI / 180) * phi;

            float sin_theta = (float)Math.Sin(theta_rad);
            float sin_phi = (float)Math.Sin(phi_rad);
            float cos_theta = (float)Math.Cos(theta_rad);
            float cos_phi = (float)Math.Cos(phi_rad);

            result.mat[0, 0] = -sin_theta;
            result.mat[0, 1] = cos_theta;

            result.mat[1, 0] = -cos_theta * cos_phi;
            result.mat[1, 1] = -cos_phi * sin_theta;
            result.mat[1, 2] = sin_phi;

            result.mat[2, 0] = cos_theta * sin_phi;
            result.mat[2, 1] = sin_theta * sin_phi;
            result.mat[2, 2] = cos_phi;
            result.mat[2, 3] = -r;

            return result;
        }

        public static Matrix ProjectMatrix(float d, float v_z)
        {
            Matrix result = Identity();

            result.mat[0, 0] = -(d / v_z);
            result.mat[1, 1] = -(d / v_z);

            return result;
        }

        public override string ToString()
        {
            var s = "";

            for (int i = 0; i < mat.GetLength(1); i++)
            {
                for (int j = 0; j < mat.GetLength(0); j++)
                {
                    s += mat[i, j] + " ";
                }

                s += "\n";
            }

            return s;
        }
    }
}