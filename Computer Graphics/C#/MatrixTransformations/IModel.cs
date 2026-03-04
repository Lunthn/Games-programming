namespace MatrixTransformations
{
    public interface IModel
    {
        public Color Color { get; set; }
        public Color LabelColor { get; set; }
        public List<Vector> VertexBuffer { get; }

        void Draw(Graphics g, List<Vector> projectedVertices, bool hideDebug = false);
    }
}