using System;
using System.Windows.Forms;

namespace SteeringCS
{
    public partial class Form1 : Form
    {
        private World _world;
        private System.Timers.Timer _renderTimer;

        public const int UpdateTimestepMs = 11;
        public const int RenderTimestepMs = 6;
        private bool _showInfoPanel = true;

        private int _defaultEntities = 1;
        private int _defaultTrees = 10;

        public Form1()
        {
            InitializeComponent();
            _world = new World(dbPanel1.Width, dbPanel1.Height, UpdateTimestepMs);

            _renderTimer = new System.Timers.Timer();
            _renderTimer.Elapsed += OnRenderTimerElapsed;
            _renderTimer.Interval = RenderTimestepMs;
            _renderTimer.Enabled = true;

            this.Resize += OnFormResize;
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;

            _world.IsPlaying = true;
            _world.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            entityCountTextBox.Text = _defaultEntities.ToString();
            _world.Populate(_defaultEntities, _defaultTrees);
            this.ActiveControl = null;
        }

        private void OnFormResize(object sender, EventArgs e)
        {
            Invalidate();
            _world.SetWorldSize(dbPanel1.Width, dbPanel1.Height);
            _world.ResetPositions(_world.Vehicles.Count);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                if (int.TryParse(entityCountTextBox.Text, out int count) && count > 0)
                {
                    _world.ResetPositions(count);
                }
                else
                {
                    _world.ResetPositions(_defaultEntities);
                    entityCountTextBox.Text = $"{_defaultEntities}";
                }
            }
            else if (e.KeyCode == Keys.P)
            {
                _world.IsPlaying = !_world.IsPlaying;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.D)
            {
                _world.ShowDebugInfo = !_world.ShowDebugInfo;
            }
            else if (e.KeyCode == Keys.M)
            {
                if (_showInfoPanel) this.infoPanel.Hide();
                else this.infoPanel.Show();

                _showInfoPanel = !_showInfoPanel;
            }
        }

        private void EntityCountTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                if (int.TryParse(entityCountTextBox.Text, out int count) && count > 0)
                {
                    _world.ResetPositions(count);
                }
                else
                {
                    entityCountTextBox.Text = _world.Vehicles.Count.ToString();
                }
                this.ActiveControl = null;
                e.Handled = true;
            }
        }

        private void OnRenderTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (infoLabel.IsHandleCreated)
            {
                try
                {
                    infoLabel.Invoke((MethodInvoker)delegate
                    {
                        UpdateInfoLabel();
                    });
                }
                catch { /* Handle disposal during shutdown */ }
            }
            dbPanel1.Invalidate();
        }

        private void UpdateInfoLabel()
        {
            infoLabel.Text = $"Entities: {_world.Vehicles.Count}\n" +
                           $"Status: {(_world.IsPlaying ? "Playing" : "Paused")}\n" +
                           $"Debug: {(_world.ShowDebugInfo ? "On" : "Off")}\n" +
                           $"\nControls\n" +
                           $"R - Restart\n" +
                           $"P - Pause\n" +
                           $"D - Debug Info\n" +
                           $"M - Hide label\n" +
                           $"Click - Set Target \n";
        }

        private void DbPanel1_Paint(object sender, PaintEventArgs e)
        {
            _world.Render(e.Graphics);
        }

        private void DbPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _world.SetTarget(e.X, e.Y);
            }
            this.ActiveControl = null;
        }

        private void entityCountTextBox_TextChanged(object sender, EventArgs e)
        {
        }
    }
}