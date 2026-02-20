using System.CodeDom;
using System.ComponentModel.Design;
using System.DirectoryServices;
using System.Reflection.Metadata.Ecma335;
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
        private float posX = 0;
        private float posY = 0;
        private float posZ = 0;
        private float rotX = 0;
        private float rotY = 0;
        private float rotZ = 0;
        private float scale = 1;
        private int phase = 0;
        private bool animationIsPlaying = false;

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
            cube = new Cube(Color.Purple);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            x_axis.Draw(e.Graphics, ViewingPipeline(x_axis.vertexbuffer));
            y_axis.Draw(e.Graphics, ViewingPipeline(y_axis.vertexbuffer));
            z_axis.Draw(e.Graphics, ViewingPipeline(z_axis.vertexbuffer));

            cube.Draw(e.Graphics, ViewingPipeline(TransformCube(cube.vertexbuffer)));

            DrawInfo(e.Graphics);
        }

        public List<Vector> TransformCube(List<Vector> vertexbuffer)
        {
            var result = new List<Vector>();

            var scaleMatrix = Matrix.ScaleMatrix(scale);
            var rotX = Matrix.RotateMatrixX(this.rotX);
            var rotY = Matrix.RotateMatrixY(this.rotY);
            var rotZ = Matrix.RotateMatrixZ(this.rotZ);
            var translateMatrix = Matrix.TranslateMatrix(new Vector(posX, posY, posZ));

            var worldMatrix = translateMatrix * rotZ * rotY * rotX * scaleMatrix;

            foreach (Vector v in vertexbuffer)
            {
                Vector v2 = worldMatrix * v;
                result.Add(v2);
            }

            return result;
        }

        public List<Vector> ViewingPipeline(List<Vector> vertexbuffer)
        {
            var viewMatrix = Matrix.ViewMatrix(r, theta, phi);
            var result = new List<Vector>();

            foreach (Vector v in vertexbuffer)
            {
                Vector v2 = viewMatrix * v;

                var projectMatrix = Matrix.ProjectMatrix(d, v2.z);
                Vector v3 = projectMatrix * v2;

                result.Add(v3);
            }

            return ViewportTransformation(result);
        }

        private void DrawInfo(Graphics g)
        {
            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Black))
            {
                float x = 10;
                float y = 10;
                float lineHeight = font.GetHeight(g) + 2;

                string[] labels = {
                    $"Scale:      {scale} - S/s",
                    $"TranslateX: {posX}  - Left/right",
                    $"TranslateY: {posY}  - Up/down",
                    $"TranslateZ: {posZ}  - PgDn/PgUp",
                    "",
                    $"RotateX:    {rotX}  - X/x",
                    $"RotateY:    {rotY}  - Y/y",
                    $"RotateZ:    {rotZ}  - Z/z",
                    "",
                    $"r:    {r}  - R/r",
                    $"d:    {d}  - D/d",
                    $"phi:    {phi}  - P/p",
                    $"theta:    {theta}  - T/t",
                     "",
                    $"phase:    {phase}"
                };
                for (int i = 0; i < labels.Length; i++)
                {
                    if (!string.IsNullOrEmpty(labels[i]))
                    {
                        g.DrawString(labels[i], font, brush, x, y + (i * lineHeight));
                    }
                }
            }
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

        private void ResetValues()
        {
            posX = 0f;
            posY = 0f;
            posZ = 0f;
            rotX = 0f;
            rotY = 0f;
            rotZ = 0f;
            scale = 1f;
            animationIsPlaying = false;

            r = 10f;
            d = 800f;
            phi = -10f;
            theta = -100f;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();

            if (e.KeyCode == Keys.Left)
            {
                posX -= 0.1f;
            }
            else if (e.KeyCode == Keys.Right)
            {
                posX += 0.1f;
            }

            if (e.KeyCode == Keys.Down)
            {
                posY -= 0.1f;
            }
            else if (e.KeyCode == Keys.Up)
            {
                posY += 0.1f;
            }

            if (e.KeyCode == Keys.PageDown)
            {
                posZ -= 0.1f;
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                posZ += 0.1f;
            }

            if (e.KeyCode == Keys.X && !e.Shift)
            {
                rotX -= 1f;
            }
            else if (e.KeyCode == Keys.X && e.Shift)
            {
                rotX += 1f;
            }

            if (e.KeyCode == Keys.Y && !e.Shift)
            {
                rotY -= 1f;
            }
            else if (e.KeyCode == Keys.Y && e.Shift)
            {
                rotY += 1f;
            }

            if (e.KeyCode == Keys.Z && !e.Shift)
            {
                rotZ -= 1f;
            }
            else if (e.KeyCode == Keys.Z && e.Shift)
            {
                rotZ += 1f;
            }

            if (e.KeyCode == Keys.S && !e.Shift)
            {
                scale -= 0.1f;
            }
            else if (e.KeyCode == Keys.S && e.Shift)
            {
                scale += 0.1f;
            }

            if (e.KeyCode == Keys.R && !e.Shift)
            {
                r -= 1f;
            }
            else if (e.KeyCode == Keys.R && e.Shift)
            {
                r += 1f;
            }

            if (e.KeyCode == Keys.D && !e.Shift)
            {
                d -= 20f;
            }
            else if (e.KeyCode == Keys.D && e.Shift)
            {
                d += 20f;
            }

            if (e.KeyCode == Keys.P && !e.Shift)
            {
                phi -= 1f;
            }
            else if (e.KeyCode == Keys.P && e.Shift)
            {
                phi += 1f;
            }

            if (e.KeyCode == Keys.T && !e.Shift)
            {
                theta -= 1f;
            }
            else if (e.KeyCode == Keys.T && e.Shift)
            {
                theta += 1f;
            }

            if (e.KeyCode == Keys.C)
            {
                ResetValues();
            }

            Invalidate();
        }
    }
}