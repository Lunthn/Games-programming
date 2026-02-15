using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteeringCS
{

    public partial class MySlider : UserControl
    {
        public MySlider()
        {
            InitializeComponent();
            Init(null,"title", 0, 100, 0);
        }

        private IMySliderListener parent;

        public MySlider(IMySliderListener parent)
        {
            InitializeComponent();
            Init(parent, "title", 0, 100, 0);
        }

        public MySlider(IMySliderListener parent, string title, int min, int max, int value)
        {
            InitializeComponent();
            Init(parent, "title", min, max, value);
        }

        public void Init(IMySliderListener parent, string title, int min, int max, int value)
        {
            this.parent = parent;
            Title = title;
            Minimum = min;
            Maximum = max;
            Value = value;
            this.trackBar.SmallChange = 1;
            this.trackBar.LargeChange = 2;
            SetTickFrequency();
            this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
            tableLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.Resize += new System.EventHandler(this.ResizeForm_Resize);
        }



        public void SetTickFrequency()
        {
            int step = (Maximum - Minimum) / 10;
            this.trackBar.TickFrequency = step;
        }


        public string Title
        {
            get { return titleLabel.Text; }
            set
            {
                this.titleLabel.Text = value;
            }
        }

        public int Minimum
        {
            get { return trackBar.Minimum; }
            set
            {
                this.trackBar.Minimum = value;
                this.minValueLabel.Text = "" + value;
            }
        }


        public int Maximum
        {
            get { return trackBar.Maximum; }
            set
            {
                this.trackBar.Maximum = value;
                this.maxValueLabel.Text = "" + value;
            }
        }

        public int Value
        {
            get { return trackBar.Value; }
            set
            {
                if (value < Minimum) { value = Minimum; }
                if (value > Maximum) { value = Maximum; }
                this.trackBar.Value = value;
            }
        }


        private void trackBar_Scroll(object sender, EventArgs e)
        {
            if(parent != null)
            {
                parent.SliderValueChanged(this, trackBar.Value);
            }
        }

        private void ResizeForm_Resize(object sender, System.EventArgs e)
        {
            Invalidate();
            trackBar.Width = this.Width;
        }
    }
}
