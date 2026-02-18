using System.Collections.Generic;
using System.Drawing;

namespace MatrixTransformations
{
    public class AxisX
    {
        private int size;

        public List<Vector> vertexbuffer;

        public AxisX(int size = 100)
        {
            this.size = size;

            vertexbuffer = new List<Vector>();
            vertexbuffer.Add(new Vector(0, 0));
            vertexbuffer.Add(new Vector(size, 0));
        }

        public void Draw(Graphics g, List<Vector> vb)
        {
            Pen pen = new Pen(Color.Red, 2f);
            g.DrawLine(pen, vb[0].x, vb[0].y, vb[1].x, vb[1].y);
            Font font = new Font("Arial", 10);
            PointF p = new PointF(vb[1].x, vb[1].y);
            g.DrawString("x", font, Brushes.Red, p);
        }
    }
}