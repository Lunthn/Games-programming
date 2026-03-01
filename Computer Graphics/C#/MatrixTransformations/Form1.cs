using System.Runtime.InteropServices;
using Timer = System.Windows.Forms.Timer;

namespace MatrixTransformations
{
    public partial class Form1 : Form
    {
        // Window dimensions

        private const int WIDTH = 1200;
        private const int HEIGHT = 700;

        // Magic number constants

        private const int FADE_ALPHA = 80;
        private const int ASCII_MIN = 33;
        private const int ASCII_MAX = 127;
        private const float TRANSLATE_STEP = 0.1f;
        private const float ROTATE_STEP = 1f;
        private const float SCALE_STEP = 0.1f;
        private const float CAMERA_R_STEP = 1f;
        private const float CAMERA_D_STEP = 20f;
        private const float CAMERA_ANGLE_STEP = 1f;

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

        private List<Vector> stars;
        private int starsAmount = 200;

        private Color currentTextColor = Color.Black;
        private Bitmap canvas;
        private Graphics canvasGraphics;

        private bool enteredMatrix = false;
        private bool hideDebug = false;

        private Random rng = new Random();

        // Windows API for dark title bar support
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        // Toggles the title bar between light and dark mode
        private void SetDarkModeTitleBar(bool enabled)
        {
            int useDarkMode = enabled ? 1 : 0;
            DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));
        }

        public Form1()
        {
            InitializeComponent();

            this.Width = WIDTH;
            this.Height = HEIGHT;
            this.DoubleBuffered = true;
            this.Text = "Matrix Transformations";

            // Define axes
            x_axis = new AxisX(3);
            y_axis = new AxisY(3);
            z_axis = new AxisZ(3);

            // Create objects
            cube = new Cube(Color.Purple);

            // Fill the world with stars to enhance the sense of depth
            this.stars = new List<Vector>();

            for (int i = 0; i < starsAmount; i++)
            {
                float x = (float)(rng.NextDouble() * 20 - 10);
                float y = (float)(rng.NextDouble() * 20 - 10);
                float z = (float)(rng.NextDouble() * 20 - 10);
                stars.Add(new Vector(x, y, z));
            }

            // Set up animation timer
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

            // Off-screen canvas used for the fading trail effect in matrix mode
            canvas = new Bitmap(WIDTH, HEIGHT);
            canvasGraphics = Graphics.FromImage(canvas);
            canvasGraphics.Clear(Color.Black);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = enteredMatrix ? canvasGraphics : e.Graphics;

            if (enteredMatrix)
            {
                // Overlay semi-transparent black to fade previous frames (motion trail effect)
                using (SolidBrush fadeBrush = new SolidBrush(Color.FromArgb(FADE_ALPHA, Color.Black)))
                    g.FillRectangle(fadeBrush, 0, 0, WIDTH, HEIGHT);
            }
            else
            {
                g.Clear(Color.White);
            }

            if (!hideDebug)
            {
                DrawAxes(g);
            }

            cube.Draw(g, ViewingPipeline(TransformModel(cube.vertexbuffer)), currentTextColor, hideDebug);

            if (enteredMatrix)
            {
                DrawStars(g);
                e.Graphics.DrawImage(canvas, 0, 0);
            }

            DrawInfo(e.Graphics);
        }

        private void DrawAxes(Graphics g)
        {
            x_axis.Draw(g, ViewingPipeline(x_axis.vertexbuffer));
            y_axis.Draw(g, ViewingPipeline(y_axis.vertexbuffer));
            z_axis.Draw(g, ViewingPipeline(z_axis.vertexbuffer));
        }

        // Centralizes all visual theme changes for matrix mode
        private void SetMatrixMode(bool enabled)
        {
            enteredMatrix = enabled;
            canvasGraphics.Clear(Color.Black);

            if (enabled)
            {
                currentTextColor = Color.Green;
                cube.color = Color.Green;
                this.Text = "=== MATRIX_MAIN_FRAME ===";
                SetDarkModeTitleBar(true);
            }
            else
            {
                canvasGraphics.Clear(Color.Black);
                currentTextColor = Color.Black;
                cube.color = Color.Purple;
                this.Text = "Matrix Transformations";
                SetDarkModeTitleBar(false);
            }
        }

        // Draws random ASCII characters at projected star positions for the matrix rain effect
        private void DrawStars(Graphics g)
        {
            using (Font matrixFont = new Font("Consolas", 8, FontStyle.Bold))
            {
                var projectedStars = ViewingPipeline(stars);
                foreach (var star in projectedStars)
                {
                    char randomChar = (char)rng.Next(ASCII_MIN, ASCII_MAX);
                    g.DrawString(randomChar.ToString(), matrixFont, Brushes.Green, star.x, star.y);
                }
            }
        }

        // Animation based on requirements
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

        // Applies scale, rotation, and translation transforms to the cube's vertices
        public List<Vector> TransformModel(List<Vector> vertexbuffer)
        {
            var result = new List<Vector>();

            var scaleMatrix = Matrix.ScaleMatrix(scale);
            var rotateMatrixX = Matrix.RotateMatrixX(this.rotX);
            var rotateMatrixY = Matrix.RotateMatrixY(this.rotY);
            var rotateMatrixZ = Matrix.RotateMatrixZ(this.rotZ);
            var translateMatrix = Matrix.TranslateMatrix(new Vector(posX, posY, posZ));

            // Combine transforms: T * Rz * Ry * Rx * S
            var totalMatrix = translateMatrix * rotateMatrixZ * rotateMatrixY * rotateMatrixX * scaleMatrix;

            foreach (Vector v in vertexbuffer)
            {
                Vector v2 = totalMatrix * v;
                result.Add(v2);
            }

            return result;
        }

        // Applies view transform, perspective projection, and viewport mapping
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
            using (Font font = new Font(enteredMatrix ? "Consolas" : "Arial", 10, FontStyle.Bold))
            using (Brush brush = new SolidBrush(currentTextColor))
            {
                float x = 20;
                float y = 20;
                float lineHeight = font.GetHeight(g) + 2;

                string debugInfo = enteredMatrix ? $"> SYSTEM_DEBUG: {!hideDebug} [M]" : $"Hide debug/controls: {hideDebug} - M";
                string resetInfo = enteredMatrix ? "> INITIALIZE_RESET [C]" : "Reset everything - C";

                g.DrawString(debugInfo, font, brush, x, y);
                g.DrawString(resetInfo, font, brush, x, y + lineHeight);

                if (!hideDebug)
                {
                    List<string> labels = new List<string> {
                        "",
                        enteredMatrix ? $"[SCALE]      {scale:F2}" : $"Scale: {scale} - S/s",
                        enteredMatrix ? $"[TRANS_X]    {posX:F1}"  : $"TranslateX: {posX} - Left/right",
                        enteredMatrix ? $"[TRANS_Y]    {posY:F1}"  : $"TranslateY: {posY} - Up/down",
                        enteredMatrix ? $"[TRANS_Z]    {posZ:F1}"  : $"TranslateZ: {posZ} - PgDn/PgUp",
                        "",
                        enteredMatrix ? $"[ROT_X]      {rotX:F0}°" : $"RotateX: {rotX} - X/x",
                        enteredMatrix ? $"[ROT_Y]      {rotY:F0}°" : $"RotateY: {rotY} - Y/y",
                        enteredMatrix ? $"[ROT_Z]      {rotZ:F0}°" : $"RotateZ: {rotZ} - Z/z",
                        "",
                        enteredMatrix ? $"[CAM_R]      {r:F1}"     : $"r: {r} - R/r",
                        enteredMatrix ? $"[CAM_D]      {d:F0}"     : $"d: {d} - D/d",
                        enteredMatrix ? $"[PHI]        {phi:F1}"   : $"phi: {phi} - P/p",
                        enteredMatrix ? $"[THETA]      {theta:F1}" : $"theta: {theta} - T/t",
                        "",
                        enteredMatrix ? $"[ANIMATION]  {animationIsPlaying} [A]" : $"Animation: {animationIsPlaying} - A"
                    };

                    if (phase != 0)
                    {
                        labels.Add(enteredMatrix ? $">> CORE_PHASE_{phase}.{phasePart}" : $"Phase: {phase} (part: {phasePart})");
                    }

                    labels.Add("");

                    for (int i = 0; i < labels.Count; i++)
                    {
                        g.DrawString(labels[i], font, brush, x, y + ((i + 2) * lineHeight));
                    }

                    float finalY = y + ((labels.Count + 2) * lineHeight);

                    if (!enteredMatrix)
                    {
                        using (Brush alertBrush = new SolidBrush(Color.Red))
                        using (font)
                        {
                            g.DrawString("TAKE THE RED PILL: PRESS [F]", font, alertBrush, x, finalY);
                        }
                    }
                    else
                    {
                        g.DrawString($"> MATRIX: ONLINE [F]", font, brush, x, finalY);
                    }
                }
            }
        }

        // Converts normalized 2D coordinates to screen space
        public static List<Vector> ViewportTransformation(List<Vector> vb)
        {
            float delta_x = WIDTH / 2;
            float delta_y = HEIGHT / 2;

            List<Vector> result = new List<Vector>();

            foreach (Vector v in vb)
            {
                float newX = v.x + delta_x;
                float newY = delta_y - v.y; // Flip Y so positive is up
                result.Add(new Vector(newX, newY));
            }

            return result;
        }

        private void ResetTransforms()
        {
            posX = 0f;
            posY = 0f;
            posZ = 0f;
            rotX = 0f;
            rotY = 0f;
            rotZ = 0f;
            scale = 1f;
            animationIsPlaying = false;
            phase = 0;

            r = 10f;
            d = 800f;
            phi = -10f;
            theta = -100f;

            canvasGraphics.Clear(Color.Black);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();

            if (e.KeyCode == Keys.Left)
            {
                posX -= TRANSLATE_STEP;
            }
            else if (e.KeyCode == Keys.Right)
            {
                posX += TRANSLATE_STEP;
            }

            if (e.KeyCode == Keys.Down)
            {
                posY -= TRANSLATE_STEP;
            }
            else if (e.KeyCode == Keys.Up)
            {
                posY += TRANSLATE_STEP;
            }

            if (e.KeyCode == Keys.PageDown)
            {
                posZ -= TRANSLATE_STEP;
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                posZ += TRANSLATE_STEP;
            }

            if (e.KeyCode == Keys.X && !e.Shift)
            {
                rotX -= ROTATE_STEP;
            }
            else if (e.KeyCode == Keys.X && e.Shift)
            {
                rotX += ROTATE_STEP;
            }

            if (e.KeyCode == Keys.Y && !e.Shift)
            {
                rotY -= ROTATE_STEP;
            }
            else if (e.KeyCode == Keys.Y && e.Shift)
            {
                rotY += ROTATE_STEP;
            }

            if (e.KeyCode == Keys.Z && !e.Shift)
            {
                rotZ -= ROTATE_STEP;
            }
            else if (e.KeyCode == Keys.Z && e.Shift)
            {
                rotZ += ROTATE_STEP;
            }

            if (e.KeyCode == Keys.S && !e.Shift)
            {
                scale -= SCALE_STEP;
            }
            else if (e.KeyCode == Keys.S && e.Shift)
            {
                scale += SCALE_STEP;
            }

            if (e.KeyCode == Keys.R && !e.Shift)
            {
                r -= CAMERA_R_STEP;
            }
            else if (e.KeyCode == Keys.R && e.Shift)
            {
                r += CAMERA_R_STEP;
            }

            if (e.KeyCode == Keys.D && !e.Shift)
            {
                d -= CAMERA_D_STEP;
            }
            else if (e.KeyCode == Keys.D && e.Shift)
            {
                d += CAMERA_D_STEP;
            }

            if (e.KeyCode == Keys.P && !e.Shift)
            {
                phi -= CAMERA_ANGLE_STEP;
            }
            else if (e.KeyCode == Keys.P && e.Shift)
            {
                phi += CAMERA_ANGLE_STEP;
            }

            if (e.KeyCode == Keys.T && !e.Shift)
            {
                theta -= CAMERA_ANGLE_STEP;
            }
            else if (e.KeyCode == Keys.T && e.Shift)
            {
                theta += CAMERA_ANGLE_STEP;
            }

            if (e.KeyCode == Keys.C)
            {
                ResetTransforms();
                SetMatrixMode(false);
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

                    ResetTransforms();
                }
            }

            // Toggle matrix visual mode (green theme + dark title bar)
            if (e.KeyCode == Keys.F)
            {
                SetMatrixMode(!enteredMatrix);
            }

            if (e.KeyCode == Keys.M)
            {
                hideDebug = !hideDebug;
                canvasGraphics.Clear(Color.Black);
            }

            Invalidate();
        }
    }
}