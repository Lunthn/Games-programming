using System;
using System.Windows.Forms;

namespace SteeringCS
{
    public partial class Form1 : Form, IMySliderListener
    {
        private World world;
        private System.Timers.Timer render_timer;

        public const int update_timestep_in_ms = 11;
        public const int render_timestep_in_ms = 6;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        public Form1()
        {
            InitializeComponent();
            world = new World(dbPanel1.Width, dbPanel1.Height, update_timestep_in_ms);

            render_timer = new System.Timers.Timer();
            render_timer.Elapsed += Render_timer_Elapsed;
            render_timer.Interval = render_timestep_in_ms;
            render_timer.Enabled = true;

            this.min_velocity_Slider.Init(this, "min. velocity", 0, 1000, (int)(this.world.Min_velocity_in_pixels));
            this.max_velocity_Slider.Init(this, "max. velocity", 0, 1000, (int)(this.world.Max_velocity_in_pixels));
            this.Newton_percentage_Slider.Init(this, "Newton (inertia) %", 75, 100, this.world.Newton_percentage);
            this.gravity_checkBox.Checked = world.Seek_force_like_gravity;

            this.Resize += new System.EventHandler(this.ResizeForm_Resize);

            Play = true;
            world.Start();
        }

        private void ResizeForm_Resize(object sender, System.EventArgs e)
        {
            Invalidate();
            world.Set_size(dbPanel1.Width, dbPanel1.Height);
            Console.WriteLine("Resize");
        }

        private void Play_Button_Click(object sender, EventArgs e)
        {
            Play = !Play;
        }

        public void Play_Button_Update_Text()
        {
            if (Play) { Play_button.Text = "||"; }
            else { Play_button.Text = "|>"; }
        }

        private void Step_Button_Click(object sender, EventArgs e)
        {
            Play = false;
            world.Update_simulation();
        }

        public bool Play
        {
            get { return world.Play; }
            set { world.Play = value; }
        }

        private void Render_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.dbPanel1.IsHandleCreated)
            {
                this.dbPanel1.Invoke((MethodInvoker)delegate
                {
                    // Running on the UI thread
                    Play_Button_Update_Text();
                });
            }

            dbPanel1.Invalidate(); //force new render
        }

        private void DbPanel1_Paint(object sender, PaintEventArgs e)
        {
            world.Render(e.Graphics);
        }

        private void DbPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Console.WriteLine("mouse --> right click");
            }
            else if (e.Button == MouseButtons.Left)
            {
                world.Set_seek_target(e.X, e.Y);
                Console.WriteLine("mouse --> left click: seek target");
            }
        }

        public void SliderValueChanged(MySlider source, int value)
        {
            if (source == min_velocity_Slider)
            {
                world.Min_velocity_in_pixels = min_velocity_Slider.Value;
            }
            else if (source == max_velocity_Slider)
            {
                world.Max_velocity_in_pixels = max_velocity_Slider.Value;
            }
            else if (source == Newton_percentage_Slider)
            {
                world.Newton_percentage = Newton_percentage_Slider.Value;
            }
        }

        private void ShowVectorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            world.Show_debug_info = showVectorCheckBox.Checked;
        }

        private void gravity_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            world.Seek_force_like_gravity = gravity_checkBox.Checked;
        }

        private void restart_button_Click(object sender, EventArgs e)
        {
            try
            {
                int nr_vehicles = Int32.Parse(nr_of_vehicles_textBox.Text);
                world.Randomize_positions(nr_vehicles);
            }
            catch
            {
                nr_of_vehicles_textBox.Text = "a number > 0 please!";
            }
        }
    }
}