namespace MatrixTransformations
{
    public class TransformState
    {
        public float PosX;
        public float PosY;
        public float PosZ;
        public float RotX;
        public float RotY;
        public float RotZ;
        public float Scale = 1f;

        public void Reset()
        {
            PosX = 0f;
            PosY = 0f;
            PosZ = 0f;

            RotX = 0f;
            RotY = 0f;
            RotZ = 0f;

            Scale = 1f;
        }
    }
}