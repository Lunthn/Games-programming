using Xunit;
using System;
using MatrixTransformations;

namespace MatrixTransformations.Tests
{
    public class MatrixTests
    {
        private const float Precision = 0.0001f;

        [Fact]
        public void DefaultConstructor_ShouldInitializeToZero()
        {
            var matrix = new Matrix();
            var mat = matrix.GetMat();

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    if ((i == mat.GetLength(0) - 1 && j == mat.GetLength(1) - 1) ||
                        (i == mat.GetLength(0) - 2 && j == mat.GetLength(1) - 2))
                    {
                        Assert.Equal(1, mat[i, j]);
                    }
                    else
                    {
                        Assert.Equal(0, mat[i, j]);
                    }
                }
            }
        }

        [Fact]
        public void AdditionOperator_ShouldAddValuesCorrectly()
        {
            var m1 = Matrix.Identity();
            var m2 = Matrix.Identity();
            var result = m1 + m2;
            var mat = result.GetMat();

            Assert.Equal(2, mat[0, 0]);
            Assert.Equal(2, mat[1, 1]);
            Assert.Equal(2, mat[2, 2]);
            Assert.Equal(1, mat[3, 3]);
        }

        [Fact]
        public void SubtractionOperator_ShouldSubtractValuesCorrectly()
        {
            var m1 = Matrix.Identity();
            var m2 = Matrix.Identity();
            var result = m1 - m2;
            var mat = result.GetMat();

            Assert.Equal(0, mat[0, 0]);
            Assert.Equal(0, mat[1, 1]);
            Assert.Equal(0, mat[2, 2]);
            Assert.Equal(1, mat[3, 3]);
        }

        [Fact]
        public void MatrixMultiplication_Identity_ShouldReturnSameMatrix()
        {
            var m1 = new Matrix(1, 2, 3, 4);
            var identity = Matrix.Identity();
            
            var result = m1 * identity;
            var mat = result.GetMat();

            Assert.Equal(1, mat[0, 0]);
            Assert.Equal(2, mat[0, 1]);
            Assert.Equal(3, mat[1, 0]);
            Assert.Equal(4, mat[1, 1]);
        }

        [Fact]
        public void MatrixMultiplication_ShouldMultiplyValuesCorrectly()
        {
            var m1 = new Matrix(1, 2, 3, 4);
            var m2 = new Matrix(4, 3, 2, 1);
            
            var result = m1 * m2;
            var mat = result.GetMat();

            Assert.Equal(8, mat[0, 0]);
            Assert.Equal(5, mat[0, 1]);
            Assert.Equal(20, mat[1, 0]);
            Assert.Equal(13, mat[1, 1]);
        }

        [Fact]
        public void MatrixByFloatMultiplication_ShouldMultiplyValuesCorrectly()
        {
            var m1 = Matrix.Identity();
            var result = m1 * 5f;
            var mat = result.GetMat();

            for(int i = 0; i < mat.GetLength(0); i++)
            {
                for(int j = 0; j < mat.GetLength(1); j++)
                {
                    if(i == j)
                    {
                        Assert.Equal(5, mat[i, j]);
                    }
                    else
                    {
                        Assert.Equal(0, mat[i, j]);
                    }
                }
            }
        }

        [Fact]
        public void Identity_ShouldHaveOnesOnDiagonal()
        {
            var identity = Matrix.Identity();
            var mat = identity.GetMat();

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                Assert.Equal(1, mat[i, i]);
            }
        }

        [Fact]
        public void ScaleMatrix_ShouldApplyScale()
        {
            var scale = Matrix.ScaleMatrix(2.5f);
            var mat = scale.GetMat();

            Assert.Equal(2.5f, mat[0, 0]);
            Assert.Equal(2.5f, mat[1, 1]);
            Assert.Equal(2.5f, mat[2, 2]);
            Assert.Equal(1.0f, mat[3, 3]);
        }

        [Theory]
        [InlineData(90)]
        public void RotateMatrixX_ShouldRotateCorrectly(float degrees)
        {
            var matrix = Matrix.RotateMatrixX(degrees);
            var mat = matrix.GetMat();

            Assert.Equal(0, mat[1, 1], Precision);
            Assert.Equal(-1, mat[1, 2], Precision);
            Assert.Equal(1, mat[2, 1], Precision);
            Assert.Equal(0, mat[2, 2], Precision);
        }

        [Fact]
        public void RotateMatrixY_ShouldRotateCorrectly()
        {
            float degrees = 90f;
            var matrix = Matrix.RotateMatrixY(degrees);
            var mat = matrix.GetMat();

            Assert.Equal(0, mat[0, 0], Precision);
            Assert.Equal(1, mat[0, 2], Precision);
            Assert.Equal(-1, mat[2, 0], Precision);
            Assert.Equal(0, mat[2, 2], Precision);
        }

        [Fact]
        public void RotateMatrixZ_ShouldRotateCorrectly()
        {
            float degrees = 90f;
            var matrix = Matrix.RotateMatrixZ(degrees);
            var mat = matrix.GetMat();

            Assert.Equal(0, mat[0, 0], Precision);
            Assert.Equal(-1, mat[0, 1], Precision);
            Assert.Equal(1, mat[1, 0], Precision);
            Assert.Equal(0, mat[1, 1], Precision);
        }

        [Fact]
        public void TranslateMatrix_ReturnsTranslationMatrix()
        {
            var translation = new Vector(10, -5, 3); 
            var matrix = Matrix.TranslateMatrix(translation);
            var mat = matrix.GetMat();

            Assert.Equal(10, mat[0, 3]);
            Assert.Equal(-5, mat[1, 3]);
            Assert.Equal(3, mat[2, 3]);
            Assert.Equal(1, mat[3, 3]);
        }

        [Fact]
        public void ViewMatrix_ReturnsViewMatrix()
        {
            float r = 10f;
            float theta = 0f;
            float phi = 0f;

            var view = Matrix.ViewMatrix(r, theta, phi);
            var mat = view.GetMat();

            Assert.Equal(0, mat[0, 0], Precision);
            Assert.Equal(1, mat[0, 1], Precision);
            Assert.Equal(-10, mat[2, 3], Precision);
        }

        [Fact]
        public void ProjectMatrix_ReturnsProjectionMatrix()
        {
            float d = 100f;
            float vz = 200f;
            float expectedScale = -0.5f;
            
            var projection = Matrix.ProjectMatrix(d, vz);
            var mat = projection.GetMat();

            Assert.Equal(expectedScale, mat[0, 0], Precision);
            Assert.Equal(expectedScale, mat[1, 1], Precision);
            
            Assert.Equal(1, mat[2, 2]);
            Assert.Equal(1, mat[3, 3]);
        }
    }
}