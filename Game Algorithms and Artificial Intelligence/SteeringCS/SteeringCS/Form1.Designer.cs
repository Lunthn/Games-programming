namespace SteeringCS
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.nr_of_vehicles_textBox = new System.Windows.Forms.TextBox();
            this.restart_button = new System.Windows.Forms.Button();
            this.gravity_checkBox = new System.Windows.Forms.CheckBox();
            this.Newton_percentage_Slider = new SteeringCS.MySlider();
            this.stepButton = new System.Windows.Forms.Button();
            this.Play_button = new System.Windows.Forms.Button();
            this.max_velocity_Slider = new SteeringCS.MySlider();
            this.min_velocity_Slider = new SteeringCS.MySlider();
            this.showVectorCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dbPanel1 = new SteeringCS.Double_buffered_Panel();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.nr_of_vehicles_textBox);
            this.panel1.Controls.Add(this.restart_button);
            this.panel1.Controls.Add(this.gravity_checkBox);
            this.panel1.Controls.Add(this.Newton_percentage_Slider);
            this.panel1.Controls.Add(this.stepButton);
            this.panel1.Controls.Add(this.Play_button);
            this.panel1.Controls.Add(this.max_velocity_Slider);
            this.panel1.Controls.Add(this.min_velocity_Slider);
            this.panel1.Controls.Add(this.showVectorCheckBox);
            this.panel1.Location = new System.Drawing.Point(839, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(384, 960);
            this.panel1.TabIndex = 10;
            // 
            // nr_of_vehicles_textBox
            // 
            this.nr_of_vehicles_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nr_of_vehicles_textBox.Location = new System.Drawing.Point(116, 342);
            this.nr_of_vehicles_textBox.Name = "nr_of_vehicles_textBox";
            this.nr_of_vehicles_textBox.Size = new System.Drawing.Size(235, 30);
            this.nr_of_vehicles_textBox.TabIndex = 42;
            this.nr_of_vehicles_textBox.Text = "3";
            // 
            // restart_button
            // 
            this.restart_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.restart_button.Location = new System.Drawing.Point(5, 339);
            this.restart_button.Name = "restart_button";
            this.restart_button.Size = new System.Drawing.Size(93, 34);
            this.restart_button.TabIndex = 41;
            this.restart_button.Text = "restart";
            this.restart_button.UseVisualStyleBackColor = true;
            this.restart_button.Click += new System.EventHandler(this.restart_button_Click);
            // 
            // gravity_checkBox
            // 
            this.gravity_checkBox.AutoSize = true;
            this.gravity_checkBox.Location = new System.Drawing.Point(5, 206);
            this.gravity_checkBox.Name = "gravity_checkBox";
            this.gravity_checkBox.Size = new System.Drawing.Size(102, 20);
            this.gravity_checkBox.TabIndex = 40;
            this.gravity_checkBox.Text = "seek gravity";
            this.gravity_checkBox.UseVisualStyleBackColor = true;
            this.gravity_checkBox.CheckedChanged += new System.EventHandler(this.gravity_checkBox_CheckedChanged);
            // 
            // Newton_percentage_Slider
            // 
            this.Newton_percentage_Slider.Location = new System.Drawing.Point(3, 242);
            this.Newton_percentage_Slider.Maximum = 100;
            this.Newton_percentage_Slider.Minimum = 0;
            this.Newton_percentage_Slider.Name = "Newton_percentage_Slider";
            this.Newton_percentage_Slider.Size = new System.Drawing.Size(348, 67);
            this.Newton_percentage_Slider.TabIndex = 39;
            this.Newton_percentage_Slider.Title = "txt";
            this.Newton_percentage_Slider.Value = 0;
            // 
            // stepButton
            // 
            this.stepButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepButton.Location = new System.Drawing.Point(266, 177);
            this.stepButton.Name = "stepButton";
            this.stepButton.Size = new System.Drawing.Size(85, 49);
            this.stepButton.TabIndex = 23;
            this.stepButton.Text = "||  |>";
            this.stepButton.UseVisualStyleBackColor = true;
            this.stepButton.Click += new System.EventHandler(this.Step_Button_Click);
            // 
            // Play_button
            // 
            this.Play_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Play_button.Location = new System.Drawing.Point(183, 177);
            this.Play_button.Name = "Play_button";
            this.Play_button.Size = new System.Drawing.Size(77, 49);
            this.Play_button.TabIndex = 22;
            this.Play_button.Text = "|>";
            this.Play_button.UseVisualStyleBackColor = true;
            this.Play_button.Click += new System.EventHandler(this.Play_Button_Click);
            // 
            // max_velocity_Slider
            // 
            this.max_velocity_Slider.Location = new System.Drawing.Point(5, 93);
            this.max_velocity_Slider.Maximum = 100;
            this.max_velocity_Slider.Minimum = 0;
            this.max_velocity_Slider.Name = "max_velocity_Slider";
            this.max_velocity_Slider.Size = new System.Drawing.Size(346, 64);
            this.max_velocity_Slider.TabIndex = 21;
            this.max_velocity_Slider.Title = "txt";
            this.max_velocity_Slider.Value = 0;
            // 
            // min_velocity_Slider
            // 
            this.min_velocity_Slider.Location = new System.Drawing.Point(3, 14);
            this.min_velocity_Slider.Maximum = 100;
            this.min_velocity_Slider.Minimum = 0;
            this.min_velocity_Slider.Name = "min_velocity_Slider";
            this.min_velocity_Slider.Size = new System.Drawing.Size(348, 66);
            this.min_velocity_Slider.TabIndex = 19;
            this.min_velocity_Slider.Title = "txt";
            this.min_velocity_Slider.Value = 0;
            // 
            // showVectorCheckBox
            // 
            this.showVectorCheckBox.AutoSize = true;
            this.showVectorCheckBox.Location = new System.Drawing.Point(5, 180);
            this.showVectorCheckBox.Name = "showVectorCheckBox";
            this.showVectorCheckBox.Size = new System.Drawing.Size(126, 20);
            this.showVectorCheckBox.TabIndex = 17;
            this.showVectorCheckBox.Text = "show debug info";
            this.showVectorCheckBox.UseVisualStyleBackColor = true;
            this.showVectorCheckBox.Checked = true;
            this.showVectorCheckBox.CheckedChanged += new System.EventHandler(this.ShowVectorCheckBox_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 390F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.dbPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 966F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1226, 966);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // dbPanel1
            // 
            this.dbPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.dbPanel1.BackColor = System.Drawing.Color.White;
            this.dbPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbPanel1.Location = new System.Drawing.Point(4, 4);
            this.dbPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.dbPanel1.Name = "dbPanel1";
            this.dbPanel1.Size = new System.Drawing.Size(828, 958);
            this.dbPanel1.TabIndex = 0;
            this.dbPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.DbPanel1_Paint);
            this.dbPanel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DbPanel1_MouseClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1226, 966);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Steering";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Double_buffered_Panel dbPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button stepButton;
        private System.Windows.Forms.Button Play_button;
        private MySlider max_velocity_Slider;
        private MySlider min_velocity_Slider;
        private System.Windows.Forms.CheckBox showVectorCheckBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private MySlider Newton_percentage_Slider;
        private System.Windows.Forms.CheckBox gravity_checkBox;
        private System.Windows.Forms.Button restart_button;
        private System.Windows.Forms.TextBox nr_of_vehicles_textBox;
    }
}

