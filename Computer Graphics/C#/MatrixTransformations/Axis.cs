using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixTransformations
{
    public class Axis
    {
        private string label;
        private Color color;
        public List<Vector> vertexbuffer;

        public Axis(int x, int y, int z, string label, Color color)
        {
            this.label = label;
            this.color = color;

            vertexbuffer = new List<Vector>();
            vertexbuffer.Add(new Vector(0, 0, 0));
            vertexbuffer.Add(new Vector(x, y, z));
        }

        public void Draw(Graphics g, List<Vector> vb)
        {
            using (Pen pen = new Pen(color, 2f))
            using (Font font = new Font("Arial", 10))
            using (SolidBrush brush = new SolidBrush(color))
            {
                g.DrawLine(pen, vb[0].x, vb[0].y, vb[1].x, vb[1].y);
                PointF p = new PointF(vb[1].x, vb[1].y);
                g.DrawString(label, font, brush, p);
            }
        }
    }
}