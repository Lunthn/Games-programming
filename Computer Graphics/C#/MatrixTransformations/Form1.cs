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
        private Square darkblue_square;

        private float scalar;
        private float rotation;
        private float translationX, translationY;

        public Form1()
        {
            InitializeComponent();

            this.Width = WIDTH;
            this.Height = HEIGHT;
            this.DoubleBuffered = true;

            this.rotation = 0f;
            this.scalar = 1f;
            this.translationX = 0f;
            this.translationY = 0f;

            // Define axes
            x_axis = new AxisX(200);
            y_axis = new AxisY(200);

            // Create objects
            square = new Square(Color.Purple, 100);
            cyan_square = new Square(Color.Cyan, 100);
            orange_square = new Square(Color.Orange, 100);
            darkblue_square = new Square(Color.DarkBlue, 100);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw axes
            x_axis.Draw(e.Graphics, ViewportTransformation(x_axis.vertexbuffer));
            y_axis.Draw(e.Graphics, ViewportTransformation(y_axis.vertexbuffer));

            List<Vector> vb = new List<Vector>();

            var scaleMatrix = Matrix.ScaleMatrix(scalar);
            var rotateMatrix = Matrix.RotateMatrix(rotation);
            var translationMatrix = Matrix.TranslateMatrix(translationX, translationY);

            foreach (Vector v in square.vertexbuffer)
            {
                Vector v2 = scaleMatrix * v;
                v2 = rotateMatrix * v2;
                v2 = translationMatrix * v2;
                vb.Add(v2);
            }

            square.Draw(e.Graphics, ViewportTransformation(vb));

            vb.Clear();

            scaleMatrix = Matrix.ScaleMatrix(1.5f);
            foreach (Vector v in square.vertexbuffer)
            {
                Vector v2 = scaleMatrix * v;
                vb.Add(v2);
            }

            cyan_square.Draw(e.Graphics, ViewportTransformation(vb));

            vb.Clear();

            rotateMatrix = Matrix.RotateMatrix(20f);
            foreach (Vector v in square.vertexbuffer)
            {
                Vector v2 = rotateMatrix * v;
                vb.Add(v2);
            }

            orange_square.Draw(e.Graphics, ViewportTransformation(vb));

            vb.Clear();
            translationMatrix = Matrix.TranslateMatrix(75f, -25);
            foreach (Vector v in square.vertexbuffer)
            {
                Vector v2 = translationMatrix * v;
                vb.Add(v2);
            }

            darkblue_square.Draw(e.Graphics, ViewportTransformation(vb));
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
            else if (e.KeyCode == Keys.W)
            {
                this.translationY += 5f;
            }
            else if (e.KeyCode == Keys.S)
            {
                this.translationY -= 5f;
            }
            else if (e.KeyCode == Keys.A)
            {
                this.translationX -= 5f;
            }
            else if (e.KeyCode == Keys.D)
            {
                this.translationX += 5f;
            }
            else if (e.KeyCode == Keys.Q)
            {
                this.rotation -= 5f;
            }
            else if (e.KeyCode == Keys.E)
            {
                this.rotation += 5f;
            }
            else if (e.KeyCode == Keys.Z)
            {
                this.scalar += 0.1f;
            }
            else if (e.KeyCode == Keys.X)
            {
                this.scalar -= 0.1f;
            }

            Invalidate();
        }
    }
}