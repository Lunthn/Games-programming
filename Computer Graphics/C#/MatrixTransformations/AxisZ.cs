using System.Collections.Generic;
using System.Drawing;

namespace MatrixTransformations
{
    public class AxisZ
    {
        private int size;

        public List<Vector> vertexbuffer;

        public AxisZ(int size = 100)
        {
            this.size = size;

            vertexbuffer = new List<Vector>();
            vertexbuffer.Add(new Vector(0, 0));
            vertexbuffer.Add(new Vector(0, 0, size));
        }

        public void Draw(Graphics g, List<Vector> vb)
        {
            Pen pen = new Pen(Color.Blue, 2f);
            g.DrawLine(pen, vb[0].x, vb[0].y, vb[1].x, vb[1].y);
            Font font = new Font("Arial", 10);
            PointF p = new PointF(vb[1].x, vb[1].y);
            g.DrawString("z", font, Brushes.Blue, p);
        }
    }
}