using Xunit;
using MatrixTransformations;

namespace MatrixTransformations.Tests
{
    public class VectorTests
    {
        private const float Precision = 0.0001f;

        private static void AssertVectorEqual(Vector expected, Vector actual)
        {
            Assert.Equal(expected.x, actual.x, Precision);
            Assert.Equal(expected.y, actual.y, Precision);
            Assert.Equal(expected.z, actual.z, Precision);
        }

        [Fact]
        public void Constructor3D_SetsXYZAndW()
        {
            var v = new Vector(1f, 2f, 3f);

            Assert.Equal(1f, v.x);
            Assert.Equal(2f, v.y);
            Assert.Equal(3f, v.z);
            Assert.Equal(1f, v.w);
        }

        [Fact]
        public void Constructor2D_ZIsZero()
        {
            var v = new Vector(1f, 2f);

            Assert.Equal(0f, v.z);
        }

        [Fact]
        public void Add_TwoVectors_ReturnsSum()
        {
            var result = new Vector(1f, 2f, 3f) + new Vector(4f, 5f, 6f);

            AssertVectorEqual(new Vector(5f, 7f, 9f), result);
        }

        [Fact]
        public void Subtract_TwoVectors_ReturnsDifference()
        {
            var result = new Vector(5f, 7f, 9f) - new Vector(1f, 2f, 3f);

            AssertVectorEqual(new Vector(4f, 5f, 6f), result);
        }

        [Fact]
        public void Multiply_TwoVectors_ReturnsCorrectVector()
        {
            var result = new Vector(2f, 3f, 4f) * new Vector(5f, 6f, 7f);

            AssertVectorEqual(new Vector(10f, 18f, 28f), result);
        }

        [Fact]
        public void Multiply_VectorByFloat_ReturnsCorrectVectorr()
        {
            var result = 3f * new Vector(1f, 2f, 3f);

            AssertVectorEqual(new Vector(3f, 6f, 9f), result);
        }
    }
}