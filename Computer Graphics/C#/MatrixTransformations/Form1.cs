using System.ComponentModel.Design;
using System.Runtime.InteropServices;
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
        private AxisZ z_axis;

        // Objects
        private Cube cube;

        // Values
        private float d = 800;

        private float r = 10;
        private float theta = -100;
        private float phi = -10;

        public Form1()
        {
            InitializeComponent();

            this.Width = WIDTH;
            this.Height = HEIGHT;
            this.DoubleBuffered = true;

            // Define axes
            x_axis = new AxisX(3);
            y_axis = new AxisY(3);
            z_axis = new AxisZ(3);

            // Create objects
            cube = new Cube(Color.Black);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw axes
            x_axis.Draw(e.Graphics, ViewportTransformation(x_axis.vertexbuffer));
            y_axis.Draw(e.Graphics, ViewportTransformation(y_axis.vertexbuffer));
            z_axis.Draw(e.Graphics, ViewportTransformation(z_axis.vertexbuffer));

            var viewMatrix = Matrix.ViewMatrix(r, theta, phi);
            List<Vector> vb = new List<Vector>();

            foreach (Vector v in cube.vertexbuffer)
            {
                Vector v2 = viewMatrix * v;
                var projectMatrix = Matrix.ProjectMatrix(d, v.z);
                Vector v3 = projectMatrix * v2;

                vb.Add(v3);
            }

            cube.Draw(e.Graphics, ViewportTransformation(vb));
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

            Invalidate();
        }
    }
}