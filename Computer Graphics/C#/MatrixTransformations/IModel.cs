namespace MatrixTransformations
{
    public interface IModel
    {
        void Draw(Graphics g, List<Vector> projectedVertices, bool hideDebug = false);
    }
}