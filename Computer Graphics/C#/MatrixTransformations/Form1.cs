using System.CodeDom;
using System.ComponentModel.Design;
using System.DirectoryServices;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using Timer = System.Windows.Forms.Timer;

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
        private int phasePart = 0;

        private Timer animationTimer;
        private bool animationIsPlaying = false;

        private bool showStars = false;
        private bool useFancy = false;

        private List<Vector> stars;
        private int starsAmount = 400;

        private Bitmap canvas;
        private Graphics canvasGraphics;

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

            // Fill the world with stars to enhance the sense of depth
            Random rng = new Random();
            this.stars = new List<Vector>();

            for (int i = 0; i < starsAmount; i++)
            {
                float x = (float)(rng.NextDouble() * 20 - 10);
                float y = (float)(rng.NextDouble() * 20 - 10);
                float z = (float)(rng.NextDouble() * 20 - 10);
                stars.Add(new Vector(x, y, z));
            }

            this.animationTimer = new Timer();
            animationTimer.Interval = 50;
            animationTimer.Tick += (s, e) =>
            {
                if (animationIsPlaying)
                {
                    Animate();
                    Invalidate();
                }
            };

            canvas = new Bitmap(WIDTH, HEIGHT);
            canvasGraphics = Graphics.FromImage(canvas);
            canvasGraphics.Clear(Color.Black);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (useFancy)
            {
                using (SolidBrush fadeBrush = new SolidBrush(Color.FromArgb(40, Color.Black)))
                {
                    canvasGraphics.FillRectangle(fadeBrush, 0, 0, WIDTH, HEIGHT);
                }

                if (showStars) DrawStars(canvasGraphics);

                x_axis.Draw(canvasGraphics, ViewingPipeline(x_axis.vertexbuffer));
                y_axis.Draw(canvasGraphics, ViewingPipeline(y_axis.vertexbuffer));
                z_axis.Draw(canvasGraphics, ViewingPipeline(z_axis.vertexbuffer));
                cube.Draw(canvasGraphics, ViewingPipeline(TransformCube(cube.vertexbuffer)));

                e.Graphics.DrawImage(canvas, 0, 0);
            }
            else
            {
                e.Graphics.Clear(Color.White);

                if (showStars) DrawStars(e.Graphics);

                x_axis.Draw(e.Graphics, ViewingPipeline(x_axis.vertexbuffer));
                y_axis.Draw(e.Graphics, ViewingPipeline(y_axis.vertexbuffer));
                z_axis.Draw(e.Graphics, ViewingPipeline(z_axis.vertexbuffer));
                cube.Draw(e.Graphics, ViewingPipeline(TransformCube(cube.vertexbuffer)));
            }

            DrawInfo(e.Graphics);
        }

        private void DrawStars(Graphics g)
        {
            var projectedStars = ViewingPipeline(stars);
            foreach (var star in projectedStars)
            {
                g.FillEllipse(useFancy ? Brushes.White : Brushes.Black, star.x, star.y, 2, 2);
            }
        }

        private void Animate()
        {
            if (phase == 1)
            {
                theta -= 1f;
                if (phasePart == 1)
                {
                    if (scale <= 1.5f) scale += 0.01f;
                    else phasePart = 2;
                }
                else
                {
                    if (scale >= 1f) scale -= 0.01f;
                    else
                    {
                        scale = 1;
                        phase = 2;
                        phasePart = 1;
                    }
                }
            }
            else if (phase == 2)
            {
                theta -= 1f;
                if (phasePart == 1)
                {
                    if (rotX <= 45f) rotX += 1f;
                    else phasePart = 2;
                }
                else
                {
                    if (rotX >= 0f) rotX -= 1f;
                    else
                    {
                        rotX = 0f;
                        phase = 3;
                        phasePart = 1;
                    }
                }
            }
            else if (phase == 3)
            {
                phi += 1f;
                if (phasePart == 1)
                {
                    if (rotY <= 45f) rotY += 1f;
                    else phasePart = 2;
                }
                else
                {
                    if (rotY >= 0f) rotY -= 1f;
                    else
                    {
                        rotY = 0f;
                        phase = 4;
                        phasePart = 1;
                    }
                }
            }
            else
            {
                if (theta != -100) theta += 1.0f;
                if (phi != -10) phi -= 1.0f;
                if (phi == -10 && theta == -100) phase = 1;
            }
        }

        public List<Vector> TransformCube(List<Vector> vertexbuffer)
        {
            var result = new List<Vector>();

            var scaleMatrix = Matrix.ScaleMatrix(scale);
            var rotateMatrixX = Matrix.RotateMatrixX(this.rotX);
            var rotateMatrixY = Matrix.RotateMatrixY(this.rotY);
            var rotateMatrixZ = Matrix.RotateMatrixZ(this.rotZ);
            var translateMatrix = Matrix.TranslateMatrix(new Vector(posX, posY, posZ));

            var totalMatrix = translateMatrix * rotateMatrixZ * rotateMatrixY * rotateMatrixX * scaleMatrix;

            foreach (Vector v in vertexbuffer)
            {
                Vector v2 = totalMatrix * v;
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
            using (Brush brush = new SolidBrush(useFancy ? Color.White : Color.Black))
            {
                float x = 10;
                float y = 10;
                float lineHeight = font.GetHeight(g) + 2;

                string[] labels = {
                    $"Scale: {scale} - S/s",
                    $"TranslateX: {posX} - Left/right",
                    $"TranslateY: {posY} - Up/down",
                    $"TranslateZ: {posZ} - PgDn/PgUp",
                    "",
                    $"RotateX: {rotX} - X/x",
                    $"RotateY: {rotY} - Y/y",
                    $"RotateZ: {rotZ} - Z/z",
                    "",
                    $"r: {r} - R/r",
                    $"d: {d} - D/d",
                    $"phi: {phi} - P/p",
                    $"theta: {theta} - T/t",
                     "",
                    $"Show Stars: {showStars} - H",
                    $"Fancy Mode: {useFancy} - F",
                     "",
                    $"Animation: {animationIsPlaying} - A",
                };
                for (int i = 0; i < labels.Length; i++)
                {
                    if (!string.IsNullOrEmpty(labels[i]))
                    {
                        g.DrawString(labels[i], font, brush, x, y + (i * lineHeight));
                    }
                }
                if (phase != 0)
                {
                    string phaseInfo = $"Phase: {phase} (part: {phasePart})";
                    g.DrawString(phaseInfo, font, brush, x, y + (labels.Length * lineHeight));
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

            useFancy = false;
            showStars = false;
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

            if (e.KeyCode == Keys.A)
            {
                animationIsPlaying = !animationIsPlaying;

                if (phase == 0)
                {
                    animationTimer.Start();
                    phase = 1;
                    phasePart = 1;
                }
                else
                {
                    animationTimer.Stop();
                    phase = 0;
                    phasePart = 0;
                    ResetValues();
                }
            }

            if (e.KeyCode == Keys.H)
            {
                showStars = !showStars;
            }

            if (e.KeyCode == Keys.F)
            {
                useFancy = !useFancy;
                if (!useFancy) canvasGraphics.Clear(Color.Black);
            }

            Invalidate();
        }
    }
}