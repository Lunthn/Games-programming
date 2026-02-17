using System.Text;

namespace MatrixTransformations
{
    public partial class Form1 : Form
    {
        // Window dimensions
        private const int WIDTH = 800;

        private const int HEIGHT = 600;

        // Axes
        private AxisX x_axis;

        private AxisY y_axis;

        // Objects
        private Square square;

        private Square cyan_square;
        private Square orange_square;

        private float scalar;

        private float rotation;

        public Form1()
        {
            InitializeComponent();

            this.Width = WIDTH;
            this.Height = HEIGHT;
            this.DoubleBuffered = true;

            this.rotation = 20f;
            this.scalar = 1.5f;

            // Define axes
            x_axis = new AxisX(200);
            y_axis = new AxisY(200);

            // Create objects
            square = new Square(Color.Purple, 100);
            cyan_square = new Square(Color.Cyan, 100);
            orange_square = new Square(Color.Orange, 100);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw axes
            x_axis.Draw(e.Graphics, ViewportTransformation(x_axis.vertexbuffer));
            y_axis.Draw(e.Graphics, ViewportTransformation(y_axis.vertexbuffer));

            List<Vector> vb = new List<Vector>();

            var scaleMatrix = Matrix.ScaleMatrix(scalar);

            foreach (Vector v in square.vertexbuffer)
            {
                Vector v2 = scaleMatrix * v;
                vb.Add(v2);
            }

            cyan_square.Draw(e.Graphics, ViewportTransformation(vb));

            vb.Clear();

            var rotateMatrix = Matrix.RotateMatrix(rotation);

            foreach (Vector v in square.vertexbuffer)
            {
                Vector v2 = rotateMatrix * v;
                vb.Add(v2);
            }

            orange_square.Draw(e.Graphics, ViewportTransformation(vb));

            // Draw square
            square.Draw(e.Graphics, ViewportTransformation(square.vertexbuffer));
        }

        public static List<Vector> ViewportTransformation(List<Vector> vb)
        {
            float delta_x = WIDTH / 2;
            float delta_y = HEIGHT / 2;

            List<Vector> result = new List<Vector>();

            foreach (Vector v in vb)
            {
                float newX = v.x + delta_x;
                float newY = delta_y - v.y;
                result.Add(new Vector(newX, newY));
            }

            return result;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();
            else if (e.KeyCode == Keys.S && e.Shift)
            {
                this.scalar += 0.05f;
            }
            else if (e.KeyCode == Keys.S && !e.Shift)
            {
                this.scalar -= 0.05f;
            }
            else if (e.KeyCode == Keys.W && e.Shift)
            {
                this.rotation += 1f;
            }
            else if (e.KeyCode == Keys.W && !e.Shift)
            {
                this.rotation -= 1f;
            }

            Invalidate();
        }
    }
}