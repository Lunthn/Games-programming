using System.Net;
using System.Numerics;
using System.Runtime.CompilerServices;
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

        private Axis x_axis = new Axis(3, 0, 0, "x", Color.Red);
        private Axis y_axis = new Axis(0, 3, 0, "y", Color.Green);
        private Axis z_axis = new Axis(0, 0, 3, "z", Color.Blue);

        // Objects
        public List<RenderObject> renderObjects = new List<RenderObject>();

        // Transforms are only done on the selected object
        public RenderObject selectedObject;

        private float d = 800;
        private float r = 10;
        private float theta = -100;
        private float phi = -10;

        private int phase, phasePart = 0;
        private Timer animationTimer;
        private bool animationIsPlaying = false;

        private List<Vector> stars;
        private int starsAmount = 200;

        private Color currentTextColor = Color.Black;
        private Bitmap canvas;
        private Graphics canvasGraphics;
        private bool fancyModeEnabled = false;
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

            RenderObject cube = new(new Cube(Color.Purple, Color.Black), new TransformState());

            selectedObject = cube;
            renderObjects.Add(cube);

            // Fill the world with stars to enhance the sense of depth
            this.stars = GenerateStars();

            // Set up animation timer
            this.animationTimer = new Timer();
            animationTimer.Interval = 50;
            animationTimer.Tick += (s, e) =>
            {
                if (animationIsPlaying)
                {
                    foreach (RenderObject renderObject in renderObjects) Animate(renderObject.transformState);
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
            Graphics g = fancyModeEnabled ? canvasGraphics : e.Graphics;

            if (fancyModeEnabled)
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

            foreach (RenderObject renderObject in renderObjects) renderObject.model.Draw(g, ViewingPipeline(TransformModel(renderObject.model.VertexBuffer)), hideDebug);

            if (fancyModeEnabled)
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
        private void SetFancyMode(bool enabled)
        {
            fancyModeEnabled = enabled;
            canvasGraphics.Clear(Color.Black);

            if (enabled)
            {
                currentTextColor = Color.Green;

                foreach (RenderObject renderObject in renderObjects)
                {
                    renderObject.model.Color = Color.Green;
                    renderObject.model.LabelColor = currentTextColor;
                }

                this.Text = "=== MATRIX_MAIN_FRAME ===";
                SetDarkModeTitleBar(true);
            }
            else
            {
                currentTextColor = Color.Black;

                foreach (RenderObject renderObject in renderObjects)
                {
                    renderObject.model.Color = Color.Purple;
                    renderObject.model.LabelColor = currentTextColor;
                }

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
        private void Animate(TransformState t)
        {
            if (phase == 1)
            {
                theta -= 1f;
                if (phasePart == 1)
                {
                    if (t.Scale <= 1.5f) t.Scale += 0.01f;
                    else phasePart = 2;
                }
                else
                {
                    if (t.Scale >= 1f) t.Scale -= 0.01f;
                    else
                    {
                        t.Scale = 1f;
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
                    if (t.RotX <= 45f) t.RotX += 1f;
                    else phasePart = 2;
                }
                else
                {
                    if (t.RotX >= 0f) t.RotX -= 1f;
                    else
                    {
                        t.RotX = 0f;
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
                    if (t.RotY <= 45f) t.RotY += 1f;
                    else phasePart = 2;
                }
                else
                {
                    if (t.RotY >= 0f) t.RotY -= 1f;
                    else
                    {
                        t.RotY = 0f;
                        phase = 4;
                        phasePart = 1;
                    }
                }
            }
            else
            {
                if (theta != -100) theta += 1f;
                if (phi != -10) phi -= 1f;
                if (phi == -10 && theta == -100) phase = 1;
            }
        }

        // Applies scale, rotation, and translation transforms to the cube's vertices
        public List<Vector> TransformModel(List<Vector> vertexbuffer)
        {
            var result = new List<Vector>();

            var scaleMatrix = Matrix.ScaleMatrix(selectedObject.transformState.Scale);
            var rotateMatrixX = Matrix.RotateMatrixX(selectedObject.transformState.RotX);
            var rotateMatrixY = Matrix.RotateMatrixY(selectedObject.transformState.RotY);
            var rotateMatrixZ = Matrix.RotateMatrixZ(selectedObject.transformState.RotZ);
            var translateMatrix = Matrix.TranslateMatrix(new Vector(selectedObject.transformState.PosX, selectedObject.transformState.PosY, selectedObject.transformState.PosZ));

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

        private void DrawInfo(Graphics g)
        {
            var t = selectedObject.transformState;

            using (Font font = new Font(fancyModeEnabled ? "Consolas" : "Arial", 10, FontStyle.Bold))
            using (Brush brush = new SolidBrush(currentTextColor))
            {
                float x = 20;
                float y = 20;
                float lineHeight = font.GetHeight(g) + 2;

                string debugInfo = fancyModeEnabled ? $"> SYSTEM_DEBUG: {!hideDebug} [M]" : $"Hide debug/controls: {hideDebug} - M";
                string resetInfo = fancyModeEnabled ? "> INITIALIZE_RESET [C]" : "Reset everything - C";

                g.DrawString(debugInfo, font, brush, x, y);
                g.DrawString(resetInfo, font, brush, x, y + lineHeight);

                if (!hideDebug)
                {
                    List<string> labels = new List<string> {
                        "",
                        fancyModeEnabled ? $"[SELECTED SCALE]      {t.Scale:F2}" : $"Selected Scale: {t.Scale} - S/s",
                        fancyModeEnabled ? $"[SELECTED TRANS_X]    {t.PosX:F1}"  : $"Selected TranslateX: {t.PosX} - Left/right",
                        fancyModeEnabled ? $"[SELECTED TRANS_Y]    {t.PosY:F1}"  : $"Selected TranslateY: {t.PosY} - Up/down",
                        fancyModeEnabled ? $"[SELECTED TRANS_Z]    {t.PosZ:F1}"  : $"Selected TranslateZ: {t.PosZ} - PgDn/PgUp",
                        "",
                        fancyModeEnabled ? $"[SELECTED ROT_X]      {t.RotX:F0}°" : $"Selected RotateX: {t.RotX} - X/x",
                        fancyModeEnabled ? $"[SELECTED ROT_Y]      {t.RotY:F0}°" : $"Selected RotateY: {t.RotY} - Y/y",
                        fancyModeEnabled ? $"[SELECTED ROT_Z]      {t.RotZ:F0}°" : $"Selected RotateZ: {t.RotZ} - Z/z",
                        "",
                        fancyModeEnabled ? $"[SELECTED CAM_R]      {r:F1}"     : $"r: {r} - R/r",
                        fancyModeEnabled ? $"[SELECTED CAM_D]      {d:F0}"     : $"d: {d} - D/d",
                        fancyModeEnabled ? $"[SELECTED PHI]        {phi:F1}"   : $"phi: {phi} - P/p",
                        fancyModeEnabled ? $"[SELECTED THETA]      {theta:F1}" : $"theta: {theta} - T/t",
                        "",
                        fancyModeEnabled ? $"[ANIMATION]  {animationIsPlaying} [A]" : $"Animation: {animationIsPlaying} - A"
                    };

                    if (phase != 0)
                        labels.Add(fancyModeEnabled ? $">> CORE_PHASE_{phase}.{phasePart}" : $"Phase: {phase} (part: {phasePart})");

                    labels.Add("");

                    for (int i = 0; i < labels.Count; i++)
                        g.DrawString(labels[i], font, brush, x, y + ((i + 2) * lineHeight));

                    float finalY = y + ((labels.Count + 2) * lineHeight);

                    if (!fancyModeEnabled)
                    {
                        using (Brush alertBrush = new SolidBrush(Color.Red))
                            g.DrawString("TAKE THE RED PILL: PRESS [F]", font, alertBrush, x, finalY);
                    }
                    else
                    {
                        g.DrawString("> MATRIX: ONLINE [F]", font, brush, x, finalY);
                    }
                }
            }
        }

        private void ResetTransforms()
        {
            foreach (RenderObject renderObject in renderObjects) renderObject.transformState.Reset();

            animationIsPlaying = false;

            phase = 0;

            r = 10f;
            d = 800f;
            phi = -10f;
            theta = -100f;

            canvasGraphics.Clear(Color.Black);
        }

        private List<Vector> GenerateStars()
        {
            List<Vector> stars = new List<Vector>();

            for (int i = 0; i < starsAmount; i++)
            {
                float x = (float)(rng.NextDouble() * 20 - 10);
                float y = (float)(rng.NextDouble() * 20 - 10);
                float z = (float)(rng.NextDouble() * 20 - 10);
                stars.Add(new Vector(x, y, z));
            }

            return stars;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var t = selectedObject.transformState;

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;

                // Translation
                case Keys.Left: t.PosX -= TRANSLATE_STEP; break;
                case Keys.Right: t.PosX += TRANSLATE_STEP; break;
                case Keys.Down: t.PosY -= TRANSLATE_STEP; break;
                case Keys.Up: t.PosY += TRANSLATE_STEP; break;
                case Keys.PageDown: t.PosZ -= TRANSLATE_STEP; break;
                case Keys.PageUp: t.PosZ += TRANSLATE_STEP; break;

                // Rotation & Scaling
                case Keys.X: t.RotX += e.Shift ? ROTATE_STEP : -ROTATE_STEP; break;
                case Keys.Y: t.RotY += e.Shift ? ROTATE_STEP : -ROTATE_STEP; break;
                case Keys.Z: t.RotZ += e.Shift ? ROTATE_STEP : -ROTATE_STEP; break;
                case Keys.S: t.Scale += e.Shift ? SCALE_STEP : -SCALE_STEP; break;

                // Camera Controls
                case Keys.R: r += e.Shift ? CAMERA_R_STEP : -CAMERA_R_STEP; break;
                case Keys.D: d += e.Shift ? CAMERA_D_STEP : -CAMERA_D_STEP; break;
                case Keys.P: phi += e.Shift ? CAMERA_ANGLE_STEP : -CAMERA_ANGLE_STEP; break;
                case Keys.T: theta += e.Shift ? CAMERA_ANGLE_STEP : -CAMERA_ANGLE_STEP; break;

                // Reset & Theme controls
                case Keys.C:
                    ResetTransforms();
                    SetFancyMode(false);
                    hideDebug = false;
                    break;

                case Keys.F: SetFancyMode(!fancyModeEnabled); break;
                case Keys.M:
                    hideDebug = !hideDebug;
                    canvasGraphics.Clear(Color.Black);
                    break;

                // Animation
                case Keys.A:
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
                    break;
            }

            Invalidate();
        }
    }
}